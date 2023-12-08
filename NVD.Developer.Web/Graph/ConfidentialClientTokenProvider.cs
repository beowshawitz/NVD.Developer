using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions.Authentication;

namespace NVD.Developer.Web.Graph
{
	public class ConfidentialClientTokenProvider : IAccessTokenProvider
	{
		private readonly IConfiguration _config;
		private readonly string[] _scopes;

		public ConfidentialClientTokenProvider(IConfiguration config)
		{
			_config = config;
			_scopes = config.GetSection("Scopes").Get<string[]>();
		}

		public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
		{
			var applicationOptions = new ConfidentialClientApplicationOptions();
			_config.Bind("AzureAd", applicationOptions);
			var app = ConfidentialClientApplicationBuilder.CreateWithApplicationOptions(applicationOptions).Build();
			var result = await app.AcquireTokenForClient(_scopes).ExecuteAsync(cancellationToken);

			return result.AccessToken;
		}

		public AllowedHostsValidator AllowedHostsValidator { get; }
	}
}
