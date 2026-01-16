using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string PageName { get; set; }
        public string UserId { get; set; }
        public DateTime ActionDateTime { get; set; }
        public string ActionType { get; set; } // Insert, Update, Delete
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
    }

}
