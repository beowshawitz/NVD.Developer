using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph.Models;
using NVD.Developer.Core;
using NVD.Developer.Core.Models;
using NVD.Developer.Web.Graph;
using NVD.Developer.Web.Services;

namespace NVD.Developer.Web.Pages.Admin.ApplicationRequests
{
    public class IndexModel : PageModel
    {
		private readonly ILogger<IndexModel> _logger;
		private readonly ApplicationRequestService _applicationRequestService;
		private readonly GraphApiClient _graphApiClient;

		public IndexModel(ILogger<IndexModel> logger, ApplicationRequestService applicationRequestService, GraphApiClient graphApiClient)
		{
			_logger = logger;
			_applicationRequestService = applicationRequestService;
			_graphApiClient = graphApiClient;
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
			else
			{
				await PostProcessPageResultAsync(PageResult);
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
			else
			{
				await PostProcessPageResultAsync(PageResult);
			}
			return Page();
		}

		private async Task PostProcessPageResultAsync(AppReqPageResult pageResult)
		{
			foreach(var item in pageResult.Requests)
			{
				await PopulateUserInformationAsync(item);
			}
		}

		private async Task PopulateUserInformationAsync(ApplicationRequest request)
		{
			if (request != null)
			{
				try
				{
					User? requestor = await _graphApiClient.GetGraphApiUser(request.UserId);
					if (requestor != null)
					{
						request.UserName = requestor.DisplayName;
						request.UserContactEmail = requestor.Mail;
					}
				}
				catch (Exception ex)
				{
					_logger.Log(LogLevel.Error, $"Error retrieving Graph user, {ex}");
				}
			}
		}
	}
}
