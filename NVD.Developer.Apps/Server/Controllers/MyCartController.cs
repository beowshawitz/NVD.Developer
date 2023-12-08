using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Web.Resource;
using NVD.Developer.Apps.Server.Cache;
using NVD.Developer.Apps.Shared;
using System.Xml.Linq;

namespace NVD.Developer.Apps.Server.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
	public class MyCartController : ControllerBase
	{
        private readonly ILogger<MyCartController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public MyCartController(ILogger<MyCartController> logger, IConfiguration config, IMemoryCache memoryCache)
        {
            _logger = logger;
            _configuration = config;
            _memoryCache = memoryCache;
        }

        [HttpGet("{userId}")]
        public IEnumerable<AppDeployment> Get(string userId)
		{
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            CartManager cartMan = new CartManager(_configuration, _memoryCache);
            return cartMan.GetUsersCart(userId);
        }

        [HttpPut("{userId}")]
        public bool Put(string userId, [FromBody] AppDeployment? app)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }
            CartManager cartMan = new CartManager(_configuration, _memoryCache);
            if (app != null)
            {
                return cartMan.AddToUsersCart(userId, app);
            }
            return false;
        }

        [HttpDelete("{userId}/{appId}")]
        public bool Delete(string userId, string appId)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(appId))
            {
                return false;
            }
            CartManager cartMan = new CartManager(_configuration, _memoryCache);
            return cartMan.RemoveFromUsersCart(userId, appId);
        }
    }
}