namespace NVD.Developer.Web.Authorization
{
    public static class GraphClaimTypes
    {
        public const string DisplayName = "graph_name";
        public const string Email = "graph_email";
        public const string Mobile = "graph_mobile";
        public const string Photo = "graph_photo";
        public const string Id = "graph_id";
    }

    public static class AzureClaimTypes
    {
        public const string PreferredUsername = "preferred_username";
        public const string Email = "email";
        public const string DisplayName = "name";
    }

    /// <summary>
    /// Contain all the authorization policies available in this application.
    /// </summary>
    public static class AuthorizationPolicies
    {
        public const string AssignmentToAdminRoleRequired = "AssignmentToAdminRoleRequired";
    }

    /// <summary>
    /// Contains a list of all the Azure AD app roles this app may use.
    /// </summary>
    public static class ApplicationRole
    {
        public const string Administrator = "app-role-admin";
    }
}
