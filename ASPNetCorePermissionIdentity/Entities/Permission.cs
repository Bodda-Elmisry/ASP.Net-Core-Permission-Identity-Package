using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNetCorePermissionIdentity.Entities
{
    public class Permission<TKey> : BaseEntity<TKey> where TKey : IEquatable<TKey>
    {
        public string Name { get; set; } = default!;
        public string? DisplayName { get; set; }
        public string? Description { get; set; }
        public TKey? GroupId { get; set; }
        public PermissionGroup<TKey>? Group { get; set; }
    }
}
