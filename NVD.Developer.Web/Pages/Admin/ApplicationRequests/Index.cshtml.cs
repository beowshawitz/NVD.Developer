using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NVD.Developer.Core;
using NVD.Developer.Web.Services;

namespace NVD.Developer.Web.Pages.Admin.ApplicationRequests
{
    public class IndexModel : PageModel
    {
		private readonly ILogger<IndexModel> _logger;
		private readonly ApplicationRequestService _applicationRequestService;

		public IndexModel(ILogger<IndexModel> logger, ApplicationRequestService applicationRequestService)
		{
			_logger = logger;
			_applicationRequestService = applicationRequestService;
			PageSubmission = new PageSubmission(CurrentPage);
			PageResult = new AppReqPageResult(CurrentPage, 0);
		}

		[BindProperty(SupportsGet = true)]
		public int CurrentPage { get; set; } = 1;

		[BindProperty]
		public PageSubmission PageSubmission { get; set; }

		[BindProperty]
		public AppReqPageResult? PageResult { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			PageSubmission = new PageSubmission(CurrentPage);
			try
			{
				PageResult = await _applicationRequestService.GetApplicationRequests(PageSubmission);
			}
			catch (Exception ex)
			{
				_logger.LogError("An error occurred while requesting the application request list.", ex);
				HttpContext.Session.SetString("Error", "The page request could not be completed. Please contact support if this continues to occur.");
			}
			if (PageResult == null)
			{
				PageResult = new AppReqPageResult(PageSubmission.PageSize, PageSubmission.Start);
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			try
			{
				PageResult = await _applicationRequestService.GetApplicationRequests(PageSubmission);
			}
			catch (Exception ex)
			{
				_logger.LogError("An error occurred while requesting the application request list.", ex);
				HttpContext.Session.SetString("Error", "The page request could not be completed. Please contact support if this continues to occur.");
			}
			if (PageResult == null)
			{
				PageResult = new AppReqPageResult(PageSubmission.PageSize, PageSubmission.Start);
			}
			return Page();
		}
	}
}
