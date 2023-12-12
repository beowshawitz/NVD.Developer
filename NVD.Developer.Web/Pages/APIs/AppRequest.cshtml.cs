using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph.Models;
using NVD.Developer.Core.Models;
using NVD.Developer.Web.Graph;
using NVD.Developer.Web.Services;

namespace NVD.Developer.Web.Pages.APIs
{
    public class AppRequestModel : PageModel
    {
		private readonly ILogger<AppRequestModel> _logger;
		private readonly ApplicationRequestService _applicationRequestService;
		private readonly GraphApiClient _graphApiClient;
		public AppRequestModel(GraphApiClient graphApiClient, ApplicationRequestService applicationRequestService, ILogger<AppRequestModel> logger)
		{
			_applicationRequestService = applicationRequestService;
			_graphApiClient = graphApiClient;
			_logger = logger;
		}

		public async Task<IActionResult> OnGetAsync(int itemId)
		{
			if (itemId > 0)
			{
				var item = await _applicationRequestService.GetRequest(itemId);
				if (item != null)
				{
					await PopulateUserInformationAsync(item);
					return Partial("Partials/ApplicationRequestPartial", item);
				}
			}
			return new JsonResult(null);
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
