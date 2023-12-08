using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph.Models;
using NVD.Developer.Core.Models;
using NVD.Developer.Web.Authorization;
using NVD.Developer.Web.Graph;
using NVD.Developer.Web.Services;

namespace NVD.Developer.Web.Pages.Applications
{
    public class RequestModel : PageModel
    {
		private readonly GraphApiClient _graphApiClient;
		private readonly ApplicationRequestService _applicationRequestService;

		public RequestModel(GraphApiClient graphApiClient, ApplicationRequestService applicationRequestService)
		{
			UserRequest = new ApplicationRequest();
			_graphApiClient = graphApiClient;
			_applicationRequestService = applicationRequestService;
		}

		public AuthenticatedUserData IdentityPrincipalData { get; set; } = default!;

		[BindProperty] 
		public ApplicationRequest UserRequest { get; set; }

		
		public void OnGet()
        {
			GetIdentityPrincipalInformation();
			if (IdentityPrincipalData != null)
			{
				UserRequest.UserId = IdentityPrincipalData.Id;
				UserRequest.UserName = IdentityPrincipalData.DisplayName;
				UserRequest.UserContactNumber = IdentityPrincipalData.Mobile;
				UserRequest.UserContactEmail = IdentityPrincipalData.Email;
			}

		}
		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}
			if (UserRequest != null)
			{
				UserRequest.DateCreated = DateTime.Now;
				UserRequest.DateUpdated = DateTime.Now;
				var newApp = await _applicationRequestService.SaveUpdateItem(UserRequest);
				if(newApp != null && newApp.Id > 0) 
				{
					HttpContext.Session.SetString("Notification", $"Your application request was received and will be processed.");
					return RedirectToPage("/Applications/Index");
				}
				else
				{
					HttpContext.Session.SetString("Error", "The application request was not created correctly.");
				}
			}
			return RedirectToPage();
		}

		private void GetIdentityPrincipalInformation()
		{
			IdentityPrincipalData = new AuthenticatedUserData();
			if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
			{
				IdentityPrincipalData.Id = User.GetUserGraphId();
				IdentityPrincipalData.Name = User.Identity.Name;
				if (!string.IsNullOrEmpty(User.GetUserGraphDisplayName()))
				{
					IdentityPrincipalData.DisplayName = User.GetUserGraphDisplayName();
				}
				if (!string.IsNullOrEmpty(User.GetUserEmail()))
				{
					IdentityPrincipalData.Email = User.GetUserEmail();
				}
				if (!string.IsNullOrEmpty(User.GetUserGraphMobile()))
				{
					IdentityPrincipalData.Mobile = User.GetUserGraphMobile();
				}

				User.Claims.ToList().ForEach(c =>
				{
					IdentityPrincipalData.Claims.Add($"{c.Type} - {c.Value}");
				});
			}
		}
	}
}
