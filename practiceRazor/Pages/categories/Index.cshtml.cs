using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using practiceRazor.Data;
using practiceRazor.Models;

namespace practiceRazor.Pages.categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public List<Category> Categories { get; set; }
        public IndexModel(ApplicationDbContext db)
        {
            _db=db;
        }
        public void OnGet()
        {
            Categories = _db.Categories.ToList();
        }
    }
}
