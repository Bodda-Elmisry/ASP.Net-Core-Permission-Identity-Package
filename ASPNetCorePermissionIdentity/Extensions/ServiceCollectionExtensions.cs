using ASPNetCorePermissionIdentity.Authorization;
using ASPNetCorePermissionIdentity.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace ASPNetCorePermissionIdentity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPermissionIdentity<TCtx, TUser, TRole, TKey>(
        this IServiceCollection services,
        Action<PermissionOptions>? configure = null)
        where TCtx : Persistence.PermissionIdentityDbContext<TUser, TRole, TKey>
        where TUser : Microsoft.AspNetCore.Identity.IdentityUser<TKey>
        where TRole : Microsoft.AspNetCore.Identity.IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        {
            if (configure is not null) services.Configure(configure);

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped(typeof(IPermissionResolver<TKey>), typeof(PermissionResolver<TUser, TRole, TKey, TCtx>));

            return services;
        }
    }
}
