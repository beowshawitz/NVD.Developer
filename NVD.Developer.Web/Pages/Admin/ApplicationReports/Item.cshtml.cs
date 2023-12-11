using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph.Models;
using NVD.Developer.Core.Models;
using NVD.Developer.Web.Graph;
using NVD.Developer.Web.Services;

namespace NVD.Developer.Web.Pages.Admin.ApplicationReports
{
    public class ItemModel : PageModel
    {
		private readonly ApplicationReportService _applicationReportService;
		private readonly GraphApiClient _graphApiClient;
		private readonly ILogger<ItemModel> _logger;

		public ItemModel(ApplicationReportService applicationReportService, GraphApiClient graphApiClient, ILogger<ItemModel> logger)
		{
			UserReport = new ApplicationReport();
			_applicationReportService = applicationReportService;
			_graphApiClient = graphApiClient;
			_logger = logger;
		}

		[BindProperty]
		public ApplicationReport UserReport { get; set; }

		public List<ApplicationReportStatus> ReportStatusItems { get; set; }

		public async Task<IActionResult> OnGetAsync(int id = 0)
		{
			if (id.Equals(0))
			{
				return NotFound();
			}
			else
			{
				var reportItem = await _applicationReportService.GetReport(id);
				if (reportItem == null)
				{
					return NotFound();
				}
				else
				{
					UserReport = reportItem;
					await PopulateUserInformationAsync(UserReport);
				}
				ReportStatusItems = await _applicationReportService.GetStatusItems();
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			if (UserReport != null)
			{
				var newApp = await _applicationReportService.SaveUpdateItem(UserReport);
				if (newApp != null && newApp.Id > 0)
				{
					HttpContext.Session.SetString("Notification", $"Your application report was received and will be processed.");
					return RedirectToPage("/Admin/ApplicationReports/Index");
				}
				else
				{
					HttpContext.Session.SetString("Error", "The application report was not created correctly.");
				}
			}
			return RedirectToPage();
		}

		public async Task<IActionResult> OnPostUpdateStatusAsync(int newStatusId, int appRepId = 0)
		{
			if (appRepId > -1)
			{
				var appReport = await _applicationReportService.GetReport(appRepId);

				if (appReport == null)
				{
					HttpContext.Session.SetString("Error", "An application report with the reported identifier does not exist.");
				}
				else 
				{
					appReport.StatusId = newStatusId;
					appReport.Status = null;
					appReport.DateUpdated = DateTime.Now;
					var appUpdated = await _applicationReportService.SaveUpdateItem(appReport);
					if (appUpdated != null && appUpdated.Id > 0)
					{
						HttpContext.Session.SetString("Notification", $"The application status was updated.");
					}
				}
				return RedirectToPage(new { id = appRepId });
			}
			else
			{
				return RedirectToPage("/Admin/ApplicationReports/Index");
			}
		}

		public async Task PopulateUserInformationAsync(ApplicationReport report)
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
