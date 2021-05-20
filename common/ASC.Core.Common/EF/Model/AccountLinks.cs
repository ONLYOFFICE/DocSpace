using System;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
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
            modelBuilder
                .Add(MySqlAddAccountLinks, Provider.MySql)
                .Add(PgSqlAddAccountLinks, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddAccountLinks(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountLinks>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.UId })
                    .HasName("PRIMARY");

                entity.ToTable("account_links");

                entity.HasIndex(e => e.UId)
                    .HasDatabaseName("uid");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.UId)
                    .HasColumnName("uid")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Linked)
                    .HasColumnName("linked")
                    .HasColumnType("datetime");

                entity.Property(e => e.Profile)
                    .IsRequired()
                    .HasColumnName("profile")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Provider)
                    .HasColumnName("provider")
                    .HasColumnType("char(60)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddAccountLinks(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountLinks>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.UId })
                    .HasName("account_links_pkey");

                entity.ToTable("account_links", "onlyoffice");

                entity.HasIndex(e => e.UId)
                    .HasDatabaseName("uid");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(200);

                entity.Property(e => e.UId)
                    .HasColumnName("uid")
                    .HasMaxLength(200);

                entity.Property(e => e.Linked).HasColumnName("linked");

                entity.Property(e => e.Profile)
                    .IsRequired()
                    .HasColumnName("profile");

                entity.Property(e => e.Provider)
                    .HasColumnName("provider")
                    .HasMaxLength(60)
                    .IsFixedLength()
                    .HasDefaultValueSql("NULL");
            });
        }
    }
}
