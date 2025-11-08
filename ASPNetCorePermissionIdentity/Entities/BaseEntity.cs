using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASPNetCorePermissionIdentity.Entities
{
    public abstract class BaseEntity <TKey>
    {
        public TKey Id { get; set; } = default!;
        public DateTime CreateDateTime { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDateTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
