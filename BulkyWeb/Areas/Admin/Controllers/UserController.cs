using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        private readonly UserManager<IdentityUser> _userManager;
        public UserController(
            IUnitOfWork unitOfWork, 
            ApplicationDbContext db, 
            UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _db = db;
        }

        public List<ApplicationUser> applicationUsers { get; set; }

        public IActionResult Index()
        {
            //we are using the dataTable
            return View();
        }

        public IActionResult RoleManagement(string userId)
        {
            var roleId = _db.UserRoles.FirstOrDefault(u=>u.UserId == userId).RoleId;


            var rolesVM = new RolesVM()
            {
                applicationUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == userId),
                RoleList = _db.Roles.Select(u => u.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                }),
                Companies = _db.Companies.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
			};
            rolesVM.applicationUser.Role = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;

            return View(rolesVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RolesVM rolesVM)
        {
            ///old roleId of the user
            var roleId = _db.UserRoles.FirstOrDefault(u => u.UserId == rolesVM.applicationUser.Id).RoleId;

            var oldRole = _db.Roles.FirstOrDefault(u => u.Id == roleId).Name;

            ///current role (not Mapped prop)
            if(rolesVM.applicationUser.Role != oldRole)
            {
                ///role needs to be updated
                var applicationUser = _db.ApplicationUsers.FirstOrDefault(u=>u.Id == rolesVM.applicationUser.Id);
                if (rolesVM.applicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = rolesVM.applicationUser.CompanyId;
                }
                if(oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
                _db.SaveChanges();

				_userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser,rolesVM.applicationUser.Role).GetAwaiter().GetResult();
			}
            return RedirectToAction("Index");
        }


			#region API CALLS

			public IActionResult GetAll()
        {
            applicationUsers = _unitOfWork.ApplicationUser.GetAll(includeProperties:"Company").ToList();

            //get all user role list from the db
            var userRoles = _db.UserRoles.ToList();

            //the number of roles in the db (here, 4)
            var roles = _db.Roles.ToList();

            foreach(var user in applicationUsers)
            {

                var roleId = userRoles.FirstOrDefault(u=>u.UserId==user.Id).RoleId;
                user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;


                //not all users are company users
                if (user.Company == null)
                {
                    user.Company = new Company() { Name="NA"};
                }
            }


            return Json(new {data=applicationUsers});
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            var objFromDb = _db.ApplicationUsers.FirstOrDefault(u=> u.Id == id);
            if(objFromDb==null)
            {
                return Json(new { success = false, message = "Error while locking/unlocking" });
            }
            if(objFromDb.LockoutEnd!=null && objFromDb.LockoutEnd > DateTime.Now)
            {   //user is locked
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                //user is not locked, so we lock them
                objFromDb.LockoutEnd = DateTime.Now.AddDays(5);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "changes applied !!" });
        }

        #endregion
    }
}
