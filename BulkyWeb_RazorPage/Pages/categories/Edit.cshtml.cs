using BulkyWeb_RazorPage.Data;
using BulkyWeb_RazorPage.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWeb_RazorPage.Pages.categories
{
	[BindProperties] // binds all the properties in this class
	public class EditModel : PageModel
	{

		private readonly ApplicationDbContext _db;


		//bind individual properties
		//[BindProperty] // when we perform the post action in the form, the data gets binded to this property
		public Category Category { get; set; }
		public EditModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet(int? id)
		{
			Category = _db.Categories.Find(id);
		}

		// as we want to redirect to different page, we need the return type as 'IActionResult()'
		// reset other code remains the same
		public IActionResult OnPost()
		{

			if (ModelState.IsValid)
			{
				_db.Categories.Update(Category); // updates the content of the 'obj' passed in the database (Id shouldn't be zero, else EFC will add a new record)
				_db.SaveChanges(); // the EFC will save the changes done
				TempData["Success"] = "Category updated successfully";
				return RedirectToPage("Index");
			}
			return Page();
		}
	}
}
