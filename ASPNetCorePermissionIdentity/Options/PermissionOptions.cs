namespace ASPNetCorePermissionIdentity.Options
{
    public class PermissionOptions
    {
        public string Schema { get; set; } = "sec";
        public bool UseTenantFilter { get; set; } = false;
    }
}
