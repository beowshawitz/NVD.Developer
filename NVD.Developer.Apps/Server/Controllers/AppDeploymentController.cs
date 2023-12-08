using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Web.Resource;
using NVD.Developer.Apps.Server.Cache;
using NVD.Developer.Apps.Shared;

namespace NVD.Developer.Apps.Server.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	public class AppDeploymentController : ControllerBase
	{
		private readonly ILogger<AppDeploymentController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public AppDeploymentController(ILogger<AppDeploymentController> logger, IConfiguration config, IMemoryCache memoryCache)
		{
			_logger = logger;
            _configuration = config;
            _memoryCache = memoryCache;
        }

		[HttpPost]
		public async Task<IEnumerable<AppDeployment>> PostAsync([FromBody] AppPageSubmission postOptions)
		{
			ApplicationManager appMan = new ApplicationManager(_configuration, _memoryCache);
            if (await appMan.LoadAppDataAsync())
            {
                return appMan.GetApps(postOptions);
            }
            else
            {
                return new List<AppDeployment>();
            }
		}

        [HttpGet("GetById")]
        public AppDeployment? GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            ApplicationManager appMan = new ApplicationManager(_configuration, _memoryCache);
            return appMan.GetApp(name);
        }
    }
}