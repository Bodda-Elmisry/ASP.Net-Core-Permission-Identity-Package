namespace ASPNetCorePermissionIdentity.Entities
{
    public class RolePermission<TKey> where TKey : IEquatable<TKey>
    {
        public TKey RoleId { get; set; }
        public TKey PermissionId { get; set; }
    }
}
