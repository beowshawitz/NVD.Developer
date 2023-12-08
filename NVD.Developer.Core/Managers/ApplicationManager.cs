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
    public class ApplicationManager
    {
		private readonly IConfiguration _configuration;
		private readonly NvdDeveloperContext _context;

		public ApplicationManager(IConfiguration config, NvdDeveloperContext context)
		{
			_configuration = config;
			_context = context;
		}

        public async Task<AppPageResult> GetApps(PageSubmission postOptions)
        {
			AppPageResult result = new AppPageResult(postOptions.PageSize, postOptions.Start);
            if (_context != null)
            {
				IQueryable<Application> appsIQ = from apps in _context.Applications
													.Include(pm => pm.PackageManagers).ThenInclude(p=>p.PackageManager)
													.Include(v => v.Versions)
												   select apps;
				if (!string.IsNullOrEmpty(postOptions.Filter))
				{
					appsIQ = appsIQ.Where(x => x.DisplayName != null && x.DisplayName.ToLower().Contains(postOptions.Filter.ToLower()));
				}
				result.ItemCount = await appsIQ.CountAsync();
				appsIQ = ApplySort(appsIQ, postOptions.SortBy, postOptions.SortDirection);
				try
				{
					result.Applications = await appsIQ.Skip(postOptions.Start).Take(postOptions.PageSize).ToListAsync();
				}
				catch(Exception ex) 
				{
					ApplicationException appEx = new ApplicationException("Trying to get the page from the queryable list.", ex);
					throw appEx;
				}
			}
            return result;
		}

		private IQueryable<Application> ApplySort(IQueryable<Application> appsIQ, string sortBy, string sortDirection)
		{
			if(!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortDirection)) 
			{ 
				if(sortBy.Equals("name", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase)) 
				{
					appsIQ = appsIQ.OrderBy(x => x.DisplayName);
				}
				if (sortBy.Equals("name", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
				{
					appsIQ = appsIQ.OrderByDescending(x => x.DisplayName);
				}
				if (sortBy.Equals("datecreated", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
				{
					appsIQ = appsIQ.OrderBy(x => x.DateCreated);
				}
				if (sortBy.Equals("datecreated", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
				{
					appsIQ = appsIQ.OrderByDescending(x => x.DateCreated);
				}
				if (sortBy.Equals("dateupdated", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
				{
					appsIQ = appsIQ.OrderBy(x => x.DateUpdated);
				}
				if (sortBy.Equals("datecreadateupdatedted", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
				{
					appsIQ = appsIQ.OrderByDescending(x => x.DateUpdated);
				}
			}
			else
			{
				appsIQ = appsIQ.OrderBy(x => x.Name);
			}
			return appsIQ;
		}


		public async Task<Application?> GetApp(int appId)
        {
			if (_context != null && _context.Applications.Any(x => x.Id.Equals(appId)))
			{
				var app = await _context.Applications
					.Include(x => x.Versions)
					.Include(pm => pm.PackageManagers).ThenInclude(p => p.PackageManager)
					.FirstOrDefaultAsync(x => x.Id.Equals(appId));
				return app;
			}
			else
			{
				return null;
			}
        }

		public bool ApplicationExists(string appName)
		{
			if (_context != null)
			{
				return _context.Applications.Any(x => x.Name.Equals(appName));
			}
			else
			{
				return false;
			}
		}

		public async Task<Application?> CreateApp(Application app)
		{
			if (_context != null && !_context.Applications.Any(x => x.Name.Equals(app.Name)))
			{
				_context.Applications.Add(app);
				await _context.SaveChangesAsync();
				return app;
			}
			else
			{
				return null;
			}
		}

		public async Task<Application?> UpdateApp(Application app)
		{
			if (_context != null && _context.Applications.Any(x => x.Id.Equals(app.Id)))
			{
				_context.Applications.Update(app);
				await _context.SaveChangesAsync();
				return app;
			}
			else
			{
				return null;
			}
		}

		public async Task<bool> DeleteApp(int applicationId)
		{
			bool completed = false;
			if (_context != null && _context.Applications.Any(x => x.Id.Equals(applicationId)))
			{
				var app = await GetApp(applicationId);
				if(app != null)
				{
					if(app.Versions != null && app.Versions.Count > 0)
					{
						app.Versions.RemoveRange(0, app.Versions.Count);
					}
					var appListReferences = _context.ApplicationLists.Where(x => x.ApplicationId.Equals(applicationId));
					if(appListReferences != null && appListReferences.Count() > 0)
					{
						_context.ApplicationLists.RemoveRange(appListReferences);
					}
					_context.Applications.Remove(app);
					await _context.SaveChangesAsync();
					completed = true;
				}
			}
			return completed;
		}
	}
}
