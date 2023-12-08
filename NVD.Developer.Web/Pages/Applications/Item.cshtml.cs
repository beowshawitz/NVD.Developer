using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Web;
using NVD.Developer.Core.Models;
using NVD.Developer.Web.Authorization;
using NVD.Developer.Web.Services;
using NVD.Developer.Web.Validations;
using System.ComponentModel.DataAnnotations;

namespace NVD.Developer.Web.Pages.Applications
{
    public class ItemModel : PageModel
    {
		private readonly ILogger<ItemModel> _logger;
		private readonly ApplicationService _applicationService;
		private readonly MyListService _myAppListService;
		private readonly IAuthorizationService _authorizationService;

		public ItemModel(ILogger<ItemModel> logger, MyListService myAppListService, ApplicationService applicationService, IAuthorizationService authorizationService)
		{
			_logger = logger;
			_myAppListService = myAppListService;
			_applicationService = applicationService;
			_authorizationService = authorizationService;
		}

		[BindProperty]
		public Application ApplicationItem { get; set; } = default!;

		[BindProperty]
		public bool IsAdmin { get; set; } = false;

		public string ErrorMessage { get; set; } = string.Empty;		

		public async Task<IActionResult> OnGetAsync(int id = 0)
		{
            if (id.Equals(0))
			{
				return NotFound();
			}
			else
			{
				var applicationItem = await _applicationService.GetApplication(id);
				if (applicationItem == null)
				{
					return NotFound();
				}
				else
				{
					ApplicationItem = applicationItem;
				}
			}

			var httpContext = Request.HttpContext;
			var authorizationResult = await _authorizationService.AuthorizeAsync(httpContext.User, AuthorizationPolicies.AssignmentToAdminRoleRequired);
			IsAdmin = authorizationResult.Succeeded;
			return Page();
		}

		public async Task<IActionResult> OnPostAddToListAsync(int appId = 0, int? versionId = null)
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
					if (await _myAppListService.AddToList(userId, appId, versionId))
					{
						if(versionId == null)
						{
							HttpContext.Session.SetString("Notification", $"{app.DisplayName} was added to your list.");
						}
						else
						{
							string versionName = app.GetVersionName(versionId.Value);
							if(string.IsNullOrEmpty(versionName))
							{
								HttpContext.Session.SetString("Notification", $"{app.DisplayName} was added to your list.");
							}
							else
							{
								HttpContext.Session.SetString("Notification", $"{app.DisplayName} {versionName} was added to your list.");
							}							
						}						
					}
				}
			}
			return RedirectToPage(new { id = appId });
		}

		public async Task<IActionResult> OnPostReportAppAsync(int appId = 0)
		{
			if (appId > 0)
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
					//report on the application, based on user input
					HttpContext.Session.SetString("Notification", "Your report was received and sent to the appropriate personnel.");
				}
			}
			else
			{
				HttpContext.Session.SetString("Error", "An application with the requested identifier does not exist.");
			}
			return RedirectToPage(new { id = appId });
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
