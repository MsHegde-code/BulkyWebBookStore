
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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


        public IActionResult Details(int? id)
        {
            var productDetails = _unitOfWork.Product.Get(u=>u.Id == id,includeProperties:"Category");
            return View(productDetails);
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
