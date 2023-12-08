using Microsoft.Extensions.Caching.Memory;
using NVD.Developer.Apps.Shared;
using System.Net;

namespace NVD.Developer.Apps.Server.Cache
{
    public class CartManager
    {
        private const string cacheName = "CartData";
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        private Dictionary<string, List<AppDeployment>>? _cartData;

        public CartManager(IConfiguration config, IMemoryCache memoryCache)
        {
            _configuration = config;
            _memoryCache = memoryCache;
            _cartData = new Dictionary<string, List<AppDeployment>>();
            InitializeCart();
        }

        private void InitializeCart()
        {
            if (_memoryCache != null)
            {
                try
                {
                    if (!_memoryCache.TryGetValue(cacheName, out _cartData))
                    {
                        if (_cartData == null)
                        {
                            _cartData = new Dictionary<string, List<AppDeployment>>();
                        }

                        var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(20));

                        _memoryCache.Set(cacheName, _cartData, cacheEntryOptions);
                    }
                }
                catch (Exception ex)
                {
                    ApplicationException appEx = new ApplicationException("An error occurred while retrieving and storing the cart data.", ex);
                    throw appEx;
                }
            }
            else
            {
                ApplicationException appEx = new ApplicationException("The cache was not available for storage.");
                throw appEx;
            }
        }

        public List<AppDeployment> GetUsersCart(string userId)
        {
            if (_cartData != null && _cartData.ContainsKey(userId))
            {
                return _cartData[userId];
            }
            else
            {
                return new List<AppDeployment>();
            }
        }

        public bool AddToUsersCart(string userId, AppDeployment app)
        {
            if (_cartData != null && _cartData.ContainsKey(userId))
            {
                List<AppDeployment> usersCart = _cartData[userId];
                if(usersCart == null)
                {
					usersCart = new List<AppDeployment>();
					usersCart.Add(app);
					_cartData[userId] = usersCart;
                    return true;
				}
                else if(!usersCart.Contains(app))
                {
					usersCart.Add(app);
					_cartData[userId] = usersCart;
					return true;
				}
				return false;
            }
            else if (_cartData != null && !_cartData.ContainsKey(userId))
            {
                List<AppDeployment> usersCart = new List<AppDeployment>();
                usersCart.Add(app);
                _cartData.Add(userId, usersCart);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveFromUsersCart(string userId, string appId)
        {
            if (_cartData != null && _cartData.ContainsKey(userId))
            {
                List<AppDeployment> usersCart = _cartData[userId];
                if (usersCart != null && usersCart.Exists(x=> !string.IsNullOrEmpty(x.Id) && x.Id.Equals(appId)))
                {
                    usersCart.RemoveAll(x => !string.IsNullOrEmpty(x.Id) && x.Id.Equals(appId));
                    _cartData[userId] = usersCart;
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
