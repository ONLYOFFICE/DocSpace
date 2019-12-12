using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("login_events")]
    public class LoginEvents : MessageEvent
    {
        public string Login { get; set; }
    }
}
