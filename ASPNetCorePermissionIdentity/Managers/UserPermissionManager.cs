using System;
using System.Collections.Generic;
using System.Linq;
using ASPNetCorePermissionIdentity.Abstractions;
using ASPNetCorePermissionIdentity.Entities;
using Microsoft.AspNetCore.Identity;

namespace ASPNetCorePermissionIdentity.Managers
{
    public class UserPermissionManager<TKey> : IUserPermissionManager<TKey> where TKey : IEquatable<TKey>
    {
        private readonly IPermissionManager<TKey> _permissionManager;
        private readonly IdentityErrorDescriber _errors;

        public UserPermissionManager(IPermissionManager<TKey> permissionManager, IdentityErrorDescriber? errors = null)
        {
            _permissionManager = permissionManager;
            _errors = errors ?? new IdentityErrorDescriber();
        }

        public async Task<IdentityResult> GrantAsync(TKey userId, IEnumerable<string> permissionNames, CancellationToken ct = default)
        {
            foreach (var name in Clean(permissionNames))
            {
                var perm = await _permissionManager.FindByNameAsync(name, ct);
                if (perm == null) return Error(_errors.DefaultError());
                var result = await _permissionManager.AddToUserAsync(perm.Id!, userId, ct);
                if (!result.Succeeded) return result;
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RevokeAsync(TKey userId, IEnumerable<string> permissionNames, CancellationToken ct = default)
        {
            foreach (var name in Clean(permissionNames))
            {
                var perm = await _permissionManager.FindByNameAsync(name, ct);
                if (perm == null) continue;
                var result = await _permissionManager.RemoveFromUserAsync(perm.Id!, userId, ct);
                if (!result.Succeeded) return result;
            }
            return IdentityResult.Success;
        }

        public async Task<IReadOnlyCollection<string>> ListAsync(TKey userId, CancellationToken ct = default)
        {
            var perms = await _permissionManager.GetUserPermissionsAsync(userId, ct);
            return perms.Select(p => p.Name).ToArray();
        }

        public Task<bool> HasAsync(TKey userId, string permissionName, CancellationToken ct = default)
            => _permissionManager.UserHasAsync(userId, permissionName, ct);

        private static IEnumerable<string> Clean(IEnumerable<string> names) =>
            names.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n.Trim());

        private IdentityResult Error(IdentityError error) => IdentityResult.Failed(error);
    }
}
