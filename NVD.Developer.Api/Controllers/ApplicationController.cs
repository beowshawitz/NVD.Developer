using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NVD.Developer.Core.Managers;
using NVD.Developer.Core;
using NVD.Developer.Core.Data;
using NVD.Developer.Core.Models;

namespace NVD.Developer.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ApplicationController : ControllerBase
	{
		private readonly ILogger<ApplicationController> _logger;
		private readonly IConfiguration _configuration;
		private readonly NvdDeveloperContext _context;

		public ApplicationController(ILogger<ApplicationController> logger, IConfiguration config, NvdDeveloperContext context)
		{
			_logger = logger;
			_configuration = config;
			_context = context;
		}

		[HttpPost]
		[Route("GetApplications")]
		public async Task<AppPageResult> GetApplications([FromBody] PageSubmission postOptions)
		{
			ApplicationManager appMan = new ApplicationManager(_configuration, _context);
			return await appMan.GetApps(postOptions);
		}

		[HttpGet("{appId}")]
		public async Task<Application?> GetAsync(int appId)
		{
			if (appId.Equals(0))
			{
				return null;
			}
			ApplicationManager appMan = new ApplicationManager(_configuration, _context);
			return await appMan.GetApp(appId);
		}

		[HttpGet]
		[Route("ApplicationExists")]
		public bool ApplicationExists([FromQuery] string appName)
		{
			if (string.IsNullOrEmpty(appName))
			{
				return false;
			}
			ApplicationManager appMan = new ApplicationManager(_configuration, _context);
			return appMan.ApplicationExists(appName);
		}

		[HttpPut]
		public async Task<Application> PutAsync([FromBody] Application app)
		{
			ApplicationManager appMan = new ApplicationManager(_configuration, _context);
			return await appMan.UpdateApp(app);
		}

		[HttpPost]
		public async Task<Application> PostAsync([FromBody] Application app)
		{
			ApplicationManager appMan = new ApplicationManager(_configuration, _context);
			return await appMan.CreateApp(app);
		}

		[HttpDelete("{appId}")]
		public async Task<bool> DeleteAsync(int appId)
		{
			ApplicationManager appMan = new ApplicationManager(_configuration, _context);
			return await appMan.DeleteApp(appId);
		}
	}
}