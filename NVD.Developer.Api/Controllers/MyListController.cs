using Microsoft.AspNetCore.Mvc;
using NVD.Developer.Core.Managers;
using NVD.Developer.Core.Data;
using NVD.Developer.Core.Models;

namespace NVD.Developer.Api.Controllers
{
    [ApiController]
	[Route("api/[controller]")]
	public class MyListController : ControllerBase
	{
        private readonly ILogger<MyListController> _logger;
        private readonly IConfiguration _configuration;
		private readonly NvdDeveloperContext _context;

		public MyListController(ILogger<MyListController> logger, IConfiguration config, NvdDeveloperContext context)
		{
            _logger = logger;
            _configuration = config;
			_context = context;
		}

        [HttpGet("{userId}")]
        public async Task<IEnumerable<ApplicationListItem>> Get(string userId)
		{
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            ApplicationListManager listMan = new ApplicationListManager(_configuration, _context);
            return await listMan.GetUserList(userId);
        }

        [HttpPost("{userId}")]
        public async Task<bool> Post(string userId, [FromBody] ApplicationListItem? appItem)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            ApplicationListManager listMan = new ApplicationListManager(_configuration, _context);
            if (appItem != null)
            {
                return await listMan.AddToUserList(userId, appItem);
            }
            return false;
        }

        [HttpDelete("{userId}")]
        public async Task<bool> Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            ApplicationListManager listMan = new ApplicationListManager(_configuration, _context);
            return await listMan.RemoveFromUserList(userId);
        }

		[HttpDelete("{userId}/{appId}")]
		public async Task<bool> Delete(string userId, int appId)
		{
			if (string.IsNullOrEmpty(userId) || appId.Equals(-1))
			{
				return false;
			}
			ApplicationListManager listMan = new ApplicationListManager(_configuration, _context);
			return await listMan.RemoveFromUserList(userId, appId, null);
		}

		[HttpDelete("{userId}/{appId}/{versionId}")]
		public async Task<bool> Delete(string userId, int appId, int? versionId)
		{
			if (string.IsNullOrEmpty(userId) || appId.Equals(-1))
			{
				return false;
			}
			ApplicationListManager listMan = new ApplicationListManager(_configuration, _context);
			return await listMan.RemoveFromUserList(userId, appId, versionId);
		}

		[HttpPost]
		[Route("GenerateScript")]
		public List<string> GenerateScript([FromBody] List<ApplicationListItem> list)
		{
			ApplicationListManager listMan = new ApplicationListManager(_configuration, _context);
			return listMan.GenerateScripts(list);
		}
	}
}