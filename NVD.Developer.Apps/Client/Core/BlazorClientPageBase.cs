using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace NVD.Developer.Apps.Client.Core
{
    public class BlazorClientPageBase : ComponentBase
    {
        [Inject]
        protected AuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        public async Task<string?> GetUserId()
        {
            if (AuthenticationStateProvider != null)
            {
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;
                if (user.Identity is not null && user.Identity.IsAuthenticated)
                {
                    return user.FindFirst(c => c.Type == "oid")?.Value;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
