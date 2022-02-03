using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

namespace ASC.AuditTrail.Models;

public class LoginEventQuery
{
    public LoginEvent LoginEvents { get; set; }
    public User User { get; set; }
}