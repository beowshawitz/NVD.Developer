using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NVD.Developer.Web.Pages
{
	public class IndexModel : PageModel
	{
		public IndexModel()
		{
		}

		public IActionResult OnGet()
		{
			return RedirectToPage("/Applications/Index");
		}
	}
}