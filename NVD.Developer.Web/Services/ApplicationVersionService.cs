using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NVD.Developer.Core.Models;
using System.Text;

namespace NVD.Developer.Web.Services
{
    public class ApplicationVersionService : AzServiceBase
	{
		private readonly string _route;

		public ApplicationVersionService(HttpClient httpClient, IConfiguration configuration) : base(httpClient, configuration)
		{
			_route = configuration["ApplicationVersionService:ApiRoute"];
		}

		public async Task<bool> VersionExists(int appId, string version)
		{
			try
			{
				var response = await _httpClient.GetAsync($"{_route}/VersionExists?appId={appId}&version={version}");
				response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? false : JsonConvert.DeserializeObject<bool>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while verifying the existance of an application version.", ex);
				throw appEx;
			}
		}

		public async Task<bool> AddToList(int appId, ApplicationVersion version)
		{
			try
			{
				var json = JsonConvert.SerializeObject(version);
				using var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync($"{_route}/{appId}", content);
				response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return !string.IsNullOrEmpty(jsonResp) && JsonConvert.DeserializeObject<bool>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while adding the application to the user application list.", ex);
				throw appEx;
			}
		}

        public async Task<bool> RemoveFromList(int appId, int versionId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_route}/{appId}/{versionId}");
                response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return !string.IsNullOrEmpty(jsonResp) && JsonConvert.DeserializeObject<bool>(jsonResp);
				}
			}
            catch (Exception ex)
            {
                ApplicationException appEx = new ApplicationException("An error occurred while removing the application from the user application list.", ex);
                throw appEx;
            }
        }
	}
}
