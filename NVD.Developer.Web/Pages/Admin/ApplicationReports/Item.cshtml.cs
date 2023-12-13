using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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

		[BindProperty]
		public ApplicationReportComment NewComment { get; set; } = null!;

		[BindProperty]
		public int ItemId { get; set; }

		[BindProperty]
		public string UserGraphId { get; set; } = null!;

		public List<ApplicationReportStatus> ReportStatusItems { get; set; }

		public async Task<IActionResult> OnGetAsync(int id = 0)
		{
			if (id.Equals(0))
			{
				return NotFound();
			}
			else
			{
				ItemId = id;
				var reportItem = await _applicationReportService.GetReport(id);
				if (reportItem != null)
				{
					UserReport = reportItem;
					await PopulateUserInformationAsync(UserReport);
					ReportStatusItems = await _applicationReportService.GetStatusItems();
					NewComment = new ApplicationReportComment();
					NewComment.ReportId = id;

					UserGraphId = await _graphApiClient.GetGraphApiUserId();

					return Page();
				}				
			}
			return NotFound();
		}

		public async Task<IActionResult> OnPostUpdateStatusAsync(int newStatusId, int appRepId = 0)
		{
			if (appRepId > -1)
			{
				var reportItem = await _applicationReportService.GetReport(appRepId);

				if (reportItem == null)
				{
					HttpContext.Session.SetString("Error", "An application report with the requested identifier does not exist.");
				}
				else
				{
					reportItem.StatusId = newStatusId;
					reportItem.Status = null;
					reportItem.DateUpdated = DateTime.Now;
					var appUpdated = await _applicationReportService.SaveUpdateItem(reportItem);
					if (appUpdated != null && appUpdated.Id > 0)
					{
						HttpContext.Session.SetString("Notification", $"The application status was updated.");
					}
				}
				return RedirectToPage(new { id = appRepId });
			}
			else
			{
				return RedirectToPage("/Admin/ApplicationRequests/Index");
			}
		}

		public async Task<IActionResult> OnPostNewCommentAsync(int itemId)
		{
			if (itemId > 0)
			{
				ModelState.Clear();
				NewComment.DateCreated = DateTime.Now;
				NewComment.DateUpdated = DateTime.Now;
				if (!TryValidateModel(NewComment, nameof(ApplicationReportComment)))
				{
					HttpContext.Session.SetString("Error", "The comment did not pass validation and was not created.");
					return RedirectToPage(new { id = itemId });
				}
				bool completed = await _applicationReportService.AddComment(NewComment);
				if (completed)
				{
					HttpContext.Session.SetString("Notification", $"The comment was saved for this report.");
				}
				else
				{
					HttpContext.Session.SetString("Error", $"The comment did not get saved correctly.");
				}
				return RedirectToPage(new { id = itemId });
			}
			else
			{
				HttpContext.Session.SetString("Error", "An item with the requested identifier does not exist.");
			}
			return RedirectToPage(new { id = itemId });
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
				foreach(var comment in report.Comments)
				{
					try
					{
						var user = await _graphApiClient.GetGraphApiUser(comment.UserId);
						if(user != null)
						{
							comment.Author = user.DisplayName;
							comment.AuthorEmail = user.Mail;
						}
						else
						{
							comment.Author = "Unknown";
						}
					}
					catch (Exception ex)
					{
						_logger.Log(LogLevel.Error, $"Error retrieving Graph user by id {comment.UserId}, {ex}");
						comment.Author = "Unknown";
					}
				}
			}
		}
	}
}
