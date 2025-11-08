using Microsoft.AspNetCore.Authorization;

namespace ASPNetCorePermissionIdentity.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Name { get; }
        public PermissionRequirement(string name) => Name = name;
    }
}
