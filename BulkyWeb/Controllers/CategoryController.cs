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


        //create page
        public IActionResult Create() { // no action, it redirects to the Create.html page
            return View();
        }

        // getting response from http post
        [HttpPost]
        public IActionResult Create(Category obj) //the action is performed in the category model
        {
            //if(obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("Name", "name cannot exactly match with Display Order");
            //}
            if (ModelState.IsValid) // Checks the category 'obj' is valid, and examins all the data annotation(/checks validation) of the Category model and returns bool
            {
                _db.Categories.Add(obj); // add the content from the create category form into the db
                _db.SaveChanges(); // the EFC will save the changes done
                TempData["Success"] = "Category created successfully"; //key value pair for the temp-data, which can be used for the next rendered page
                return RedirectToAction("Index", "Category");
            }
            return View();   
            
        }

        //Edit Page
        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
                return NotFound();

            Category? categoryFromDb1 = _db.Categories.Find(id); // can only work with PK 
            //Category? categoryFromDb2 = _db.Categories.FirstOrDefault(u=>u.Id == id); // can also search on Non-PK //
            //Category? categoryFromDb3 = _db.Categories.Where(u=>u.Id==id).FirstOrDefault();


            if (categoryFromDb1 == null)
            {
                return NotFound();
            }
            
            return View(categoryFromDb1);
        }

        [HttpPost]
        public IActionResult Edit(Category obj) 
        // if the 'id' of current obj is zero it is due to different Id name, either change it to -> 'Id'
        // or add a tag <input asp-for="Id-name" hidden/> is required in html of that page 
        {
            
            if (ModelState.IsValid) 
            {
                _db.Categories.Update(obj); // updates the content of the 'obj' passed in the database (Id shouldn't be zero, else EFC will add a new record)
                _db.SaveChanges(); // the EFC will save the changes done
                TempData["Success"] = "Category updated successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();

        }

		//Delete action

		public IActionResult Delete(int? id)
		{
			if (id == 0 || id == null)
				return NotFound();

			Category? categoryFromDb1 = _db.Categories.Find(id); // can only work with PK 
			if (categoryFromDb1 == null)
			{
				return NotFound();
			}

			return View(categoryFromDb1);
		}

		[HttpPost, ActionName("Delete")] //setting the name of this method explicitly as the name is changed to DeletePOST
		public IActionResult DeletePOST(int? id) // we can also use (Category obj) as parameter
		{
            Category? obj = _db.Categories.Find(id);
            if(obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);
			_db.SaveChanges(); // the EFC will save the changes done
			TempData["Success"] = "Category deleted successfully"; //key value pair for the temp-data, which can be used for the next rendered page, will stay for one request
			return RedirectToAction("Index", "Category");

		}
	}
}
