using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bulky.Controllers
{
    public class CategoryController : Controller
    {
		private readonly ICategoryRepository _categoryRepository;

        // we are referring to the ICategoryRepository field type, and asking for its implementation using dependency injection
        public CategoryController(ICategoryRepository categoryRepository) {
            _categoryRepository = categoryRepository;
        }
        public IActionResult Index() // when the index action is performed, function returns the view of the index 
        {
            // as we want the data to be displayed in the website, we add those data into a list
            var objCategoryList = _categoryRepository.GetAll().ToList(); // list of categories, we need to pass this list to the view
            return View(objCategoryList);
        }


        //create page
        public IActionResult Create() { 
            return View();
        }

        // handling the http post request 
        [HttpPost]
        public IActionResult Create(Category obj) //the action is performed in the category model
        {
            //if(obj.Name == obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("Name", "name cannot exactly match with Display Order");
            //}
            if (ModelState.IsValid) // Checks the category 'obj' is valid, and examins all the data annotation(/checks validation) of the Category model and returns bool
            {
                _categoryRepository.Add(obj); // add the content from the create category form into the db
                _categoryRepository.Save();// the EFC (which is implemented in ICategoryRepo) will save the changes done

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

            Category? categoryFromDb1 = _categoryRepository.Get(u => u.Id == id);  // can also search on Non-PK //
																				   //Category? categoryFromDb2 = _db.Categories.FirstOrDefault(u=>u.Id == id);
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
                _categoryRepository.Update(obj); // updates the content of the 'obj' passed in the database (Id shouldn't be zero, else EFC will add a new record)
                _categoryRepository.Save(); // the EFC will save the changes done
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

			Category? categoryFromDb1 = _categoryRepository.Get(u => u.Id == id); // can only work with PK 
			if (categoryFromDb1 == null)
			{
				return NotFound();
			}

			return View(categoryFromDb1);
		}

		[HttpPost, ActionName("Delete")] //setting the name of this method explicitly as the name is changed to DeletePOST
		public IActionResult DeletePOST(int? id) // we can also use (Category obj) as parameter
		{
            Category? obj = _categoryRepository.Get(u => u.Id == id);
			if (obj == null)
            {
                return NotFound();
            }
            _categoryRepository.Remove(obj);
            _categoryRepository.Save(); // the EFC will save the changes done
			TempData["Success"] = "Category deleted successfully"; //key value pair for the temp-data, which can be used for the next rendered page, will stay for one request
			return RedirectToAction("Index", "Category");

		}
	}
}
