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
        public List<ApplicationListItem>? MyList { get; set; }

		[BindProperty]
		public string InstallMode { get; set; } = "I";
		[BindProperty]
		public string InstallModeDesc { get; set; } = "Interactive";

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
                    var list = await _myAppListService.GetList(userId);
                    if(list != null)
                    {
                        MyList = list.ToList();
                    }
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
            var userId = GetUserId();

            if (userId == null)
            {
				HttpContext.Session.SetString("Error", "An user identifier for the list could not be determined.");
            }
            else
            {
				var list = await _myAppListService.GetList(userId);
                if (list == null)
                {
					HttpContext.Session.SetString("Error", "There were no applications in the user list to process.");
                }
                else
                {
                    var updatedList = ApplyUserSelectionsToOriginalList(MyList, list);
					var processedList = ProcessUserSelections(updatedList);
					List<string> scripts = await _myAppListService.GenerateScript(processedList);
					GeneratedScript = PostProcess(scripts);
					HttpContext.Session.SetString("Notification", $"The requested scripts have been generated.");
                    MyList = updatedList;
				}
            }
			return Page();
		}

		public async Task<IActionResult> OnPostCreatePS1Async()
		{
			var userId = GetUserId();

			if (userId == null)
			{
				HttpContext.Session.SetString("Error", "An user identifier for the list could not be determined.");
			}
			else
			{
				var list = await _myAppListService.GetList(userId);
				if (list == null)
				{
					HttpContext.Session.SetString("Error", "There were no applications in the user list to process.");
				}
				else
				{
					string scriptOutput = string.Empty;
					var updatedList = ApplyUserSelectionsToOriginalList(MyList, list);
					var processedList = ProcessUserSelections(updatedList);
					List<string> scripts = await _myAppListService.GenerateScript(processedList);
					scriptOutput = PostProcess(scripts);
					HttpContext.Session.SetString("Notification", $"The requested scripts have been generated.");
					MyList = updatedList;
					return File(System.Text.Encoding.ASCII.GetBytes(scriptOutput), "application/octet-stream", "NVD.Dev.App.List.ps1");
				}
			}
			return Page();
		}

		private List<ApplicationListItem> ApplyUserSelectionsToOriginalList(List<ApplicationListItem> userSelections, IEnumerable<ApplicationListItem> originalList)
        {
            foreach(var item in originalList) 
            { 
                item.ApplyUserSelections(userSelections.Find(x=>x.Id.Equals(item.Id)), InstallMode);
            }
            return originalList.ToList();

		}

		private List<ApplicationListItem> ProcessUserSelections(List<ApplicationListItem> appItems)
        {
			List<ApplicationListItem> results = new List<ApplicationListItem>();
            List<int> checkedAppIds = appItems.Where(x=>x.IsSelected).Select(x=>x.Id).ToList();

            if(checkedAppIds.Count > 0)
            {
				results = appItems.Where(x => checkedAppIds.Contains(x.Id)).ToList();
			}
            else
            {
                results = appItems.ToList();
			}

            return results;
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
