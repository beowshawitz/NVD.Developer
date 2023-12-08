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
    public class ApplicationRequestService : AzServiceBase
	{
		private readonly string _route;

		public ApplicationRequestService(HttpClient httpClient, IConfiguration configuration) : base(httpClient, configuration)
		{
			_route = configuration["ApplicationRequestService:ApiRoute"];
		}

        public async Task<AppReqPageResult?> GetApplicationRequests(PageSubmission request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                using var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_route}/GetRequests", content);
                response.EnsureSuccessStatusCode();
                using(HttpContent respContent = response.Content)
                {
                    var jsonResp = await respContent.ReadAsStringAsync();
                    return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<AppReqPageResult>(jsonResp);
				}
            }
            catch (Exception ex)
            {
                ApplicationException appEx = new ApplicationException("An error occurred while retrieving the application requests.", ex);
                throw appEx;
            }
        }

		public async Task<List<ApplicationRequest>?> GetUserApplicationRequests(string userId)
		{
			try
			{
				var response = await _httpClient.GetAsync($"{_route}/GetUserRequests?userId={userId}");
				response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<List<ApplicationRequest>>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while retrieving the user's application requests.", ex);
				throw appEx;
			}
		}

		

		public async Task<ApplicationRequest?> GetRequest(int requestId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_route}/{requestId}");
                response.EnsureSuccessStatusCode();
                using (HttpContent respContent = response.Content)
                {
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<ApplicationRequest>(jsonResp);
				}
			}
            catch (Exception ex)
            {
                ApplicationException appEx = new ApplicationException("An error occurred while retrieving the application request.", ex);
                throw appEx;
            }
        }

		public async Task<ApplicationRequest?> SaveUpdateItem(ApplicationRequest item)
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
					return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<ApplicationRequest>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while saving/updating the application request.", ex);
				throw appEx;
			}
		}

		public async Task<bool> DeleteItem(ApplicationRequest item)
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
				ApplicationException appEx = new ApplicationException("An error occurred while deleting the application request.", ex);
				throw appEx;
			}
		}

		public async Task<List<ApplicationRequestStatus>?> GetStatusItems()
		{
			try
			{
				HttpResponseMessage response = await _httpClient.GetAsync($"{_route}/GetStatusItems");
				response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<List<ApplicationRequestStatus>>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while retrieving the status items.", ex);
				throw appEx;
			}
		}
	}
}
