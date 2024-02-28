using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ContactController(ApplicationDbContext db) {
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
                return RedirectToAction("Index", "Contact");
            }
            return View();
        }
    }
}
