using Microsoft.AspNetCore.Authorization;

namespace projectSales.Server.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionRequirement(string moduleKey, string actionKey)
    {
        ModuleKey = moduleKey;
        ActionKey = actionKey;
    }

    public string ModuleKey { get; }
    public string ActionKey { get; }
}

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string moduleKey, string actionKey)
    {
        Policy = PermissionPolicyBuilder.BuildPolicyName(moduleKey, actionKey);
    }
}

public static class PermissionPolicyBuilder
{
    public static string BuildPolicyName(string moduleKey, string actionKey)
        => $"Permission:{moduleKey}:{actionKey}";
}
