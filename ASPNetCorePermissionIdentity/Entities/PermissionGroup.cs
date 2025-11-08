using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNetCorePermissionIdentity.Entities
{
    public class PermissionGroup <TKey> : BaseEntity<TKey>
    {
        public string Name { get; set; } = default!;        
        public string? DisplayName { get; set; }            
        public int Order { get; set; }                      
        public ICollection<Permission<TKey>> Permissions { get; set; } = new List<Permission<TKey>>();
    }
}
