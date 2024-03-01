using BulkyWeb_RazorPage.Data;
using BulkyWeb_RazorPage.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb_RazorPage.Pages.categories
{
    public class IndexModel : PageModel
    {
        // as this is the index page of category list, we need to access the db to list the categories
        private readonly ApplicationDbContext _db;
        public List<Category> categoryList { get; set; }
        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            categoryList = _db.Categories.ToList();
        }
    }
}
