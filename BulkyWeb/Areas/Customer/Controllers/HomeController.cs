
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork; // as we want to get the data from the db

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;   
        }

        public IActionResult Index() // defines the index action of the page
        {
            //getting userId
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);//we dont user value here, as it can be 'null' without a user(signed out)

            if(claim != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count()); //(claim.Value consists of userId)
            }


            IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties:"Category");

            // if we dont pass any 'view name' as the parameter, by default it takes the methodName (here index), and from the 'controller name' (HomeController) will be considered
            // hence naming the viewFiles properly is important
            return View(productList);
        }


        public IActionResult Details(int productId)
        {
            var cart = new ShoppingCart()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "Category"),
                Count = 1,
                ProductId = productId
            };
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)//items details page
        {
            //getting the userId of the current user
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //assigning to the shopping cart the userID
            shoppingCart.ApplicationUserId = userId;


            //checking if the userID has a cart item or not
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u=>
                                            u.ApplicationUserId == userId //current user should match
                                            && u.ProductId==shoppingCart.ProductId);//IDs of product should also match as it has 2 FKs


            //shopping cart already exists
            if(cartFromDb != null)
            {
                cartFromDb.Count += shoppingCart.Count; //updating the current book count
                _unitOfWork.ShoppingCart.Update(cartFromDb);  //passing "cartFromDb" because we are updating the existing item
                _unitOfWork.Save();
                TempData["success"] = "Cart Updated";
            }
            else
            {
                //new cart
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();


                //updating the cart items number(unique items count) at navBar
                //it requires the parameters in the form of (K-V)
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count()); //gets the total number of items of current user

                TempData["success"] = "Items Added to Cart";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy() // defines the privacy action of the page
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
