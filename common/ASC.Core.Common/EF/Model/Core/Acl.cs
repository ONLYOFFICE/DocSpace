using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Common.Security.Authorizing;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    [Table("core_acl")]
    public class Acl : BaseEntity
    {
        public int Tenant { get; set; }
        public Guid Subject { get; set; }
        public Guid Action { get; set; }
        public string Object { get; set; }
        public AceType AceType { get; set; }

        internal override object[] GetKeys()
        {
            return new object[] { Tenant, Subject, Action, Object };
        }
    }

    public static class AclExtension
    {
        public static ModelBuilder AddAcl(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Acl>()
                .HasKey(c => new { c.Tenant, c.Subject, c.Action, c.Object });

            return modelBuilder;
        }
    }
}
