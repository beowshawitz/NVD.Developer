using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NVD.Developer.Core.Models;
using System.Text;

namespace NVD.Developer.Web.Services
{
    public class MyListService : AzServiceBase
	{
		private readonly string _route;

		public MyListService(HttpClient httpClient, IConfiguration configuration) : base(httpClient, configuration)
		{
			_route = configuration["MyListService:ApiRoute"];
		}

		public async Task<IEnumerable<ApplicationListItem>?> GetList(string userId)
		{
			try
			{
				var response = await _httpClient.GetAsync($"{_route}/{userId}");
				response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? null : JsonConvert.DeserializeObject<IEnumerable<ApplicationListItem>?>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while retrieving the user application list.", ex);
				throw appEx;
			}
		}

		public async Task<bool> AddToList(string userId, int applicationId, int? versionId)
		{
			try
			{
				ApplicationListItem newItem = new ApplicationListItem() { UserId=userId, ApplicationId= applicationId, VersionId=versionId };
				var json = JsonConvert.SerializeObject(newItem);
				using var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync($"{_route}/{userId}", content);
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

		public async Task<bool> RemoveFromList(string userId, int appId, int? versionId)
        {
            try
            {
				string apiRouteInfo = string.Empty;
				if(versionId == null)
				{
					apiRouteInfo = $"{_route}/{userId}/{appId}";
				}
				else
				{
					apiRouteInfo = $"{_route}/{userId}/{appId}/{versionId}";
				}
                var response = await _httpClient.DeleteAsync(apiRouteInfo);
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

		public async Task<bool> RemoveAllFromList(string userId)
		{
			try
			{
				var response = await _httpClient.DeleteAsync($"{_route}/{userId}");
				response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return !string.IsNullOrEmpty(jsonResp) && JsonConvert.DeserializeObject<bool>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while removing the applications from the user application list.", ex);
				throw appEx;
			}
		}

		public async Task<List<string>> GenerateScript(List<ApplicationListItem> items)
		{
			try
			{
				var json = JsonConvert.SerializeObject(items);
				using var content = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync($"{_route}/GenerateScript", content);
				response.EnsureSuccessStatusCode();
				using (HttpContent respContent = response.Content)
				{
					var jsonResp = await respContent.ReadAsStringAsync();
					return string.IsNullOrEmpty(jsonResp) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(jsonResp);
				}
			}
			catch (Exception ex)
			{
				ApplicationException appEx = new ApplicationException("An error occurred while removing the applications from the user application list.", ex);
				throw appEx;
			}
		}
	}
}
