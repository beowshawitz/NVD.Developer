using Microsoft.Extensions.Configuration;
using NVD.Developer.Core.Data;
using NVD.Developer.Core.Models;

namespace NVD.Developer.Core.Managers
{
    public class ApplicationVersionManager
	{
        private readonly IConfiguration _configuration;
		private readonly NvdDeveloperContext _context;

        public ApplicationVersionManager(IConfiguration config, NvdDeveloperContext context)
        {
            _configuration = config;
			_context = context;
        }

		public bool ApplicationExists(int appId, string version)
		{
			if (_context != null)
			{
				return _context.ApplicationVersions.Any(x => x.ApplicationId.Equals(appId) && x.Name.Equals(version));
			}
			else
			{
				return false;
			}
		}

		public async Task<bool> AddToVersionList(int appId, ApplicationVersion version)
        {
            if (_context != null && !_context.ApplicationVersions.Any(x => x.ApplicationId.Equals(appId) && x.Name.Equals(version.Name)))
            {
				_context.ApplicationVersions.Add(version);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

		public async Task<bool> RemoveFromVersionList(int appId, int versionId)
		{
			if (_context != null && _context.ApplicationVersions.Any(x => x.ApplicationId.Equals(appId) && x.Id.Equals(versionId)))
			{
				_context.ApplicationVersions.RemoveRange(_context.ApplicationVersions.Where(x => x.ApplicationId.Equals(appId) && x.Id.Equals(versionId)));
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}
	}
}
