namespace ASC.AuditTrail.Models;

public class LoginEventQuery
{
    public LoginEvent LoginEvents { get; set; }
    public User User { get; set; }
}