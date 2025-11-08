namespace ASPNetCorePermissionIdentity.Entities
{
    public class RolePermission<TKey>
    {
        public TKey RoleId { get; set; }
        public TKey PermissionId { get; set; }
    }
}
