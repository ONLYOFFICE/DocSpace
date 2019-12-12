using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("audit_events")]
    public class AuditEvent : MessageEvent
    {
        public string Initiator { get; set; }
        public string Target { get; set; }
    }
}
