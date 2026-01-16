using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Table("adm_Property")]
    public class Property : BaseAuditable
    {
        public string Code { get; set; }
        [Required]
        public string PropertyName { get; set; }
        public string ContactNo { get; set; }
        public bool IsActive { get; set; }
        public string Alias { get; set; } // Non-Mandatory
        public string Address1 { get; set; } // Mandatory
        public string Address2 { get; set; } // Non-Mandatory
        public string Address3 { get; set; } // Non-Mandatory
        public string Country { get; set; } // Mandatory
        public string State { get; set; } // Mandatory
        public string City { get; set; } // Mandatory
        public string Pincode { get; set; } // Mandatory
        public string Email { get; set; } // Mandatory
        public string Website { get; set; } // Mandatory
        public string Phone { get; set; } // Non-Mandatory
        public string GstNo { get; set; } // Mandatory
        public string Latitude { get; set; } // Non-Mandatory
        public string Longitude { get; set; } // Non-Mandatory
        public string PanNo { get; set; } // Mandatory
        public string MsmeNo { get; set; } // Non-Mandatory
        public string PropertyLogo { get; set; } // Mandatory
        public virtual ICollection<Building> Buildings { get; set; }
    }
}
