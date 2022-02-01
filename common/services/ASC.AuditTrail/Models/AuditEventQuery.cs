using ASC.Core.Common.EF;

namespace ASC.AuditTrail.Models
{
    public class AuditEventQuery
    {
        public Core.Common.EF.Model.AuditEvent AuditEvent { get; set; }
        public User User { get; set; }
    }
}
