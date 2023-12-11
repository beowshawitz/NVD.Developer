using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph.Models;
using NVD.Developer.Core;
using NVD.Developer.Core.Models;
using NVD.Developer.Web.Graph;
using NVD.Developer.Web.Services;

namespace NVD.Developer.Web.Pages.Admin.ApplicationReports
{
    public class IndexModel : PageModel
    {
		private readonly ILogger<IndexModel> _logger;
		private readonly ApplicationReportService _applicationReportService;
		private readonly GraphApiClient _graphApiClient;

		public IndexModel(ILogger<IndexModel> logger, ApplicationReportService applicationReportService, GraphApiClient graphApiClient)
		{
			_logger = logger;
			_applicationReportService = applicationReportService;
			_graphApiClient = graphApiClient;
			PageSubmission = new PageSubmission(CurrentPage);
			PageResult = new AppReportPageResult(CurrentPage, 0);
		}

		[BindProperty(SupportsGet = true)]
		public int CurrentPage { get; set; } = 1;

		[BindProperty]
		public PageSubmission PageSubmission { get; set; }

		[BindProperty]
		public AppReportPageResult? PageResult { get; set; }

		public async Task<IActionResult> OnGetAsync()
		{
			PageSubmission = new PageSubmission(CurrentPage);
			try
			{
				PageResult = await _applicationReportService.GetApplicationReports(PageSubmission);
			}
			catch (Exception ex)
			{
				_logger.LogError("An error occurred while requesting the application report list.", ex);
				HttpContext.Session.SetString("Error", "The page request could not be completed. Please contact support if this continues to occur.");
			}
			if (PageResult == null)
			{
				PageResult = new AppReportPageResult(PageSubmission.PageSize, PageSubmission.Start);
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
				PageResult = await _applicationReportService.GetApplicationReports(PageSubmission);
			}
			catch (Exception ex)
			{
				_logger.LogError("An error occurred while requesting the application report list.", ex);
				HttpContext.Session.SetString("Error", "The page request could not be completed. Please contact support if this continues to occur.");
			}
			if (PageResult == null)
			{
				PageResult = new AppReportPageResult(PageSubmission.PageSize, PageSubmission.Start);
			}
			else
			{
				await PostProcessPageResultAsync(PageResult);
			}
			return Page();
		}

		private async Task PostProcessPageResultAsync(AppReportPageResult pageResult)
		{
			foreach (var item in pageResult.Reports)
			{
				await PopulateUserInformationAsync(item);
			}
		}

		private async Task PopulateUserInformationAsync(ApplicationReport report)
		{
			if (report != null)
			{
				try
				{
					User? requestor = await _graphApiClient.GetGraphApiUser(report.UserId);
					if (requestor != null)
					{
						report.UserName = requestor.DisplayName;
						report.UserContactEmail = requestor.Mail;
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
