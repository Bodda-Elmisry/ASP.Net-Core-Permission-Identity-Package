using Microsoft.AspNetCore.Authorization;

namespace ASPNetCorePermissionIdentity.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(string permission)
        {
            Policy = $"perm:{permission}";
        }
    }
}
