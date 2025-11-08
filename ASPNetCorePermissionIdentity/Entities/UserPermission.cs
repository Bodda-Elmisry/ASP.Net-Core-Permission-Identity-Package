namespace ASPNetCorePermissionIdentity.Entities
{
    public class UserPermission<TKey>
    {
        public TKey UserId { get; set; }
        public TKey PermissionId { get; set; }
    }
}
