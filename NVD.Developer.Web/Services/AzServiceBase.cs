using Microsoft.Extensions.Configuration;
using System.Net.Mime;

namespace NVD.Developer.Web.Services
{
	public class AzServiceBase
	{
		protected readonly HttpClient _httpClient;

		protected AzServiceBase(HttpClient httpClient, IConfiguration configuration)
		{
			_httpClient = httpClient;
			_httpClient.BaseAddress = new Uri(configuration["AzServicesApi:BaseAddress"]);
			_httpClient.DefaultRequestHeaders.Add("Accept", MediaTypeNames.Application.Json);
		}

	}
}
