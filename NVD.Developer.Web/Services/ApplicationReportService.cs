using Azure.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph.Models;
using Newtonsoft.Json;
using NVD.Developer.Core;
using NVD.Developer.Core.Models;
using System.Text;
using System.Text.Json.Nodes;

namespace NVD.Developer.Web.Services
{
    public class ApplicationReportService : AzServiceBase
	{
		private readonly string _route;

		public ApplicationReportService(HttpClient httpClient, IConfiguration configuration) : base(httpClient, configuration)
		{
			_route = configuration["ApplicationReportService:ApiRoute"];
		}

        public async Task<AppReportPageResult?> GetApplicationReports(PageSubmission request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                using var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_route}/GetReports", content);
                response.EnsureSuccessStatusCode();
                using(HttpContent respContent = response.Content)
                {
                    var jsonResp = await respContent.ReadAsStringAsync();
                    return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<AppReportPageResult>(jsonResp);
				}
            }
            catch (Exception ex)
            {
                ApplicationException appEx = new ApplicationException("An error occurred while retrieving the application reports.", ex);
                throw appEx;
            }
        }

		public async Task<List<ApplicationReport>?> GetUserApplicationReports(string userId)
		{
			try
			{
				var response = await _httpClient.GetAsync($"{_route}/GetUserReports?userId={userId}");
				response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<List<ApplicationReport>>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while retrieving the user's application reports.", ex);
				throw appEx;
			}
		}

		

		public async Task<ApplicationReport?> GetReport(int itemId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_route}/{itemId}");
                response.EnsureSuccessStatusCode();
                using (HttpContent respContent = response.Content)
                {
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<ApplicationReport>(jsonResp);
				}
			}
            catch (Exception ex)
            {
                ApplicationException appEx = new ApplicationException("An error occurred while retrieving the application report.", ex);
                throw appEx;
            }
        }

		public async Task<ApplicationReport?> SaveUpdateItem(ApplicationReport item)
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
					return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<ApplicationReport>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while saving/updating the application report.", ex);
				throw appEx;
			}
		}

		public async Task<bool> DeleteItem(ApplicationReport item)
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
				ApplicationException appEx = new ApplicationException("An error occurred while deleting the application report.", ex);
				throw appEx;
			}
		}

		public async Task<List<ApplicationReportStatus>?> GetStatusItems()
		{
			try
			{
				HttpResponseMessage response = await _httpClient.GetAsync($"{_route}/GetStatusItems");
				response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<List<ApplicationReportStatus>>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while retrieving the status items.", ex);
				throw appEx;
			}
		}

        public async Task<bool> AddComment(ApplicationReportComment item)
        {
            try
            {
                var json = JsonConvert.SerializeObject(item);
                using var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync($"{_route}/AddComment", content);
                response.EnsureSuccessStatusCode();
                using (HttpContent respContent = response.Content)
                {
                    var jsonResp = await respContent.ReadAsStringAsync();
                    return string.IsNullOrEmpty(jsonResp) ? false : JsonConvert.DeserializeObject<bool>(jsonResp);
                }
            }
            catch (Exception ex)
            {
                ApplicationException appEx = new ApplicationException("An error occurred while adding the comment to the application report.", ex);
                throw appEx;
            }
        }
    }
}
