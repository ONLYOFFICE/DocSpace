using System;
using System.ComponentModel.DataAnnotations.Schema;
using ASC.Common.Security.Authorizing;

namespace ASC.Core.Common.EF
{
    [Table("core_acl")]
    public class Acl
    {
        public int Tenant { get; set; }
        public Guid Subject { get; set; }
        public Guid Action { get; set; }
        public string Object { get; set; }
        public AceType AceType { get; set; }
    }
}
