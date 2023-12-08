using Microsoft.Extensions.Caching.Memory;
using NVD.Developer.Apps.Shared;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace NVD.Developer.Apps.Server.Cache
{
    public class ApplicationManager
    {
        private const string cacheName = "AppData";
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        private Dictionary<string, AppDeployment>? _appData;

        public ApplicationManager(IConfiguration config, IMemoryCache memoryCache)
        {
            _configuration = config;
            _memoryCache = memoryCache;
            _appData = new Dictionary<string, AppDeployment>();
        }

        public async Task<bool> LoadAppDataAsync()
        {
            bool completed = false;
            if (_memoryCache != null)
            {
                try
                {
                    if (!_memoryCache.TryGetValue(cacheName, out _appData))
                    {
                        try
                        {
                            var client = new HttpClient();
                            HttpResponseMessage appDataResponse = await client.GetAsync(_configuration.GetSection("AppResources:PopularApps").Value);
                            if (appDataResponse == null || !appDataResponse.IsSuccessStatusCode || appDataResponse.StatusCode.Equals(HttpStatusCode.NoContent))
                            {
                                throw new HttpRequestException("Unable to retrieve the Http data.");
                            }
							var response = await appDataResponse.Content.ReadAsStringAsync();

							_appData = await appDataResponse.Content.ReadFromJsonAsync<Dictionary<string, AppDeployment>?>();
                            if (_appData == null)
                            {
                                _appData = new Dictionary<string, AppDeployment>();
                            }
                        }
                        catch (Exception ex)
                        {
                            ApplicationException appEx = new ApplicationException("An error occurred on Vulnerability load.", ex);
                            throw appEx;
                        }

                        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(20));

                        _memoryCache.Set(cacheName, _appData, cacheEntryOptions);
                    }
					completed = true;
				}
                catch (Exception ex)
                {
                    ApplicationException appEx = new ApplicationException("An error occurred while retrieving and storing the app data.", ex);
                    throw appEx;
                }
            }
            else
            {
                ApplicationException appEx = new ApplicationException("The cache was not available for storage.");
                throw appEx;
            }
            return completed;
        }

        public IEnumerable<AppDeployment> GetApps(AppPageSubmission postOptions)
        {
            if (string.IsNullOrEmpty(postOptions.SortBy)) 
            {
				postOptions.SortBy = "Name"; 
            }
            var propertyInfo = typeof(AppDeployment).GetProperty(postOptions.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (_appData != null)
            {
                if(propertyInfo != null)
                {
                    if (!string.IsNullOrEmpty(postOptions.Filter))
                    {
                        if (string.IsNullOrEmpty(postOptions.SortDirection) || postOptions.SortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase) || postOptions.SortDirection.Equals("ascending", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return _appData.Values.Where(x=> x.Name != null && x.Name.Contains(postOptions.Filter))
                                .OrderBy(e => propertyInfo.GetValue(e, null))
                                .Skip(postOptions.Start).Take(postOptions.PageSize)
                                .Select(x => x);
                        }
                        else
                        {
                            return _appData.Values.Where(x => x.Name != null && x.Name.Contains(postOptions.Filter))
                                .OrderByDescending(e => propertyInfo.GetValue(e, null))
                                .Skip(postOptions.Start).Take(postOptions.PageSize)
                                .Select(x => x);
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(postOptions.SortDirection) || postOptions.SortDirection.Equals("asc", StringComparison.InvariantCultureIgnoreCase) || postOptions.SortDirection.Equals("ascending", StringComparison.InvariantCultureIgnoreCase))
                        {
                            return _appData.Values.OrderBy(e => propertyInfo.GetValue(e, null)).
                                Skip(postOptions.Start).Take(postOptions.PageSize)
                                .Select(x => x);
                        }
                        else
                        {
                            return _appData.Values.OrderByDescending(e => propertyInfo.GetValue(e, null))
                                .Skip(postOptions.Start).Take(postOptions.PageSize)
                                .Select(x => x);
                        }
                    }                    
                }
                else
                {
                    return new List<AppDeployment>();
                }
            }
            else
            {
                return new List<AppDeployment>();
            }
        }

        public AppDeployment? GetApp(string name)
        {
            if (_appData != null && _appData.ContainsKey(name))
            {
                return _appData[name];
            }
            else
            {
                return null;
            }
        }
    }
}
