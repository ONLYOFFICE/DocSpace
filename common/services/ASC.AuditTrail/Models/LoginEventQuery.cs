namespace ASC.AuditTrail.Models;

public class LoginEventQuery
{
    public DbLoginEvent LoginEvents { get; set; }
    public User User { get; set; }
}