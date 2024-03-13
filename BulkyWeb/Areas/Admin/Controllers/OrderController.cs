using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize]
    public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public IEnumerable<OrderHeader> OrderHeaders { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
		{
			return View();
		}


        public IActionResult Details(int orderId)
        {
            //order page consists of both order details and header, so we use VM
            OrderVM = new()
            {
                OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };

            return View(OrderVM);
        }

        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        [HttpPost]
        public IActionResult UpdateOrderDetail()
        {
            //updating the fields
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            TempData["Success"] = "order details is updated";
            _unitOfWork.Save();

            //as we are redirecting to details page, it requires the orderHeader.Id to populate the data
            // so remember to send the 'orderId' in the parameter of the redirectAction
            return RedirectToAction(nameof(Details), new {OrderId = orderHeaderFromDb.Id});//sp we assign the orderHeaderFromDb.Id
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            /*Alt method
            //var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u=>u.Id == OrderVM.OrderHeader.Id);
            //if(orderHeaderFromDb.OrderStatus == SD.StatusApproved)
            //{
            //    orderHeaderFromDb.OrderStatus = SD.StatusInProgress;
            //    _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            //    _unitOfWork.Save();
            //    TempData["success"] = "Order Status Updated: Inprogress";
            //    return RedirectToAction(nameof(Details), new { OrderId = orderHeaderFromDb.Id });
            //}
            //TempData["error"] = "Couldn't update status";
            //return RedirectToAction(nameof(Details), new { OrderId = orderHeaderFromDb.Id });*/
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProgress);
            _unitOfWork.Save();
            TempData["Success"] = "Order Updated successfully";
            return RedirectToAction(nameof(Details), new { OrderId = OrderVM.OrderHeader.Id });
        }



        [HttpPost]
        [Authorize(Roles = SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            //updating the tracking and carrier number to db
            var ObjOrderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            ObjOrderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            ObjOrderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            ObjOrderHeaderFromDb.ShippingDate = DateTime.Now;

            //for company orders
            if (ObjOrderHeaderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                ObjOrderHeaderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(20));
            }

            //updating status
            ObjOrderHeaderFromDb.OrderStatus = SD.StatusShipped;
            _unitOfWork.OrderHeader.Update(ObjOrderHeaderFromDb);
            _unitOfWork.Save();

            //noti
            TempData["success"] = "Status Updated: Shipped !";
            return RedirectToAction(nameof(Details),new {OrderId = ObjOrderHeaderFromDb.Id});
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var OrderFromDb = _unitOfWork.OrderHeader.Get(u=>u.Id== OrderVM.OrderHeader.Id);
            if(OrderFromDb.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = OrderFromDb.PaymentIntentId,
                };

                var service = new RefundService();
                Refund refund = service.Create(options);

                _unitOfWork.OrderHeader.UpdateStatus(OrderFromDb.Id, SD.StatusCancelled, SD.StatusRefunded);
            }
            else
            {
                _unitOfWork.OrderHeader.UpdateStatus(OrderFromDb.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _unitOfWork.Save();
            TempData["success"] = "Order Cancelled Successfully";
            return RedirectToAction(nameof(Details), new { OrderId = OrderVM.OrderHeader.Id });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult PayNowPayments()
        {
            //repopulate the data in the view Model
            OrderVM.OrderHeader = _unitOfWork.OrderHeader.Get(u=>u.Id == OrderVM.OrderHeader.Id,
                                                            includeProperties:"ApplicationUser");
            OrderVM.OrderDetail = _unitOfWork.OrderDetail.GetAll(u=>u.OrderHeaderId==OrderVM.OrderHeader.Id,
                                                                includeProperties:"Product");

            //stripe logic (Same as in cart controller)
            var domain = "https://localhost:7086";
            var options = new SessionCreateOptions
            {
                //successURl of order confimation page (id is required to display the order ID)
                SuccessUrl = domain + $"/Admin/Order/PaymentConfirmation?OrderHeaderId={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"/Admin/Order/Details?orderId={OrderVM.OrderHeader.Id}",

                //consists of all product details, we handle it in below for loop
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };


            //order details has the list of items
            foreach (var item in OrderVM.OrderDetail)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    //getting the price and quantity of each item
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.Price * 100),
                        Currency = "usd",
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
                OrderVM.OrderHeader.Id, //orderId
                session.Id,                 //SessionId
                session.PaymentIntentId);//paymentIntentId is updating in the Db
            _unitOfWork.Save();


            //redirecting to stripe login page
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303); //redirecting to new URL
        }
        //to handle order confirmation page
        public IActionResult PaymentConfirmation(int OrderHeaderId)//orderID
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderHeaderId,
                                                                includeProperties: "ApplicationUser");

            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //order from company
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    //payment IntentId is generated after the payment is completed
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(
                    OrderHeaderId, //orderId
                    session.Id,                 //SessionId
                    session.PaymentIntentId);//paymentIntentId is updating in the Db

                    //we are using the same 'order status' as the order might be shipped/approved
                    _unitOfWork.OrderHeader.UpdateStatus(OrderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }

            }
            return View(OrderHeaderId);
        }



        #region API CALLS
        public IActionResult GetAll(string status)
		{
            if(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                OrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
            }
            else
            {
                //regular user
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                OrderHeaders = _unitOfWork.OrderHeader.GetAll(u=>u.ApplicationUserId==userId,
                                                        includeProperties:"ApplicationUser");
            }


            //filtering the data by orderStatus
            switch (status)
            {
                case "pending":
                    OrderHeaders = OrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "inprocess":
                    OrderHeaders = OrderHeaders.Where(u => u.OrderStatus == SD.StatusInProgress);
                    break;
                case "completed":
                    OrderHeaders = OrderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    OrderHeaders = OrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;
            }


            return Json(new {data = OrderHeaders});
		}

		#endregion
	}
}
