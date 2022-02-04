namespace ASC.AuditTrail.Models;

public class AuditEventQuery
{
    public DbAuditEvent AuditEvent { get; set; }
    public User User { get; set; }
}