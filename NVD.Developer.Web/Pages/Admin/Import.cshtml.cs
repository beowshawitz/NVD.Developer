using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Kiota.Abstractions;
using Newtonsoft.Json;
using NVD.Developer.Core.Models;
using NVD.Developer.Web.Authorization;
using NVD.Developer.Web.Services;
using NVD.Developer.Web.Validations;
using System.ComponentModel.DataAnnotations;

namespace NVD.Developer.Web.Pages.Admin
{
	[Authorize(Policy = AuthorizationPolicies.AssignmentToAdminRoleRequired)]
	public class ImportModel : PageModel
	{
		private readonly ILogger<ImportModel> _logger;
		private readonly ApplicationService _applicationService;
		private readonly ApplicationVersionService _appVersionService;

		public ImportModel(ILogger<ImportModel> logger, ApplicationService applicationService, ApplicationVersionService appVersionService)
		{
			_logger = logger;
			_applicationService = applicationService;
			_appVersionService = appVersionService;
		}

		[BindProperty]
		[Display(Name = "Application Import")]
		[AllowedExtensions(new string[] { ".json", ".txt" }, ErrorMessage = "The {0} file type needs to be an image (json or txt).")]
		[DataType(DataType.Upload)]
		public IFormFile AppImportData { get; set; }

		public bool HasSkippedApps { get; set; } = false;
		public List<string> skippedAppNames = new List<string>();

		public IActionResult OnGet()
		{
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			if (AppImportData != null)
			{
				string jsonAppData = string.Empty;
				using (var dataStream = new MemoryStream())
				{
					await AppImportData.OpenReadStream().CopyToAsync(dataStream);
					byte[] importData = dataStream.ToArray();
					jsonAppData = System.Text.Encoding.Default.GetString(importData);
				}
				if (jsonAppData.Length > 0)
				{
					List<Application> appList = JsonConvert.DeserializeObject<List<Application>>(jsonAppData);
					int appsImported = 0;
					foreach (Application app in appList)
					{
						if (await _applicationService.AppExists(app.Name))
						{
							skippedAppNames.Add($"{app.DisplayName} already exists.");
						}
						else
						{
							if (app.IsValid())
							{
								app.DateCreated = DateTime.UtcNow;
								app.DateUpdated = DateTime.UtcNow;
								var newApp = await _applicationService.SaveUpdateItem(app);
								if(newApp != null)
								{
									appsImported++;
									if (app.Versions != null && app.Versions.Count > 0)
									{
										foreach (ApplicationVersion appVer in app.Versions)
										{
											await _appVersionService.AddToList(newApp.Id, appVer);
										}
									}
								}
							}
							else
							{
								skippedAppNames.Add($"{app.DisplayName} did not include valid input data, so it was skipped.");
							}
						}
					}
					HasSkippedApps = skippedAppNames.Count > 0;
					if(appsImported > 0)
					{
						HttpContext.Session.SetString("Notification", $"{appsImported} application(s) was/were imported into the application listing.");
					}
					else
					{
						HttpContext.Session.SetString("Warning", $"There were no applications imported. Please see the skipped list for potential issues.");
					}
				}
			}
			return Page();
		}
	}
}
