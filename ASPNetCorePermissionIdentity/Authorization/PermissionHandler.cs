using ASPNetCorePermissionIdentity.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ASPNetCorePermissionIdentity.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext ctx, PermissionRequirement req)
        {
            var ok = ctx.User.Claims.Any(c => c.Type == PermissionClaimTypes.Permission &&
                                              string.Equals(c.Value, req.Name, StringComparison.OrdinalIgnoreCase));
            if (ok) ctx.Succeed(req);
            return Task.CompletedTask;
        }
    }
}
