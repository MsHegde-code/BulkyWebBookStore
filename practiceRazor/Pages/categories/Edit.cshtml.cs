using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using practiceRazor.Data;
using practiceRazor.Models;

namespace practiceRazor.Pages.categories
{

    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty] //important
        public Category Category{ get; set; }
        public void OnGet(int? id)
        {
            Category = _db.Categories.Find(id);
        }

        public IActionResult OnPost() {
            if(Category == null)
                return NotFound();
            _db.Categories.Update(Category);
            _db.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}
