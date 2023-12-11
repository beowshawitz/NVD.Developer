using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using NVD.Developer.Core.Models;
using NVD.Developer.Web.Services;
using System;

namespace NVD.Developer.Web.Pages.List
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationService _applicationService;
        private readonly MyListService _myAppListService;

        public IndexModel(ILogger<IndexModel> logger, MyListService myAppListService, ApplicationService applicationService)
        {
            _logger = logger;
            _myAppListService = myAppListService;
            _applicationService = applicationService;
        }

        [BindProperty]
        public IEnumerable<ApplicationListItem>? MyList { get; set; }

        [BindProperty]
        public string? GeneratedScript { get; set; } = string.Empty;

        public bool ContainsListItems
        {
            get
            {
                if (MyList != null && MyList.Any())
                {
                    return true;
                }
                else { return false; }
            }
        }

        public async Task<IActionResult> OnGetAsync()
		{
			try
            {
                var userId = GetUserId();

                if (userId == null)
                {
					HttpContext.Session.SetString("Error", "An user identifier for the list could not be determined.");
                }
                else
                {
                    MyList = await _myAppListService.GetList(userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while requesting the application list.", ex);
				HttpContext.Session.SetString("Error", "Unable to retrieve the list of applications for the logged in user.");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveFromCartAsync(int appId, int? versionId)
        {
			if (ModelState.IsValid)
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
                    if (await _myAppListService.RemoveFromList(userId, appId, versionId))
                    {
						HttpContext.Session.SetString("Notification", $"{app.DisplayName} was removed from your list.");
                    }
                }
            }
			return RedirectToPage();
		}

        public async Task<IActionResult> OnPostRemoveAllFromCartAsync()
        {
			if (ModelState.IsValid)
            {
                var userId = GetUserId();

                if (userId == null)
                {
					HttpContext.Session.SetString("Error", "An user identifier for the list could not be determined.");
                }
                else
                {
                    if (await _myAppListService.RemoveAllFromList(userId))
                    {
						HttpContext.Session.SetString("Notification", $"All applications were removed from your list.");
                    }
                }
            }
			return RedirectToPage();
		}

        public async Task<IActionResult> OnPostGenerateScriptAsync()
        {
			if (ModelState.IsValid)
            {
                var userId = GetUserId();

                if (userId == null)
                {
					HttpContext.Session.SetString("Error", "An user identifier for the list could not be determined.");
                }
                else
                {
					MyList = await _myAppListService.GetList(userId);
                    if (MyList == null)
                    {
						HttpContext.Session.SetString("Error", "There were no applications in the user list to process.");
                    }
                    else
                    {
                        var list = ProcessForm(Request.Form, MyList);
						List<string> scripts = await _myAppListService.GenerateScript(list);
						GeneratedScript = PostProcess(scripts);
						HttpContext.Session.SetString("Notification", $"The requested scripts have been generated.");
                    }
                }
            }
			return Page();
		}

        private List<ApplicationListItem> ProcessForm(IFormCollection form, IEnumerable<ApplicationListItem> appItems)
        {
			List<ApplicationListItem> results = new List<ApplicationListItem>();
            List<int> checkedAppIds = GetCheckedAppItems(form, appItems);

            if(checkedAppIds.Count > 0)
            {
				results = MyList.Where(x => checkedAppIds.Contains(x.Id)).ToList();
			}
            else
            {
                results = MyList.ToList();
			}

            ApplyFormSelections(form, results);

            return results;
		}

        private List<int> GetCheckedAppItems(IFormCollection form, IEnumerable<ApplicationListItem> appItems)
        {
			List<int> checkedItems = new List<int>();
            foreach(var appItem in appItems)
            {
                if (form[$"appChk_{appItem.Id}"] == "checked" || form[$"appChk_{appItem.Id}"] == "true")
                {
					checkedItems.Add(appItem.Id);
				}
            }
            return checkedItems;
		}

		private void ApplyFormSelections(IFormCollection form, List<ApplicationListItem> appItems)
		{
			foreach (var appItem in appItems)
			{
                appItem.ApplyInstallAsFromForm(form[$"installAs_{appItem.Id}"]);
				appItem.ApplyInstallModeFromForm(form["installMode"]);
				appItem.ApplyUninstallPreviousFromForm(form[$"uninstallPrev_{appItem.Id}"]);
				appItem.ApplyAcceptAgreementsFromForm(form[$"acceptAgreements_{appItem.Id}"]);
			}
		}

		public async Task<IActionResult> OnPostCreatePS1Async(string? listIds)
		{
			var userId = GetUserId();

			if (userId == null)
			{
				HttpContext.Session.SetString("Error", "An user identifier for the list could not be determined.");
			}
			else
			{
				MyList = await _myAppListService.GetList(userId);
				if (MyList == null)
				{
					HttpContext.Session.SetString("Error", "There were no applications in the user list to process.");
				}
				else
				{
                    string scriptOutput = string.Empty;
					if (listIds == null)
					{
						List<string> scripts = await _myAppListService.GenerateScript(MyList.ToList());
						scriptOutput = PostProcess(scripts);
					}
					else
					{
						//get scripts from listIds
						int[] array = listIds.Split(',').Select(int.Parse).ToArray();
						List<string> scripts = await _myAppListService.GenerateScript(MyList.Where(x => array.Contains(x.Id)).ToList());
						scriptOutput = PostProcess(scripts);
					}
					HttpContext.Session.SetString("Notification", $"The requested scripts have been generated.");

					return File(System.Text.Encoding.ASCII.GetBytes(scriptOutput), "application/octet-stream", "NVD.Dev.App.List.ps1");
				}
			}
			return Page();
		}

		private string? GetUserId()
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

        private string PostProcess(List<string> generatedScripts)
        {
            string result = string.Empty;
            if(generatedScripts != null)
            {
				result = string.Join($";{Environment.NewLine}", generatedScripts);
			}
            return result;
		}
	}
}
