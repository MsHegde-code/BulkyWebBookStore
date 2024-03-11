using Bulky.DataAccess.Data;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ContactController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Index(Contact contact)
        {
            if (ModelState.IsValid)
            {
                _db.Contact.Add(contact);
                _db.SaveChanges();
                TempData["Success"] = "Response Recorded";
                return RedirectToAction("Index", "Contact");
            }
            TempData["Error"] = "Couldn't Record Response";
            return View();
        }
    }
}
