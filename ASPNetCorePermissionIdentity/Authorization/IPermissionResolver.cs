namespace ASPNetCorePermissionIdentity.Authorization
{
    public interface IPermissionResolver<TKey> where TKey : IEquatable<TKey>
    {
        Task<IReadOnlyCollection<string>> GetUserPermissionsAsync(TKey userId, string? tenantId, CancellationToken ct = default);
    }
}
