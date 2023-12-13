using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph.Models;
using NVD.Developer.Core.Models;
using NVD.Developer.Web.Graph;
using NVD.Developer.Web.Services;

namespace NVD.Developer.Web.Pages.Admin.ApplicationRequests
{
    public class ItemModel : PageModel
    {
		private readonly ApplicationRequestService _applicationRequestService;
		private readonly GraphApiClient _graphApiClient;
		private readonly ILogger<ItemModel> _logger;

		public ItemModel(ApplicationRequestService applicationRequestService, GraphApiClient graphApiClient, ILogger<ItemModel> logger)
		{
			UserRequest = new ApplicationRequest();
			_applicationRequestService = applicationRequestService;
			_graphApiClient = graphApiClient;
			_logger = logger;
		}

		[BindProperty]
		public ApplicationRequest UserRequest { get; set; }

		[BindProperty]
		public ApplicationRequestComment NewComment { get; set; } = null!;

		[BindProperty]
		public int ItemId { get; set; }

		[BindProperty]
		public string UserGraphId { get; set; } = null!;

		public List<ApplicationRequestStatus> RequestStatusItems { get; set; }

		public async Task<IActionResult> OnGetAsync(int id = 0)
		{
			if (id.Equals(0))
			{
				return NotFound();
			}
			else
			{
				ItemId = id;
				var requestItem = await _applicationRequestService.GetRequest(id);
				if (requestItem != null)
				{
					UserRequest = requestItem;
					await PopulateUserInformationAsync(UserRequest);
					RequestStatusItems = await _applicationRequestService.GetStatusItems();
					NewComment = new ApplicationRequestComment();
					NewComment.RequestId = id;

					UserGraphId = await _graphApiClient.GetGraphApiUserId();
					return Page();
				}
			}			
			return NotFound();
		}

		public async Task<IActionResult> OnPostUpdateStatusAsync(int newStatusId, int appReqId = 0)
		{
			if (appReqId > -1)
			{
				var appReq = await _applicationRequestService.GetRequest(appReqId);

				if (appReq == null)
				{
					HttpContext.Session.SetString("Error", "An application request with the requested identifier does not exist.");
				}
				else 
				{
					appReq.StatusId = newStatusId;
					appReq.Status = null;
					appReq.DateUpdated = DateTime.Now;
					var appUpdated = await _applicationRequestService.SaveUpdateItem(appReq);
					if (appUpdated != null && appUpdated.Id > 0)
					{
						HttpContext.Session.SetString("Notification", $"The application status was updated.");
					}
				}
				return RedirectToPage(new { id = appReqId });
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
				if (!TryValidateModel(NewComment, nameof(ApplicationRequestComment)))
				{
					HttpContext.Session.SetString("Error", "The comment did not pass validation and was not created.");
					return RedirectToPage(new { id = itemId });
				}
				bool completed = await _applicationRequestService.AddComment(NewComment);
				if (completed)
				{
					HttpContext.Session.SetString("Notification", $"The comment was saved for this request.");
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

		public async Task PopulateUserInformationAsync(ApplicationRequest request)
		{
			if (request != null)
			{
				try
				{
					User? requestor = await _graphApiClient.GetGraphApiUser(request.UserId);
					if(requestor != null)
					{
						request.UserName = requestor.DisplayName;
						request.UserContactEmail = requestor.Mail;
					}
				}
				catch (Exception ex)
				{
					_logger.Log(LogLevel.Error, $"Error retrieving Graph user, {ex}");
				}
				foreach (var comment in request.Comments)
				{
					try
					{
						var user = await _graphApiClient.GetGraphApiUser(comment.UserId);
						if (user != null)
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
