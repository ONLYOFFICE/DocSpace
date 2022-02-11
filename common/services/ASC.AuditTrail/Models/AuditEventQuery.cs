namespace ASC.AuditTrail.Models;

public class AuditEventQuery
{
    public AuditEvent Event { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}