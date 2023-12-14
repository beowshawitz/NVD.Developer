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
    public class ApplicationRequestManager
	{
		private readonly IConfiguration _configuration;
		private readonly NvdDeveloperContext _context;

		public ApplicationRequestManager(IConfiguration config, NvdDeveloperContext context)
		{
			_configuration = config;
			_context = context;
		}

        public async Task<AppReqPageResult> GetRequests(PageSubmission postOptions)
        {
			AppReqPageResult result = new AppReqPageResult(postOptions.PageSize, postOptions.Start);
            if (_context != null)
            {
				IQueryable<ApplicationRequest> reqsIQ = from requests in _context.ApplicationRequests
													.Include(x => x.Status)
												   select requests;
				if (!string.IsNullOrEmpty(postOptions.Filter))
				{
					reqsIQ = reqsIQ.Where(x => (x.UserName != null && x.UserName.ToLower().Contains(postOptions.Filter.ToLower())) || (x.ApplicationName != null && x.ApplicationName.ToLower().Contains(postOptions.Filter.ToLower())));
				}
				result.ItemCount = await reqsIQ.CountAsync();
				reqsIQ = ApplySort(reqsIQ, postOptions.SortBy, postOptions.SortDirection);
				try
				{
					result.Requests = await reqsIQ.Skip(postOptions.Start).Take(postOptions.PageSize).ToListAsync();
				}
				catch(Exception ex) 
				{
					ApplicationException appEx = new ApplicationException("Trying to get the page from the queryable list.", ex);
					throw appEx;
				}
			}
            return result;
		}

		public async Task<List<ApplicationRequest>?> GetRequests(string userId)
		{
			if (_context != null && !string.IsNullOrEmpty(userId))
			{
				IQueryable<ApplicationRequest> reqsIQ = from requests in _context.ApplicationRequests
													.Include(x => x.Status)
														select requests;
				reqsIQ = reqsIQ.Where(x => x.UserId != null && x.UserId.Equals(userId));
				reqsIQ = ApplySort(reqsIQ, "dateupdated", "desc");
				try
				{
					return await reqsIQ.ToListAsync();
				}
				catch (Exception ex)
				{
					ApplicationException appEx = new ApplicationException("Trying to get the results from the queryable list.", ex);
					throw appEx;
				}
			}
			return null;
		}

		public async Task<List<ApplicationRequestStatus>?> GetStatusItems()
		{
			if (_context != null)
			{
				return await _context.ApplicationRequestStatus.ToListAsync();
			}
			else
			{
				return null;
			}
		}

		private IQueryable<ApplicationRequest> ApplySort(IQueryable<ApplicationRequest> reqsIQ, string sortBy, string sortDirection)
		{
			if(!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortDirection)) 
			{
				if (sortBy.Equals("name", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
				{
					reqsIQ = reqsIQ.OrderBy(x => x.ApplicationName);
				}
				if (sortBy.Equals("name", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
				{
					reqsIQ = reqsIQ.OrderByDescending(x => x.ApplicationName);
				}
				if (sortBy.Equals("user", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
				{
					reqsIQ = reqsIQ.OrderBy(x => x.UserName);
				}
				if (sortBy.Equals("user", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
				{
					reqsIQ = reqsIQ.OrderByDescending(x => x.UserName);
				}
				if (sortBy.Equals("datecreated", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
				{
					reqsIQ = reqsIQ.OrderBy(x => x.DateCreated);
				}
				if (sortBy.Equals("datecreated", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
				{
					reqsIQ = reqsIQ.OrderByDescending(x => x.DateCreated);
				}
				if (sortBy.Equals("dateupdated", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase))
				{
					reqsIQ = reqsIQ.OrderBy(x => x.DateUpdated);
				}
				if (sortBy.Equals("datecreadateupdatedted", StringComparison.InvariantCultureIgnoreCase) && sortDirection.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
				{
					reqsIQ = reqsIQ.OrderByDescending(x => x.DateUpdated);
				}
			}
			else
			{
				reqsIQ = reqsIQ.OrderByDescending(x => x.DateUpdated);
			}
			return reqsIQ;
		}


		public async Task<ApplicationRequest?> GetRequest(int requestId)
        {
			if (_context != null && _context.ApplicationRequests.Any(x => x.Id.Equals(requestId)))
			{
				var request = await _context.ApplicationRequests
					.Include(ar => ar.Status)
                    .FirstOrDefaultAsync(ar => ar.Id.Equals(requestId));
				
				if (request != null)
				{
					request.Comments = await _context.ApplicationRequestComments.Where(x => x.RequestId.Equals(requestId)).OrderByDescending(x => x.DateCreated).ToListAsync();
				}
				return request;
			}
			else
			{
				return null;
			}
        }

		public async Task<ApplicationRequest?> CreateRequest(ApplicationRequest req)
		{
			if (_context != null)
			{
				_context.ApplicationRequests.Add(req);
				await _context.SaveChangesAsync();
				return req;
			}
			else
			{
				return null;
			}
		}

		public async Task<ApplicationRequest?> UpdateRequest(ApplicationRequest req)
		{
			if (_context != null && _context.ApplicationRequests.Any(x => x.Id.Equals(req.Id)))
			{
				_context.ApplicationRequests.Update(req);
				await _context.SaveChangesAsync();
				return req;
			}
			else
			{
				return null;
			}
		}

		public async Task<bool> DeleteRequest(int requestId)
		{
			bool completed = false;
			if (_context != null && _context.ApplicationRequests.Any(x => x.Id.Equals(requestId)))
			{
				var req = await GetRequest(requestId);
				if(req != null)
				{
					_context.ApplicationRequests.Remove(req);
					await _context.SaveChangesAsync();
					completed = true;
				}
			}
			return completed;
		}

        public async Task<bool> AddComment(ApplicationRequestComment item)
        {
            if (_context != null)
            {
                _context.ApplicationRequestComments.Add(item);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
