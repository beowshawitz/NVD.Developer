using Azure.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NVD.Developer.Core;
using NVD.Developer.Core.Models;
using System.Text;
using System.Text.Json.Nodes;

namespace NVD.Developer.Web.Services
{
    public class ApplicationService : AzServiceBase
	{
		private readonly string _route;

		public ApplicationService(HttpClient httpClient, IConfiguration configuration) : base(httpClient, configuration)
		{
			_route = configuration["ApplicationService:ApiRoute"];
		}

        public async Task<AppPageResult> GetApplications(PageSubmission request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                using var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_route}/GetApplications", content);
                response.EnsureSuccessStatusCode();
                using(HttpContent respContent = response.Content)
                {
                    var jsonResp = await respContent.ReadAsStringAsync();
                    return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<AppPageResult>(jsonResp);
				}
            }
            catch (Exception ex)
            {
                ApplicationException appEx = new ApplicationException("An error occurred while retrieving the applications.", ex);
                throw appEx;
            }
        }

        public async Task<Application?> GetApplication(int appId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_route}/{appId}");
                response.EnsureSuccessStatusCode();
                using (HttpContent respContent = response.Content)
                {
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<Application>(jsonResp);
				}
			}
            catch (Exception ex)
            {
                ApplicationException appEx = new ApplicationException("An error occurred while retrieving the application.", ex);
                throw appEx;
            }
        }

        public async Task<bool> AppExists(string appName)
        {
			try
			{
				var response = await _httpClient.GetAsync($"{_route}/ApplicationExists?appName={appName}");
				response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? false : JsonConvert.DeserializeObject<bool>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while verifying the existance of the application.", ex);
				throw appEx;
			}
		}

		public async Task<Application?> SaveUpdateItem(Application item)
		{
			try
			{
				var json = JsonConvert.SerializeObject(item);
				using var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
				HttpResponseMessage response = null;
				if (item.Id.Equals(0))
				{
					response = await _httpClient.PostAsync($"{_route}", content);
				}
				else
				{
					response = await _httpClient.PutAsync($"{_route}", content);
				}
				response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<Application>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while saving/updating the application.", ex);
				throw appEx;
			}
		}

		public async Task<bool> DeleteItem(Application item)
		{
			try
			{
				HttpResponseMessage response = await _httpClient.DeleteAsync($"{_route}/{item.Id}");
				response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? false : JsonConvert.DeserializeObject<bool>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while deleting the application.", ex);
				throw appEx;
			}
		}
	}
}
