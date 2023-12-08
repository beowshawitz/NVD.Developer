using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using NVD.Developer.Core;
using NVD.Developer.Web.Services;
using System.Security.Claims;

namespace NVD.Developer.Web.Pages.Applications
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationService _applicationService;
        private readonly MyListService _myAppListService;

		public IndexModel(ILogger<IndexModel> logger, MyListService myAppListService, ApplicationService applicationService)
		{
			_logger = logger;
			_myAppListService = myAppListService;
			_applicationService = applicationService;
			PageResult = new AppPageResult(CurrentPage, 0);
		}

		[BindProperty(SupportsGet = true)]
		public int CurrentPage { get; set; } = 1;

		[BindProperty]
        public PageSubmission PageSubmission { get; set; }

		[BindProperty]
        public AppPageResult PageResult { get; set; }

		public async Task<IActionResult> OnGetAsync()
        {
			PageSubmission = new PageSubmission(CurrentPage);
			try
			{
				PageResult = await _applicationService.GetApplications(PageSubmission);
			}
			catch (Exception ex)
			{
				_logger.LogError("An error occurred while requesting the application list.", ex);
				HttpContext.Session.SetString("Error", "The page request could not be completed. Please contact support if this continues to occur.");
			}
			if (PageResult == null)
			{
				PageResult = new AppPageResult(PageSubmission.PageSize, PageSubmission.Start);
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
				PageResult = await _applicationService.GetApplications(PageSubmission);
			}
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while requesting the application list.", ex);
				HttpContext.Session.SetString("Error", "The page request could not be completed. Please contact support if this continues to occur.");
			}
			if (PageResult == null)
			{
				PageResult = new AppPageResult(PageSubmission.PageSize, PageSubmission.Start);
			}
			return Page();
        }

		public async Task<IActionResult> OnPostAddToListAsync(int appId = -1)
		{
			if (appId > -1)
			{
				var app = await _applicationService.GetApplication(appId);
                var userId = GetUserId();

                if (app == null)
                {
					HttpContext.Session.SetString("Error", "An application with the requested identifier does not exist.");
                }
                else if (userId == null)
                {
					HttpContext.Session.SetString("Error", "An user identifier for the list could not be determined.");
                }
                else
                {
                    if (await _myAppListService.AddToList(userId, appId, null))
                    {
						HttpContext.Session.SetString("Notification", $"{app.DisplayName} was added to your list.");
                    }
                }
			}
			return RedirectToPage();
		}

		public string? GetUserId()
		{
			if (User.Identity is not null && User.Identity.IsAuthenticated)
			{
                return User.GetObjectId();
			}
			else
			{
				return string.Empty;
			}
		}
	}
}