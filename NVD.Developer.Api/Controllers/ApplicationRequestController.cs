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
	public class ApplicationRequestController : ControllerBase
	{
		private readonly ILogger<ApplicationRequestController> _logger;
		private readonly IConfiguration _configuration;
		private readonly NvdDeveloperContext _context;

		public ApplicationRequestController(ILogger<ApplicationRequestController> logger, IConfiguration config, NvdDeveloperContext context)
		{
			_logger = logger;
			_configuration = config;
			_context = context;
		}

		[HttpPost]
		[Route("GetRequests")]
		public async Task<AppReqPageResult> GetRequests([FromBody] PageSubmission postOptions)
		{
			ApplicationRequestManager appMan = new ApplicationRequestManager(_configuration, _context);
			return await appMan.GetRequests(postOptions);
		}

		[HttpGet]
		[Route("GetUserRequests")]
		public async Task<List<ApplicationRequest>?> GetUserRequests([FromQuery] string userId)
		{
			ApplicationRequestManager appMan = new ApplicationRequestManager(_configuration, _context);
			return await appMan.GetRequests(userId);
		}

		[HttpGet]
		[Route("GetStatusItems")]
		public async Task<List<ApplicationRequestStatus>?> GetStatusItems()
		{
			ApplicationRequestManager appMan = new ApplicationRequestManager(_configuration, _context);
			return await appMan.GetStatusItems();
		}

		[HttpGet("{requestId}")]
		public async Task<ApplicationRequest?> GetAsync(int requestId)
		{
			if (requestId.Equals(0))
			{
				return null;
			}
			ApplicationRequestManager appMan = new ApplicationRequestManager(_configuration, _context);
			return await appMan.GetRequest(requestId);
		}


		[HttpPut]
		public async Task<ApplicationRequest> PutAsync([FromBody] ApplicationRequest request)
		{
			ApplicationRequestManager appMan = new ApplicationRequestManager(_configuration, _context);
			return await appMan.UpdateRequest(request);
		}

		[HttpPost]
		public async Task<ApplicationRequest> PostAsync([FromBody] ApplicationRequest request)
		{
			ApplicationRequestManager appMan = new ApplicationRequestManager(_configuration, _context);
			return await appMan.CreateRequest(request);
		}

		[HttpDelete("{requestId}")]
		public async Task<bool> DeleteAsync(int requestId)
		{
			ApplicationRequestManager appMan = new ApplicationRequestManager(_configuration, _context);
			return await appMan.DeleteRequest(requestId);
		}
	}
}