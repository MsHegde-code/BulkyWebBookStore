using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        
        //to deal with the files, and save it in the wwwroot folder
        private readonly IWebHostEnvironment _webHostEnvironment;

        List<Product> Products { get; set; }
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;   
        }

        //Index Page
        public IActionResult Index()
        {
            Products = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(Products);
        }
        
        //combining the create Page and Edit page
        public IActionResult UpSert(int? id)
        {
			// can also directly use "IEnumerable<SelectListItem>"
            // this categoryList is used for drop-down menu
			var CategoryList = _unitOfWork.Category.GetAll().Select(u=>
                new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

            // ViewBag -> sending the data using viewbag // make sure to match the 'key' in the view
            //ViewBag.CategoryList = CategoryList;

            // using ViewModels to get the category list dropdown
            ProductVM productVM = new ProductVM
            {
                Product = new Product(),
                CategoryList = CategoryList
            };

            //create functionality
            if(id==null || id == 0)
            {
				//pass the viewModel object to the view
				return View(productVM);
			}
            //update
            else
            {   
                //retrieve a particular record for edit functionality
                productVM.Product = _unitOfWork.Product.Get(u=>u.Id==id);
                return View(productVM);
            }
            
        }
        [HttpPost]
        public IActionResult UpSert(ProductVM productVM, IFormFile? file)
        {
            // first process the img, then check whether its edit or add operation based on the id
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath; //path of the wwwroot folder
                if (file != null)
                {
                    //fileName = RandomGuid+FileExtension
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    //product folder path inside wwwroot
                    string productPath = Path.Combine(wwwRootPath,@"images\product" );

                    if(!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
						//delete old image, the file path \images\product\imgName.jpg has the address from wwwroot
						var oldImgPath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImgPath))
                               System.IO.File.Delete(oldImgPath);
                    }
                    // add new image
                    using(var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                if (productVM.Product.Id == 0) {
					_unitOfWork.Product.Add(productVM.Product);
					TempData["Success"] = "Product Added successfully";
				}
                else
                {
					_unitOfWork.Product.Update(productVM.Product);
					TempData["Success"] = "Product Updated successfully";
				}
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            // if model state is not valid
            //to populate the list when the model state is Invalid, as we are loading the create view
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u=>
                new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
			
			TempData["Error"] = "Couldn't add a new Book";
            return View(productVM);
        }

        ////Edit Page

        //public IActionResult Edit(int? id)
        //{
        //    if (id == 0 || id == null)
        //        return NotFound();

        //    var UserProduct = _unitOfWork.Product.Get(u=>u.Id==id);
        //    if (UserProduct == null)
        //        return NotFound();
        //    return View(UserProduct);
        //}
        //[HttpPost]
        //public IActionResult Edit(Product product) {
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.Update(product);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Book details Updated Successfully";
        //        return RedirectToAction("Index");
        //    }
        //    TempData["Error"] = "Couldn't Update Book details";
        //    return View();
        //}

        //Delete page
        public IActionResult Delete(int? id)
        {
            if(id==0 || id==null)
                return NotFound();
            var UserProduct = _unitOfWork.Product.Get(u=>u.Id == id);
            if (UserProduct == null)
                return NotFound();
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
