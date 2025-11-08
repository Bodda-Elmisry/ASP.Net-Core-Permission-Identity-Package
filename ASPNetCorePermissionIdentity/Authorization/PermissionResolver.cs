using ASPNetCorePermissionIdentity.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASPNetCorePermissionIdentity.Authorization
{
    internal class PermissionResolver<TUser, TRole, TKey, TCtx> : IPermissionResolver<TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TCtx : PermissionIdentityDbContext<TUser, TRole, TKey>
    {
        private readonly TCtx _db;
        public PermissionResolver(TCtx db) => _db = db;

        public async Task<IReadOnlyCollection<string>> GetUserPermissionsAsync(TKey userId, string? tenantId, CancellationToken ct = default)
        {
            var direct = from up in _db.UserPermissions
                         join p in _db.Permissions on up.PermissionId equals p.Id
                         where up.UserId!.Equals(userId)
                         select p.Name;

            var viaRoles = from ur in _db.UserRoles
                           join rp in _db.RolePermissions on ur.RoleId equals rp.RoleId
                           join p in _db.Permissions on rp.PermissionId equals p.Id
                           where ur.UserId!.Equals(userId)
                           select p.Name;

            var names = await direct.Union(viaRoles).Distinct().OrderBy(x => x).ToListAsync(ct);
            return names;
        }
    }
}
