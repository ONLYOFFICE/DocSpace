using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Common.Security.Authorizing;
using ASC.Core.Common.EF.Model;
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

        public override object[] GetKeys()
        {
            return new object[] { Tenant, Subject, Action, Object };
        }
    }

    public static class AclExtension
    {
        public static ModelBuilderWrapper AddAcl(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddAcl, Provider.MySql)
                .Add(PgSqlAddAcl, Provider.Postrge);
            return modelBuilder;
        }
        public static void MySqlAddAcl(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Acl>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.Subject, e.Action, e.Object })
                    .HasName("PRIMARY");

                entity.ToTable("core_acl");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Action)
                    .HasColumnName("action")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Object)
                    .HasColumnName("object")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AceType).HasColumnName("acetype");
            });
        }
        public static void PgSqlAddAcl(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Acl>(entity =>
            {
                entity.HasKey(e => new { e.Tenant, e.Subject, e.Action, e.Object })
                    .HasName("core_acl_pkey");

                entity.ToTable("core_acl", "onlyoffice");

                entity.Property(e => e.Tenant).HasColumnName("tenant");

                entity.Property(e => e.Subject)
                    .HasColumnName("subject")
                    .HasMaxLength(38);

                entity.Property(e => e.Action)
                    .HasColumnName("action")
                    .HasMaxLength(38);

                entity.Property(e => e.Object)
                    .HasColumnName("object")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.AceType).HasColumnName("acetype");
            });
        }
    }
}
