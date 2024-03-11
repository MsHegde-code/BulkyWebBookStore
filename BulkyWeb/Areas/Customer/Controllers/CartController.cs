using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM shoppingCartVM{ get; set; }
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
                shoppingCartList = _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId==userId,
                                                                    includeProperties:"Product").ToList()
            };


            //you can have multiple items in your cart, 
            //so you need to iterate over each and calculate its price for quantity
            foreach (var cart in shoppingCartVM.shoppingCartList)
            {
                //storing the total in the Price property of the model(NOTMAPPED)
                cart.Price = CalculateTotal(cart);

                //and storing it in orderTotal in VM
                shoppingCartVM.OrderTotal += (cart.Price);

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
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u=>u.Id== cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            if(cartFromDb.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
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
            var cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary(ShoppingCart shoppingCart)
        {

            return View();
        }



       
    }
}
