using Microsoft.AspNetCore.Mvc;
using NVD.Developer.Core.Managers;
using NVD.Developer.Core.Data;
using NVD.Developer.Core.Models;

namespace NVD.Developer.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ApplicationVersionController : ControllerBase
	{
		private readonly ILogger<ApplicationVersionController> _logger;
		private readonly IConfiguration _configuration;
		private readonly NvdDeveloperContext _context;

		public ApplicationVersionController(ILogger<ApplicationVersionController> logger, IConfiguration config, NvdDeveloperContext context)
		{
			_logger = logger;
			_configuration = config;
			_context = context;
		}

		[HttpGet]
		[Route("VersionExists")]
		public bool VersionExists([FromQuery] int appId, [FromQuery] string version)
		{
			if (appId <= 0 || string.IsNullOrEmpty(version))
			{
				return false;
			}
			ApplicationVersionManager versionMan = new ApplicationVersionManager(_configuration, _context);
			return versionMan.ApplicationExists(appId, version);
		}

		[HttpPost("{appId}")]
		public async Task<bool> PostAsync(int appId, [FromBody] ApplicationVersion version)
		{
			ApplicationVersionManager versionMan = new ApplicationVersionManager(_configuration, _context);
			return await versionMan.AddToVersionList(appId, version);
		}

		[HttpDelete("{appId}/{versionId}")]
		public async Task<bool> DeleteAsync(int appId, int versionId)
		{
			ApplicationVersionManager versionMan = new ApplicationVersionManager(_configuration, _context);
			return await versionMan.RemoveFromVersionList(appId, versionId);
		}
	}
}