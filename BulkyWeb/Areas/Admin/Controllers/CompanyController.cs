using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        List<Company> Companies { get; set; }
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public IActionResult Index()
        {
            Companies = _unitOfWork.Company.GetAll().ToList();
            return View(Companies);
        }

        public IActionResult UpSert(int? id)
        {

            if (id == null || id == 0)
            {
                return View(new Company());
            }
            //update
            else
            {
                var company = _unitOfWork.Company.Get(u => u.Id == id);
                return View(company);
            }

        }
        [HttpPost]
        public IActionResult UpSert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0) {
                    _unitOfWork.Company.Add(company);
                    TempData["Success"] = "Company Added successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    TempData["Success"] = "Company Updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(company);
        }
    

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var Companies = _unitOfWork.Company.GetAll().ToList();
            return Json(new {data=Companies});
        }


        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u=>u.Id==id);
            if(CompanyToBeDeleted == null)
            {
                return Json(new {success=false, message="error while deleting"});
            }

            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();
            
            return Json(new { success = true, message = "Company Deleted successfully" });
        }

        #endregion

    }
}


