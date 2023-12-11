using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NVD.Developer.Core.Data;
using NVD.Developer.Core.Models;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace NVD.Developer.Core.Managers
{
    public class ApplicationReportManager
	{
		private readonly IConfiguration _configuration;
		private readonly NvdDeveloperContext _context;

		public ApplicationReportManager(IConfiguration config, NvdDeveloperContext context)
		{
			_configuration = config;
			_context = context;
		}

        public async Task<AppReportPageResult> GetReports(PageSubmission postOptions)
        {
			AppReportPageResult result = new AppReportPageResult(postOptions.PageSize, postOptions.Start);
            if (_context != null)
            {
				IQueryable<ApplicationReport> repsIQ = from reports in _context.ApplicationReports
													.Include(x => x.Status)
												   select reports;
				if (!string.IsNullOrEmpty(postOptions.Filter))
				{
					repsIQ = repsIQ.Where(x => (x.UserName != null && x.UserName.ToLower().Contains(postOptions.Filter.ToLower())) || (x.ApplicationName != null && x.ApplicationName.ToLower().Contains(postOptions.Filter.ToLower())));
				}
				result.ItemCount = await repsIQ.CountAsync();
				repsIQ = ApplySort(repsIQ, postOptions.SortBy, postOptions.SortDirection);
				try
				{
					result.Reports = await repsIQ.Skip(postOptions.Start).Take(postOptions.PageSize).ToListAsync();
				}
				catch(Exception ex) 
				{
					ApplicationException appEx = new ApplicationException("Trying to get the page from the queryable list.", ex);
					throw appEx;
				}
			}
            return result;
		}

		public async Task<List<ApplicationReport>?> GetReports(string userId)
		{
			if (_context != null && !string.IsNullOrEmpty(userId))
			{
				IQueryable<ApplicationReport> repsIQ = from Reports in _context.ApplicationReports
													.Include(x => x.Status)
														select Reports;
				repsIQ = repsIQ.Where(x => x.UserId != null && x.UserId.Equals(userId));
				repsIQ = ApplySort(repsIQ, "dateupdated", "desc");
				try
				{
					return await repsIQ.ToListAsync();
				}
				catch (Exception ex)
				{
					ApplicationException appEx = new ApplicationException("Trying to get the results from the queryable list.", ex);
					throw appEx;
				}
			}
			return null;
		}

		public async Task<List<ApplicationReportStatus>?> GetStatusItems()
		{
			if (_context != null)
			{
				return await _context.ApplicationReportStatus.ToListAsync();
			}
			else
			{
				return null;
			}
		}

		private IQueryable<ApplicationReport> ApplySort(IQueryable<ApplicationReport> repsIQ, string sortBy, string sortDirection)
		{
			if(!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortDirection)) 
			{
				if (sortBy.Equals("name", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
				{
					repsIQ = repsIQ.OrderBy(x => x.ApplicationName);
				}
				if (sortBy.Equals("name", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
				{
					repsIQ = repsIQ.OrderByDescending(x => x.ApplicationName);
				}
				if (sortBy.Equals("user", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
				{
					repsIQ = repsIQ.OrderBy(x => x.UserName);
				}
				if (sortBy.Equals("user", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
				{
					repsIQ = repsIQ.OrderByDescending(x => x.UserName);
				}
				if (sortBy.Equals("datecreated", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
				{
					repsIQ = repsIQ.OrderBy(x => x.DateCreated);
				}
				if (sortBy.Equals("datecreated", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
				{
					repsIQ = repsIQ.OrderByDescending(x => x.DateCreated);
				}
				if (sortBy.Equals("dateupdated", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
				{
					repsIQ = repsIQ.OrderBy(x => x.DateUpdated);
				}
				if (sortBy.Equals("datecreadateupdatedted", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
				{
					repsIQ = repsIQ.OrderByDescending(x => x.DateUpdated);
				}
			}
			else
			{
				repsIQ = repsIQ.OrderByDescending(x => x.DateUpdated);
			}
			return repsIQ;
		}


		public async Task<ApplicationReport?> GetReport(int reportId)
        {
			if (_context != null && _context.ApplicationReports.Any(x => x.Id.Equals(reportId)))
			{
				var report = await _context.ApplicationReports
					.Include(x => x.Status)
					.FirstOrDefaultAsync(x => x.Id.Equals(reportId));
				return report;
			}
			else
			{
				return null;
			}
        }

		public async Task<ApplicationReport?> CreateReport(ApplicationReport req)
		{
			if (_context != null)
			{
				_context.ApplicationReports.Add(req);
				await _context.SaveChangesAsync();
				return req;
			}
			else
			{
				return null;
			}
		}

		public async Task<ApplicationReport?> UpdateReport(ApplicationReport req)
		{
			if (_context != null && _context.ApplicationReports.Any(x => x.Id.Equals(req.Id)))
			{
				_context.ApplicationReports.Update(req);
				await _context.SaveChangesAsync();
				return req;
			}
			else
			{
				return null;
			}
		}

		public async Task<bool> DeleteReport(int reportId)
		{
			bool completed = false;
			if (_context != null && _context.ApplicationReports.Any(x => x.Id.Equals(reportId)))
			{
				var rep = await GetReport(reportId);
				if(rep != null)
				{
					_context.ApplicationReports.Remove(rep);
					await _context.SaveChangesAsync();
					completed = true;
				}
			}
			return completed;
		}
	}
}
