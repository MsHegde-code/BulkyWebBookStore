using BulkyWeb_RazorPage.Data;
using BulkyWeb_RazorPage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb_RazorPage.Pages.categories
{
    public class DeleteModel : PageModel
    {
		private readonly ApplicationDbContext _db;

        [BindProperty]
		public Category? category { get; set; }
        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        // as we are retrieveing the id from the form, we capture this in the parameter, and use the EFC Find() method to get the particular category
        public void OnGet(int id)
        {
			category = _db.Categories.Find(id);
        }
        public IActionResult OnPost()
        {
            if(category != null)
            {
				_db.Categories.Remove(category);
				_db.SaveChanges();
                TempData["Success"] = "Category deleted successfully";
				return RedirectToPage("Index");
			}
            return RedirectToPage("delete");
        }
    }
}
