using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("adm_GroupCodes")]
    public class GroupCode : BaseAuditable
    {
        //public int Id { get; set; }

        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string GroupName { get; set; } = null!;

        public int Priority { get; set; }
        public string? Value { get; set; }

        public bool IsActive { get; set; }
        public virtual ICollection<Unit> Units { get; set; }
    }
}
