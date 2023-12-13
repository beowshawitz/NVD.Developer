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
	public class ApplicationReportController : ControllerBase
	{
		private readonly ILogger<ApplicationReportController> _logger;
		private readonly IConfiguration _configuration;
		private readonly NvdDeveloperContext _context;

		public ApplicationReportController(ILogger<ApplicationReportController> logger, IConfiguration config, NvdDeveloperContext context)
		{
			_logger = logger;
			_configuration = config;
			_context = context;
		}

		[HttpPost]
		[Route("GetReports")]
		public async Task<AppReportPageResult> GetReports([FromBody] PageSubmission postOptions)
		{
			ApplicationReportManager appMan = new ApplicationReportManager(_configuration, _context);
			return await appMan.GetReports(postOptions);
		}

		[HttpGet]
		[Route("GetUserReports")]
		public async Task<List<ApplicationReport>?> GetUserReports([FromQuery] string userId)
		{
			ApplicationReportManager appMan = new ApplicationReportManager(_configuration, _context);
			return await appMan.GetReports(userId);
		}

		[HttpGet]
		[Route("GetStatusItems")]
		public async Task<List<ApplicationReportStatus>?> GetStatusItems()
		{
			ApplicationReportManager appMan = new ApplicationReportManager(_configuration, _context);
			return await appMan.GetStatusItems();
		}

		[HttpGet("{reportId}")]
		public async Task<ApplicationReport?> GetAsync(int reportId)
		{
			if (reportId.Equals(0))
			{
				return null;
			}
			ApplicationReportManager appMan = new ApplicationReportManager(_configuration, _context);
			return await appMan.GetReport(reportId);
		}


		[HttpPut]
		public async Task<ApplicationReport?> PutAsync([FromBody] ApplicationReport report)
		{
			ApplicationReportManager appMan = new ApplicationReportManager(_configuration, _context);
			return await appMan.UpdateReport(report);
		}

		[HttpPost]
		public async Task<ApplicationReport?> PostAsync([FromBody] ApplicationReport report)
		{
			ApplicationReportManager appMan = new ApplicationReportManager(_configuration, _context);
			return await appMan.CreateReport(report);
		}

		[HttpDelete("{reportId}")]
		public async Task<bool> DeleteAsync(int reportId)
		{
			ApplicationReportManager appMan = new ApplicationReportManager(_configuration, _context);
			return await appMan.DeleteReport(reportId);
		}

        [HttpPut]
        [Route("AddComment")]
        public async Task<bool> AddCommentAsync([FromBody] ApplicationReportComment item)
        {
            ApplicationReportManager appMan = new ApplicationReportManager(_configuration, _context);
            return await appMan.AddComment(item);
        }
    }
}