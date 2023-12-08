using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NVD.Developer.Core;
using NVD.Developer.Core.Models;
using NVD.Developer.Web.Authorization;
using NVD.Developer.Web.Services;
using NVD.Developer.Web.Validations;
using System.ComponentModel.DataAnnotations;

namespace NVD.Developer.Web.Pages.Admin
{
	[Authorize(Policy = AuthorizationPolicies.AssignmentToAdminRoleRequired)]
	public class ItemModel : PageModel
	{
		private readonly ILogger<ItemModel> _logger;
		private readonly ApplicationService _applicationService;
		private readonly ApplicationVersionService _appVersionService;

		public ItemModel(ILogger<ItemModel> logger, ApplicationService applicationService, ApplicationVersionService appVersionService)
		{
			_logger = logger;
			_applicationService = applicationService;
			_appVersionService = appVersionService;
		}

		[BindProperty]
		public Application ApplicationItem { get; set; } = default!;

		[BindProperty]
		[Display(Name = "Application Image")]
		[MaxFileSize(500 * 1024)]
		[AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", ".gif", ".tif" }, ErrorMessage = "The {0} file type needs to be an image (png, jpg, gif, tif).")]
		[ImageDimensionsAttribute(500, 1000)]
		public IFormFile? AppImageData { get; set; }

		[BindProperty]
		public bool ClearAppImage { get; set; }

		public async Task<IActionResult> OnGetAsync(int id = 0)
		{
			ClearAppImage = false;

			if (id.Equals(0))
			{
				ApplicationItem = new Application() { Id = id };
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

			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			if (ClearAppImage)
			{
				ApplicationItem.ImageData = null;
			}
			if (AppImageData != null)
			{
				using (var dataStream = new MemoryStream())
				{
					await AppImageData.OpenReadStream().CopyToAsync(dataStream);
					ApplicationItem.ImageData = dataStream.ToArray();
				}
			}

			if (ApplicationItem.Id.Equals(0) && await _applicationService.AppExists(ApplicationItem.Name))
			{
				HttpContext.Session.SetString("Error", "An application with that name already exists.");
				return Page();
			}
			else
			{
				if (await TryUpdateModelAsync<Application>(ApplicationItem, "applicationitem",
							s => s.Id, s => s.Name, s => s.DisplayName, s => s.Description, s => s.IsLicenseRequired))
				{
					if (ApplicationItem.Id.Equals(0))
					{
						ApplicationItem.DateCreated = DateTime.UtcNow;
					}
					ApplicationItem.DateUpdated = DateTime.UtcNow;

					var newApp = await _applicationService.SaveUpdateItem(ApplicationItem);
					return RedirectToPage(new { id = newApp.Id });
				}

				return RedirectToPage();
			}
		}

		public async Task<IActionResult> OnPostAddVersionAsync(string newVersionText, int appId = -1)
		{
			if (appId > -1)
			{
				var app = await _applicationService.GetApplication(appId);

				if (app == null)
				{
					HttpContext.Session.SetString("Error", "An application with the requested identifier does not exist.");
				}
				else if (!app.HasVersion(newVersionText))
				{
					var newVersion = new ApplicationVersion { ApplicationId = appId, Name= newVersionText };
					if (await _appVersionService.AddToList(appId, newVersion))
					{
						HttpContext.Session.SetString("Notification", $"{newVersionText} was added to the application version listing.");
					}
				}
				return RedirectToPage(new { id = appId });
			}
			else
			{
				return RedirectToPage("/Applications/Index");
			}
		}

		public async Task<IActionResult> OnPostRemoveVersionAsync(int versionId, string versionName, int appId = -1)
		{
			if (appId > -1)
			{
				var app = await _applicationService.GetApplication(appId);

				if (app == null)
				{
					HttpContext.Session.SetString("Error", "An application with the requested identifier does not exist.");
				}
				else if (app.HasVersion(versionId) && await _appVersionService.RemoveFromList(appId, versionId))
				{
					HttpContext.Session.SetString("Notification", $"{versionName} was removed from the application version listing.");					
				}
				else
				{
					HttpContext.Session.SetString("Notification", $"{versionName} was not found within the application version listing.");
				}
				return RedirectToPage(new { id = appId });
			}
			else
			{
				return RedirectToPage("/Applications/Index");
			}
		}

		public async Task<IActionResult> OnPostRemoveApplicationAsync(int appId = -1)
		{
			if (appId > -1)
			{
				var app = await _applicationService.GetApplication(appId);

				if (app == null)
				{
					HttpContext.Session.SetString("Error", "An application with the requested identifier does not exist.");
				}
				else
				{
					if(await _applicationService.DeleteItem(app))
					{
						HttpContext.Session.SetString("Notification", $"{app.DisplayName} was successfully deleted from the listing.");
					}
					else
					{
						HttpContext.Session.SetString("Error", $"{app.DisplayName} was not deleted from the listing, as something went wrong or prevented the deletion.");
					}
				}
			}
			return RedirectToPage("/Applications/Index");
		}
	}
}
