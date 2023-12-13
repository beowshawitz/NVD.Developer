using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph.Models;
using NVD.Developer.Core.Models;
using NVD.Developer.Web.Graph;
using NVD.Developer.Web.Services;

namespace NVD.Developer.Web.Pages.APIs
{
    public class AppReportModel : PageModel
    {
		private readonly ILogger<AppReportModel> _logger;
		private readonly ApplicationReportService _applicationReportService;
		private readonly GraphApiClient _graphApiClient;
		public AppReportModel(GraphApiClient graphApiClient, ApplicationReportService applicationReportService, ILogger<AppReportModel> logger) 
        {
            _applicationReportService = applicationReportService;
            _graphApiClient = graphApiClient;
			_logger = logger;
        }

		public async Task<IActionResult> OnGetAsync(int itemId)
        {
            if(itemId > 0)
            {
				var item = await _applicationReportService.GetReport(itemId);
				if(item != null)
				{
					await PopulateUserInformationAsync(item);
					return Partial("Partials/ApplicationReportPartial", item);
				}
			}
			return new JsonResult(null);
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
				if(report.Comments != null && report.Comments.Count() > 0)
				{
					for(int i = 0; i < 1; i++)
					{
						try
						{
							var user = await _graphApiClient.GetGraphApiUser(report.Comments[i].UserId);
							if (user != null)
							{
								report.Comments[i].Author = user.DisplayName;
								report.Comments[i].AuthorEmail = user.Mail;
							}
							else
							{
								report.Comments[i].Author = "Unknown";
							}
						}
						catch (Exception ex)
						{
							_logger.Log(LogLevel.Error, $"Error retrieving Graph user by id {report.Comments[i].UserId}, {ex}");
							report.Comments[i].Author = "Unknown";
						}
					}
				}
			}
		}
	}
}
