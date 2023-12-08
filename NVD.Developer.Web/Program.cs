using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.IdentityModel.Tokens.Jwt;
using NVD.Developer.Web.Authorization;
using NVD.Developer.Web.Graph;
using NVD.Developer.Web.Services;
using Azure.Identity;
using Microsoft.Identity.Client;

namespace NVD.Developer.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);
			
			builder.Services.AddLogging();

			builder.Services.AddHttpClient();
			builder.Services.AddScoped<GraphApiClient>();
			builder.Services.AddOptions();

			// This is required to be instantiated before the OpenIdConnectOptions starts getting configured.
			// By default, the claims mapping will map claim names in the old format to accommodate older SAML applications.
			// 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role' instead of 'roles'
			// This flag ensures that the ClaimsIdentity claims collection will be built from the claims in the token
			JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

			var scopes = builder.Configuration.GetValue<string>("DownstreamApi:Scopes")?.Split(' ');
			if (scopes == null || scopes.Length == 0)
			{
				scopes = new[] { "user.read", "user.readbasic.all" };
			}

			// Add services to the container.

			builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
				.AddMicrosoftIdentityWebApp(options =>
				{
					builder.Configuration.Bind("AzureAd", options);
					//If App Service authentication is disabled, this event can inject claims into user principal from Graph API. 
					//If App Service authentication is enabled, this event is not fired.
					options.Events.OnTokenValidated = async context =>
					{
						var userAssertion = new UserAssertion(context.SecurityToken.RawData, "urn:ietf:params:oauth:grant-type:jwt-bearer");

						var oboOptions = new OnBehalfOfCredentialOptions
						{
							AuthorityHost = new Uri(builder.Configuration.GetValue<string>("AzureAd:Instance"))
						};
						var onBehalfOfCredential = new OnBehalfOfCredential(builder.Configuration.GetValue<string>("AzureAd:TenantId"),
																			builder.Configuration.GetValue<string>("AzureAd:ClientId"),
																			builder.Configuration.GetValue<string>("AzureAd:ClientSecret"),
																			userAssertion.Assertion, oboOptions);
						var graphClient = new GraphServiceClient(onBehalfOfCredential, scopes);
						// Get user information from Graph
						User? user = await graphClient.Me.GetAsync(requestConfiguration =>
						{
							requestConfiguration.QueryParameters.Select = new string[] { "id", "displayName", "mail", "userPrincipalName", "mobilePhone" };
						});
						if (user != null)
						{
							context.Principal?.AddUserGraphInfo(user);
						}
					};
				})
				.EnableTokenAcquisitionToCallDownstreamApi(scopes)
				.AddMicrosoftGraph(builder.Configuration.GetSection("DownstreamApi"))
				.AddInMemoryTokenCaches();


			builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
			{
				options.TokenValidationParameters.RoleClaimType = "roles";
				options.TokenValidationParameters.NameClaimType = "name";
			});

			// Adding authorization policies that enforce authorization using Azure AD roles.
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy(AuthorizationPolicies.AssignmentToAdminRoleRequired, policy => policy.RequireClaim("roles", ApplicationRole.Administrator));
			});

			builder.Services.AddDistributedMemoryCache();
			builder.Services.AddSession(options => {
				options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("SiteSettings:SessionTimeout"));
				options.Cookie.Name = builder.Configuration.GetValue<string>("SiteSettings:SessionCookieName");
				options.Cookie.HttpOnly = builder.Configuration.GetValue<bool>("SiteSettings:HttpOnly");
				options.Cookie.IsEssential = true;
			});

			builder.Services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => false;
				// requires using Microsoft.AspNetCore.Http;
				options.MinimumSameSitePolicy = SameSiteMode.None;
				// Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite
				options.HandleSameSiteCookieCompatibility();
			});

			builder.Services.AddHttpContextAccessor();

			IMvcBuilder mvcBuilder = builder.Services.AddRazorPages(options =>
			{
				options.Conventions.AuthorizeFolder("/Admin", AuthorizationPolicies.AssignmentToAdminRoleRequired);
			}).AddMicrosoftIdentityUI();

			if (builder.Environment.IsDevelopment())
			{
				mvcBuilder.AddRazorRuntimeCompilation();
			}

			builder.Services.AddControllersWithViews(options =>
			{
				var policy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.Build();
				options.Filters.Add(new AuthorizeFilter(policy));
			}).AddMicrosoftIdentityUI();


			builder.Services.AddHttpClient<ApplicationService>();
			builder.Services.AddHttpClient<ApplicationVersionService>();
			builder.Services.AddHttpClient<MyListService>();
			builder.Services.AddHttpClient<ApplicationRequestService>();

			builder.Services.ConfigureApplicationCookie(options =>
			{
				// Cookie settings
				options.Cookie.HttpOnly = builder.Configuration.GetValue<bool>("SiteSettings:HttpOnly");
				options.ExpireTimeSpan = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("SiteSettings:SessionTimeout"));
				options.Cookie.IsEssential = true;
				
				options.SlidingExpiration = true;
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Docker")
			{
				app.UseDeveloperExceptionPage();
				
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseSession();

			app.UseCookiePolicy();
			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapRazorPages();
			app.MapControllers();

			app.Run();
		}
	}
}