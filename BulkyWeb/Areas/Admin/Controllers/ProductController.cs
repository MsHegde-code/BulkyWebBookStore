using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        List<Product> Products { get; set; }
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //Index Page
        public IActionResult Index()
        {
            Products = _unitOfWork.Product.GetAll().ToList();
            return View(Products);
        }
        
        //create Page
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Add(product);
                _unitOfWork.Save();
                TempData["Success"] = "New Book Added successfully";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Couldn't add a new Book";
            return View();
        }

        //Edit Page

        public IActionResult Edit(int? id)
        {
            if (id == 0 || id == null)
                return NotFound();

            var UserProduct = _unitOfWork.Product.Get(u=>u.Id==id);
            return View(UserProduct);
        }
        [HttpPost]
        public IActionResult Edit(Product product) {
            if (ModelState.IsValid)
            {
                _unitOfWork.Product.Update(product);
                _unitOfWork.Save();
                TempData["success"] = "Book details Updated Successfully";
                return RedirectToAction("Index");
            }
            TempData["Error"] = "Couldn't Update Book details";
            return View();
        }

        //Delete page
        public IActionResult Delete(int? id)
        {
            if(id==0 || id==null)
                return NotFound();
            var UserProduct = _unitOfWork.Product.Get(u=>u.Id == id);
            return View(UserProduct);
        }
        [HttpPost]
        public IActionResult Delete(Product product)
        {
            //if (ModelState.IsValid) { } -> in delete function, we need not to check for the state of the model as we are not taking any input from the user,
            // The Delete action is typically associated with removing an entity, and it might not require any input from the user.
            // so we check the obj in the parameter
            if (product == null)
            {
                TempData["Error"] = "Couldn't Delete the Book";
                return RedirectToAction("Index");
            }
			_unitOfWork.Product.Remove(product);
			_unitOfWork.Save();
			TempData["Success"] = "Book Deleted Successfully";
			return RedirectToAction("Index");
		}
    }
}
