using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using NVD.Developer.Web.Graph;
using NVD.Developer.Web.Authorization;
using NVD.Developer.Web.Services;
using NVD.Developer.Core.Models;

namespace NVD.Developer.Web.Pages
{
    public class UserModel : PageModel
    {
		private readonly ILogger<UserModel> _logger;
		private readonly GraphApiClient _graphApiClient;
		private readonly ApplicationRequestService _applicationRequestService;
		private readonly ApplicationReportService _applicationReportService;
		public User CurrentUser { get; set; } = default!;
		public AuthenticatedUserData IdentityPrincipalData { get; set; } = default!;
		public IList<ApplicationRequest>? AppRequests { get; set; }

		public IList<ApplicationReport>? AppReports { get; set; }

		public UserModel(ILogger<UserModel> logger, GraphApiClient graphApiClient, ApplicationRequestService applicationRequestService, ApplicationReportService applicationReportService)
		{
			_logger = logger;
			_graphApiClient = graphApiClient;
			_applicationRequestService = applicationRequestService;
			AppRequests = new List<ApplicationRequest>();
			AppReports = new List<ApplicationReport>();
			_applicationReportService = applicationReportService;
		}

		public async Task<IActionResult> OnGet()
        {
			GetIdentityPrincipalInformation();
			await GetCurrentUserGraphInformation();
			await GetApplicationRequests();
			await GetApplicationReports();
			return Page();
		}

		private async Task GetApplicationRequests()
		{
			try
			{
				if (CurrentUser != null && !string.IsNullOrEmpty(CurrentUser.Id))
				{
					AppRequests = await _applicationRequestService.GetUserApplicationRequests(CurrentUser.Id);
				}
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, $"Error retrieving application request list for the user, {ex}");
			}
		}

		private async Task GetApplicationReports()
		{
			try
			{
				if (CurrentUser != null && !string.IsNullOrEmpty(CurrentUser.Id))
				{
					AppReports = await _applicationReportService.GetUserApplicationReports(CurrentUser.Id);
				}
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, $"Error retrieving application report list for the user, {ex}");
			}
		}

		private async Task GetCurrentUserGraphInformation()
		{
			try
			{
				CurrentUser = await _graphApiClient.GetGraphApiUser();
			}
			catch (Exception ex)
			{
				_logger.Log(LogLevel.Error, $"Error retrieving Graph user, {ex}");
				CurrentUser = new User();
			}
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

	public partial class AuthenticatedUserData
	{
		public string? Id { get; set; }
		public string? Name { get; set; }
		public string? Email { get; set; }
		public string? Mobile { get; set; }
		[Display(Name = "Display Name")]
		public string? DisplayName { get; set; }

		[Display(Name = "Claims")]
		public IList<string> Claims { get; set; } = new List<string>();

		public AuthenticatedUserData()
		{
			Id = string.Empty;
			Name = string.Empty;
			Email = string.Empty;
			Mobile = string.Empty;
			DisplayName = string.Empty;
		}
	}
}
