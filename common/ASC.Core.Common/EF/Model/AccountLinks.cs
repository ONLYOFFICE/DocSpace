using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("account_links")]
    public class AccountLinks : BaseEntity
    {
        public string Id { get; set; }
        public string UId { get; set; }
        public string Provider { get; set; }
        public string Profile { get; set; }
        public DateTime Linked { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { Id, UId };
        }
    }

    public static class AccountLinksExtension
    {
        public static ModelBuilderWrapper AddAccountLinks(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddAccountLinks, Provider.MySql)
                .Add(PgSqlAddAccountLinks, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddAccountLinks(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<AccountLinks>(entity =>
            {
                _ = entity.HasKey(e => new { e.Id, e.UId })
                    .HasName("PRIMARY");

                _ = entity.ToTable("account_links");

                _ = entity.HasIndex(e => e.UId)
                    .HasName("uid");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.UId)
                    .HasColumnName("uid")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Linked)
                    .HasColumnName("linked")
                    .HasColumnType("datetime");

                _ = entity.Property(e => e.Profile)
                    .IsRequired()
                    .HasColumnName("profile")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Provider)
                    .HasColumnName("provider")
                    .HasColumnType("char(60)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
    }
        public static void PgSqlAddAccountLinks(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<AccountLinks>(entity =>
            {
                _ = entity.HasKey(e => new { e.Id, e.UId })
                    .HasName("account_links_pkey");

                _ = entity.ToTable("account_links", "onlyoffice");

                _ = entity.HasIndex(e => e.UId)
                    .HasName("uid");

                _ = entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(200);

                _ = entity.Property(e => e.UId)
                    .HasColumnName("uid")
                    .HasMaxLength(200);

                _ = entity.Property(e => e.Linked).HasColumnName("linked");

                _ = entity.Property(e => e.Profile)
                    .IsRequired()
                    .HasColumnName("profile");

                _ = entity.Property(e => e.Provider)
                    .HasColumnName("provider")
                    .HasMaxLength(60)
                    .IsFixedLength()
                    .HasDefaultValueSql("NULL");
            });
        }
    }
}
