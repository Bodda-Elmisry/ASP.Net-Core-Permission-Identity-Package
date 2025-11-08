namespace ASPNetCorePermissionIdentity.Entities
{
    public class UserPermission<TKey> where TKey : IEquatable<TKey>
    {
        public TKey UserId { get; set; }
        public TKey PermissionId { get; set; }
    }
}
