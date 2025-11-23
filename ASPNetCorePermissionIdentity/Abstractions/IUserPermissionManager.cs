using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ASPNetCorePermissionIdentity.Abstractions
{
    public interface IUserPermissionManager<TKey> where TKey : IEquatable<TKey>
    {
        Task<IdentityResult> GrantAsync(TKey userId, IEnumerable<string> permissionNames, CancellationToken ct = default);
        Task<IdentityResult> RevokeAsync(TKey userId, IEnumerable<string> permissionNames, CancellationToken ct = default);
        Task<IReadOnlyCollection<string>> ListAsync(TKey userId, CancellationToken ct = default);
        Task<bool> HasAsync(TKey userId, string permissionName, CancellationToken ct = default);
    }
}
