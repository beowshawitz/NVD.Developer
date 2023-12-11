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

		public List<ApplicationRequestStatus> RequestStatusItems { get; set; }

		public async Task<IActionResult> OnGetAsync(int id = 0)
		{
			if (id.Equals(0))
			{
				return NotFound();
			}
			else
			{
				var requestItem = await _applicationRequestService.GetRequest(id);
				if (requestItem == null)
				{
					return NotFound();
				}
				else
				{
					UserRequest = requestItem;
					await PopulateUserInformationAsync(UserRequest);
				}
				RequestStatusItems = await _applicationRequestService.GetStatusItems();
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			if (UserRequest != null)
			{
				var newApp = await _applicationRequestService.SaveUpdateItem(UserRequest);
				if (newApp != null && newApp.Id > 0)
				{
					HttpContext.Session.SetString("Notification", $"Your application request was received and will be processed.");
					return RedirectToPage("/Admin/ApplicationRequests/Index");
				}
				else
				{
					HttpContext.Session.SetString("Error", "The application request was not created correctly.");
				}
			}
			return RedirectToPage();
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
			}
		}
	}
}
