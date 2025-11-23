using ASPNetCorePermissionIdentity.Entities;
using Microsoft.AspNetCore.Identity;

namespace ASPNetCorePermissionIdentity.Abstractions
{
    public interface IPermissionManager<TKey> where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> CreateAsync(Permission<TKey> permission, CancellationToken ct = default);
        Task<IdentityResult> UpdateAsync(Permission<TKey> permission, CancellationToken ct = default);
        Task<IdentityResult> DeleteAsync(Permission<TKey> permission, CancellationToken ct = default);

        Task<Permission<TKey>?> FindByIdAsync(TKey id, CancellationToken ct = default);
        Task<Permission<TKey>?> FindByNameAsync(string name, CancellationToken ct = default);
        Task<IReadOnlyCollection<Permission<TKey>>> GetAllAsync(CancellationToken ct = default);

        Task<IdentityResult> AddToRoleAsync(TKey permissionId, TKey roleId, CancellationToken ct = default);
        Task<IdentityResult> RemoveFromRoleAsync(TKey permissionId, TKey roleId, CancellationToken ct = default);
        Task<IReadOnlyCollection<Permission<TKey>>> GetRolePermissionsAsync(TKey roleId, CancellationToken ct = default);

        Task<IdentityResult> AddToUserAsync(TKey permissionId, TKey userId, CancellationToken ct = default);
        Task<IdentityResult> RemoveFromUserAsync(TKey permissionId, TKey userId, CancellationToken ct = default);
        Task<IReadOnlyCollection<Permission<TKey>>> GetUserPermissionsAsync(TKey userId, CancellationToken ct = default);
        Task<bool> UserHasAsync(TKey userId, string permissionName, CancellationToken ct = default);
    }
}
