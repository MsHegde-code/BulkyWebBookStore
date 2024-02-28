using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        // we are referring to the ApplicationDbContext object, and asking for its implementation 
        public CategoryController(ApplicationDbContext db) {
            _db = db;
        }
        public IActionResult Index() // when the index action is performed, function returns the view of the index 
        {
            // as we want the data to be displayed in the website, we add those data into a list
            var objCategoryList = _db.Categories.ToList(); // list of categories, we need to pass this list to the view
            return View(objCategoryList);
        }

        public IActionResult Create() { // no action, it redirects to the Create.html page
            return View();
        }

        // getting response from http post
        [HttpPost]
        public IActionResult Create(Category obj) //the action is performed in the category model
        {
            if (ModelState.IsValid) // Checks the category 'obj' is valid, and examins all the data annotation(/checks validation) of the Category model and returns bool
            {
                _db.Categories.Add(obj); // add the content from the create category form into the db
                _db.SaveChanges(); // the EFC will save the changes done
                return RedirectToAction("Index", "Category");
            }
            return View();   
            
        }
    }
}
