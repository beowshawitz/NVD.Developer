using Microsoft.Extensions.Configuration;
using NVD.Developer.Core.Data;
using NVD.Developer.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace NVD.Developer.Core.Managers
{
    public class ApplicationListManager
    {
        private readonly IConfiguration _configuration;
		private readonly NvdDeveloperContext _context;

        public ApplicationListManager(IConfiguration config, NvdDeveloperContext context)
        {
            _configuration = config;
			_context = context;
        }

        public async Task<List<ApplicationListItem>> GetUserList(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                if (_context != null && _context.ApplicationLists.Any(x => x.UserId.Equals(userId)))
                {
                    return await _context.ApplicationLists.Include(ul => ul.Application).Include(ul=>ul.Version)
                                    .Where(x => x.UserId.Equals(userId)).ToListAsync();
                }
                else
                {
                    return new List<ApplicationListItem>();
                }
            }
            else
            {
				return new List<ApplicationListItem>();
			}
        }

        public async Task<bool> AddToUserList(string userId, ApplicationListItem appItem)
        {
            if (_context != null)
            {
                if(appItem.VersionId == null && !_context.ApplicationLists.Any(x => x.UserId.Equals(appItem.UserId) && x.ApplicationId.Equals(appItem.ApplicationId)))
                {
					_context.ApplicationLists.Add(appItem);
					await _context.SaveChangesAsync();
					return true;
				}
				else if(appItem.VersionId != null && !_context.ApplicationLists.Any(x => x.UserId.Equals(appItem.UserId) && x.ApplicationId.Equals(appItem.ApplicationId) && x.VersionId.Equals(appItem.VersionId)))
                {
					_context.ApplicationLists.Add(appItem);
					await _context.SaveChangesAsync();
					return true;
				}

			}
            return false;
        }

        public async Task<bool> RemoveFromUserList(string userId)
        {
            if (_context != null && _context.ApplicationLists.Any(x => x.UserId.Equals(userId)))
            {
                _context.ApplicationLists.RemoveRange(_context.ApplicationLists.Where(x => x.UserId.Equals(userId)));
				await _context.SaveChangesAsync();
				return true;
			}

            return false;
        }

		public async Task<bool> RemoveFromUserList(string userId, int appId, int? versionId)
		{
			if (_context != null)
			{
				_context.ApplicationLists.RemoveRange(_context.ApplicationLists.Where(x => x.UserId.Equals(userId) && x.ApplicationId.Equals(appId) && x.VersionId.Equals(versionId)));
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}

        public List<string> GenerateScripts(List<ApplicationListItem> items)
        {
            List<string> scripts = new List<string>();
            if (_context != null && items != null)
            {
				foreach (ApplicationListItem item in items)
				{
					scripts.Add(item.GenerateScript());
				}
			}
            return scripts;
        }
	}
}
