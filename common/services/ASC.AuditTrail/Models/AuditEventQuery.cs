using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

namespace ASC.AuditTrail.Models;

public class AuditEventQuery
{
    public AuditEvent AuditEvent { get; set; }
    public User User { get; set; }
}
