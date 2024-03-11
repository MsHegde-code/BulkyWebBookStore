
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
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
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            //getting the userId of the current user
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //assigning to the shopping cart Application userID
            shoppingCart.ApplicationUserId = userId;


            //checking if the userID has a cart item or not
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u=>
                                            u.ApplicationUserId == userId //current user should match
                                            && u.ProductId==shoppingCart.ProductId);//IDs of product should also match


            //shopping cart already exists
            if(cartFromDb != null)
            {
                cartFromDb.Count += shoppingCart.Count; //updating the current book count
                _unitOfWork.ShoppingCart.Update(cartFromDb);  //passing "cartFromDb" because we are updating the existing item
            }
            else
            {
                //new cart
                _unitOfWork.ShoppingCart.Add(shoppingCart);
            }
            _unitOfWork.Save();
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
