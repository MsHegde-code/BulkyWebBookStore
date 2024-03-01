using BulkyWeb_RazorPage.Data;
using BulkyWeb_RazorPage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb_RazorPage.Pages.categories
{
	[BindProperties] // binds all the properties in this class
	public class CreateModel : PageModel
    {
		
		private readonly ApplicationDbContext _db;


		//bind individual properties
		//[BindProperty] // when we perform the post action in the form, the data gets binded to this property
		public Category category { get; set; }
		public CreateModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet()
        {
						
        }

		// as we want to redirect to different page, we need the return type as 'IActionResult()'
		// reset other code remains the same
		public IActionResult OnPost() {
			if (category != null)
			{
				_db.Categories.Add(category);
				_db.SaveChanges();
				TempData["Success"] = "Category created successfully";
				return RedirectToPage("Index");
			}
			return RedirectToPage("create");
		}
    }
}
