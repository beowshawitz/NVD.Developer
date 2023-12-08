using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NVD.Developer.Apps.Client;
using NVD.Developer.Apps.Client.Core;

namespace NVD.Developer.Apps.Client
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");
			builder.RootComponents.Add<HeadOutlet>("head::after");

			builder.Services.AddHttpClient("NVD.Developer.Apps.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
				.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

			builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("NVD.Developer.Apps.ServerAPI"));

			builder.Services.AddMsalAuthentication(options =>
			{
				builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
				options.ProviderOptions.DefaultAccessTokenScopes.Add("api://fcea3495-7238-4f22-99ef-8c7eb7f96ad8/API.Access");
				options.ProviderOptions.LoginMode = "redirect";
			});

            builder.Services.AddScoped<ToastService>();

            await builder.Build().RunAsync();
		}
	}
}