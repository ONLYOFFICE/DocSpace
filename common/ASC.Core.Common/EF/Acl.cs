using System;
using System.ComponentModel.DataAnnotations.Schema;
using ASC.Common.Security.Authorizing;
using Microsoft.EntityFrameworkCore;

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

    public static class AclExtension
    {
        public static void AddAcl(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Acl>()
                .HasKey(c => new { c.Tenant, c.Subject, c.Action, c.Object });
        }
    }
}
