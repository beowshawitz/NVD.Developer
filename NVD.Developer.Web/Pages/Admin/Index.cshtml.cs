using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NVD.Developer.Web.Authorization;

namespace NVD.Developer.Web.Pages.Admin
{
    [Authorize(Policy = AuthorizationPolicies.AssignmentToAdminRoleRequired)]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
