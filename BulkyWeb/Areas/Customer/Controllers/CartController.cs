using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		//the property gets populated after post action also
		[BindProperty]
		public ShoppingCartVM shoppingCartVM { get; set; }
		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			//now we need to get the userId of the current user
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			shoppingCartVM = new ShoppingCartVM()
			{
				shoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
																	includeProperties: "Product").ToList(),
				OrderHeader = new OrderHeader()
				//as we are accessing this object in the view, we need to create it 
			};


			//you can have multiple items in your cart, 
			//so you need to iterate over each and calculate its price for quantity
			foreach (var cart in shoppingCartVM.shoppingCartList)
			{
				//storing the total in the Price property of the model(NOTMAPPED)
				cart.Price = CalculateTotal(cart);

				//and storing it in orderTotal in VM
				shoppingCartVM.OrderHeader.OrderTotal += (cart.Price);

				//example:
				//40 items -> price: 2.25*40
				//60 items -> price: 1.35*60
			}

			return View(shoppingCartVM);
		}
		//calculating the price for the quantity of each item type
		private double CalculateTotal(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 50)
			{
				return shoppingCart.Count * shoppingCart.Product.Price;
			}
			else if (shoppingCart.Count > 50 && shoppingCart.Count < 100)
			{
				return shoppingCart.Count * shoppingCart.Product.Price50;
			}
			return shoppingCart.Count * shoppingCart.Product.Price100;

		}



		//adding actions for the plus, minus, delete buttons in the cart page
		public IActionResult Plus(int cartId)
		{
			//retrieve the cartdetails from the DB
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
			cartFromDb.Count += 1;
			_unitOfWork.ShoppingCart.Update(cartFromDb);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked:true);//here tracking is set to true, as this obj is manipulate in further lines of code
			if (cartFromDb.Count <= 1)
			{
				_unitOfWork.ShoppingCart.Remove(cartFromDb);
                //handle the session for the cart item number, as the cartFromDb is removed, we need to subtract 1 from the count(as its already removed)
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count() - 1);
            }
			else
			{
				cartFromDb.Count -= 1;
				_unitOfWork.ShoppingCart.Update(cartFromDb);

			}
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId)
		{
			var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId, tracked:true);
			
            //handle the session for the cart item number, as the cartFromDb is removed at the next line
			//, we need to subtract 1 from the count(assume its already removed)
            HttpContext.Session.SetInt32(SD.SessionCart,
                _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cartFromDb.ApplicationUserId).Count()-1);

            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}


		//order summary action
		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			shoppingCartVM = new()
			{
				shoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId,
																	includeProperties: "Product"),
				OrderHeader = new OrderHeader()
			};

			//to populate the data using the Application user to the OrderHeader
			shoppingCartVM.OrderHeader.ApplicationUserId = userId;
			shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

			//retrieve the data from ApplicationUser inside OrderHeader       
			shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
			shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
			shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
			shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
			shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
			shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;



			//calculate the total price of all items
			foreach (var cart in shoppingCartVM.shoppingCartList)
			{
				cart.Price = CalculateTotal(cart);
				shoppingCartVM.OrderHeader.OrderTotal += (cart.Price);

			}

			return View(shoppingCartVM);
		}

		[HttpPost]
		[ActionName("Summary")]
		public IActionResult SummaryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //to populate the data using the Application user to the OrderHeader
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;
            //we dont need to populate the applicationUser 'obj' as EFC will handle it by navigation Property


            shoppingCartVM.shoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, //always match with application userID
																	includeProperties: "Product");


			//retrieve the data from ApplicationUser inside OrderHeader       
			shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

			//calculate the total price of all items
			foreach (var cart in shoppingCartVM.shoppingCartList)
			{
				cart.Price = CalculateTotal(cart);
				shoppingCartVM.OrderHeader.OrderTotal += (cart.Price);

			}
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);//applicationUser IDs to check the type of user 

                                                                                                   //checking if the current user is Company user, if yes we can skip payment
                                                                                                   //the companyID can be null or have value, so we need to use GetValueOrDefault()
            if (applicationUser.CompanyId.GetValueOrDefault() == 0) //if company id is zero
			{
				//customer account
				shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
				shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
			}
			else
			{
				//company account
				shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
				shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;

			}
			//add the data into the db
			_unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
			_unitOfWork.Save();

			//for order details record, we need to add into DB
			foreach (var cart in shoppingCartVM.shoppingCartList)
			{
				OrderDetail orderDetail = new()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = shoppingCartVM.OrderHeader.Id, //orderHeader would be populated in the earlier steps
					Count = cart.Count,
					Price = cart.Price
				};
				//keep Add methods inside for loop, as we need to add all the records which is inside the cart
				_unitOfWork.OrderDetail.Add(orderDetail);
				_unitOfWork.Save();
			}

            ////stripe logic (from stripe documentation)
			
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
			{   //regular customer, payment capture logic, 
				StripeConfiguration.ApiKey = "sk_test_tR3PYbcVNZZ796tH88S4VQ2u";
				var domain = "https://localhost:7086";
				var options = new SessionCreateOptions
				{
					//successURl of order confimation page (id is required to display the order ID)
					SuccessUrl = domain + $"/Customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
					CancelUrl = domain + "/Customer/cart/index",

					//consists of all product details, we handle it in below for loop
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
				};

				foreach(var item in shoppingCartVM.shoppingCartList)
				{
					var sessionLineItem = new SessionLineItemOptions
					{
						//getting the price and quantity of each item
						PriceData = new SessionLineItemPriceDataOptions()
						{
							UnitAmount = (long)(item.Price * 100),
							Currency = "inr",
							ProductData = new SessionLineItemPriceDataProductDataOptions()
							{
								Name = item.Product.Title
							}
						},
						Quantity = item.Count
					};
					//adding all the data to the lineItem object which we created in options
					options.LineItems.Add(sessionLineItem);
				}

				var service = new SessionService();
				//session is generated after the stripe payment
				// it consists of SessionId, paymentIntentId so we can update in the table
				Session session = service.Create(options);
				_unitOfWork.OrderHeader.UpdateStripePaymentID(
					shoppingCartVM.OrderHeader.Id, //orderId
					session.Id,					//SessionId
					session.PaymentIntentId);//paymentIntentId is updating in the Db
				_unitOfWork.Save();


				//redirecting to stripe login page
				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303); //redirecting to new URL
			}
			return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.OrderHeader.Id });
		}


		//if user hits back button on stripe page, the redudant sessionId gets removed
		//private SessionAfterExpiration CancelUserPayment(string domain)
		//{
		//	//checking for payment status
		//	var CheckOrderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == shoppingCartVM.OrderHeader.Id);
		//	if (CheckOrderFromDb.OrderStatus == SD.StatusPending && CheckOrderFromDb.PaymentStatus == SD.StatusPending)
		//	{
		//		_unitOfWork.OrderHeader.Remove(CheckOrderFromDb);
		//		_unitOfWork.Save();
		//		TempData["error"] = "Payment Failed !!";
		//	}
		//}

		//orderConfirmation is used to check whether the payment is successful or not
		public IActionResult OrderConfirmation(int id)//orderID
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id,
																includeProperties: "ApplicationUser");

			if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
			{
				//order from regular customer
				var service = new SessionService();
				Session session = service.Get(orderHeader.SessionId);

				if (session.PaymentStatus.ToLower() == "paid")
				{
					//payment IntentId is generated after the payment is completed
					_unitOfWork.OrderHeader.UpdateStripePaymentID(
					id, //orderId
					session.Id,                 //SessionId
					session.PaymentIntentId);//paymentIntentId is updating in the Db

					_unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
					_unitOfWork.Save();
				}
			}
			//clearing out the shoppingCart after payment
			//retrieving the cart of that particular orderHeader account
			List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u =>
				u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			_unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
			_unitOfWork.Save();
			
			return View(id);
		}

	}
}
