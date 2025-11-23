using ASPNetCorePermissionIdentity.Abstractions;
using ASPNetCorePermissionIdentity.Entities;
using ASPNetCorePermissionIdentity.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASPNetCorePermissionIdentity.Managers
{
    public class PermissionManager<TUser, TRole, TKey, TCtx> : IPermissionManager<TKey>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TCtx : PermissionIdentityDbContext<TUser, TRole, TKey>
    {
        private readonly TCtx _db;
        private readonly IdentityErrorDescriber _errors;
        private readonly ILookupNormalizer? _normalizer;

        public PermissionManager(TCtx db, IdentityErrorDescriber? describer = null, ILookupNormalizer? normalizer = null)
        {
            _db = db;
            _errors = describer ?? new IdentityErrorDescriber();
            _normalizer = normalizer;
        }

        public async Task<IdentityResult> CreateAsync(Permission<TKey> permission, CancellationToken ct = default)
        {
            if (permission == null) throw new ArgumentNullException(nameof(permission));
            if (string.IsNullOrWhiteSpace(permission.Name))
                return Error("InvalidPermissionName", "Permission name is required.");

            var normalizedName = Normalize(permission.Name);
            var exists = await _db.Permissions.AsNoTracking()
                .AnyAsync(x => (x.Name ?? string.Empty).ToUpper() == normalizedName, ct);
            if (exists) return Error(nameof(IdentityErrorDescriber.DuplicateRoleName), $"Permission '{permission.Name}' already exists.");

            permission.Name = permission.Name.Trim();
            _db.Permissions.Add(permission);
            await _db.SaveChangesAsync(ct);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(Permission<TKey> permission, CancellationToken ct = default)
        {
            if (permission == null) throw new ArgumentNullException(nameof(permission));
            if (string.IsNullOrWhiteSpace(permission.Name))
                return Error("InvalidPermissionName", "Permission name is required.");

            var normalizedName = Normalize(permission.Name);
            var duplicate = await _db.Permissions.AsNoTracking()
                .AnyAsync(x => (x.Name ?? string.Empty).ToUpper() == normalizedName && !x.Id!.Equals(permission.Id), ct);
            if (duplicate) return Error(nameof(IdentityErrorDescriber.DuplicateRoleName), $"Permission '{permission.Name}' already exists.");

            permission.Name = permission.Name.Trim();
            _db.Permissions.Update(permission);
            await _db.SaveChangesAsync(ct);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Permission<TKey> permission, CancellationToken ct = default)
        {
            if (permission == null) throw new ArgumentNullException(nameof(permission));
            _db.Permissions.Remove(permission);
            await _db.SaveChangesAsync(ct);
            return IdentityResult.Success;
        }

        public Task<Permission<TKey>?> FindByIdAsync(TKey id, CancellationToken ct = default)
        {
            return _db.Permissions.FirstOrDefaultAsync(x => x.Id!.Equals(id), ct);
        }

        public Task<Permission<TKey>?> FindByNameAsync(string name, CancellationToken ct = default)
        {
            var normalized = Normalize(name);
            return _db.Permissions.FirstOrDefaultAsync(x => (x.Name ?? string.Empty).ToUpper() == normalized, ct);
        }

        public async Task<IReadOnlyCollection<Permission<TKey>>> GetAllAsync(CancellationToken ct = default)
        {
            var list = await _db.Permissions.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
            return list;
        }

        public async Task<IdentityResult> AddToRoleAsync(TKey permissionId, TKey roleId, CancellationToken ct = default)
        {
            var roleExists = await _db.Roles.AsNoTracking().AnyAsync(r => r.Id!.Equals(roleId), ct);
            if (!roleExists) return Error(new IdentityError { Code = "RoleNotFound", Description = "Role not found." });

            var permissionExists = await _db.Permissions.AsNoTracking().AnyAsync(p => p.Id!.Equals(permissionId), ct);
            if (!permissionExists) return Error(_errors.DefaultError());

            var already = await _db.RolePermissions.AsNoTracking()
                .AnyAsync(x => x.RoleId!.Equals(roleId) && x.PermissionId!.Equals(permissionId), ct);
            if (already) return IdentityResult.Success;

            _db.RolePermissions.Add(new RolePermission<TKey> { RoleId = roleId, PermissionId = permissionId });
            await _db.SaveChangesAsync(ct);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(TKey permissionId, TKey roleId, CancellationToken ct = default)
        {
            var existing = await _db.RolePermissions
                .FirstOrDefaultAsync(x => x.RoleId!.Equals(roleId) && x.PermissionId!.Equals(permissionId), ct);

            if (existing == null) return IdentityResult.Success;

            _db.RolePermissions.Remove(existing);
            await _db.SaveChangesAsync(ct);
            return IdentityResult.Success;
        }

        public async Task<IReadOnlyCollection<Permission<TKey>>> GetRolePermissionsAsync(TKey roleId, CancellationToken ct = default)
        {
            var list = await (from rp in _db.RolePermissions
                              join p in _db.Permissions on rp.PermissionId equals p.Id
                              where rp.RoleId!.Equals(roleId)
                              orderby p.Name
                              select p).AsNoTracking().ToListAsync(ct);
            return list;
        }

        public async Task<IdentityResult> AddToUserAsync(TKey permissionId, TKey userId, CancellationToken ct = default)
        {
            var userExists = await _db.Users.AsNoTracking().AnyAsync(u => u.Id!.Equals(userId), ct);
            if (!userExists) return Error(_errors.DefaultError());

            var permissionExists = await _db.Permissions.AsNoTracking().AnyAsync(p => p.Id!.Equals(permissionId), ct);
            if (!permissionExists) return Error(_errors.DefaultError());

            var already = await _db.UserPermissions.AsNoTracking()
                .AnyAsync(x => x.UserId!.Equals(userId) && x.PermissionId!.Equals(permissionId), ct);
            if (already) return IdentityResult.Success;

            _db.UserPermissions.Add(new UserPermission<TKey> { UserId = userId, PermissionId = permissionId });
            await _db.SaveChangesAsync(ct);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveFromUserAsync(TKey permissionId, TKey userId, CancellationToken ct = default)
        {
            var existing = await _db.UserPermissions
                .FirstOrDefaultAsync(x => x.UserId!.Equals(userId) && x.PermissionId!.Equals(permissionId), ct);

            if (existing == null) return IdentityResult.Success;

            _db.UserPermissions.Remove(existing);
            await _db.SaveChangesAsync(ct);
            return IdentityResult.Success;
        }

        public async Task<IReadOnlyCollection<Permission<TKey>>> GetUserPermissionsAsync(TKey userId, CancellationToken ct = default)
        {
            var direct = from up in _db.UserPermissions
                         join p in _db.Permissions on up.PermissionId equals p.Id
                         where up.UserId!.Equals(userId)
                         select p;

            var viaRoles = from ur in _db.UserRoles
                           join rp in _db.RolePermissions on ur.RoleId equals rp.RoleId
                           join p in _db.Permissions on rp.PermissionId equals p.Id
                           where ur.UserId!.Equals(userId)
                           select p;

            var list = await direct.Union(viaRoles).AsNoTracking().Distinct().OrderBy(x => x.Name).ToListAsync(ct);
            return list;
        }

        public async Task<bool> UserHasAsync(TKey userId, string permissionName, CancellationToken ct = default)
        {
            var normalized = Normalize(permissionName);
            var perms = await GetUserPermissionsAsync(userId, ct);
            return perms.Any(p => Normalize(p.Name) == normalized);
        }

        private string Normalize(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;
            var normalized = _normalizer?.NormalizeName(name) ?? name.Trim();
            return normalized.ToUpperInvariant();
        }

        private IdentityResult Error(IdentityError error) => IdentityResult.Failed(error);
        private IdentityResult Error(string code, string description) =>
            IdentityResult.Failed(new IdentityError { Code = code, Description = description });
    }
}
