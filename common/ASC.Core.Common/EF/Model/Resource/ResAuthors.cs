using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Resource
{
    [Table("res_authors")]
    public class ResAuthors
    {
        [Key]
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool Online { get; set; }
        public DateTime LastVisit { get; set; }
    }
    public static class ResAuthorsExtension
    {
        public static ModelBuilderWrapper AddResAuthors(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddResAuthors, Provider.MySql)
                .Add(PgSqlAddResAuthors, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddResAuthors(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<ResAuthors>(entity =>
            {
                _ = entity.HasKey(e => e.Login)
                    .HasName("PRIMARY");

                _ = entity.ToTable("res_authors");

                _ = entity.Property(e => e.Login)
                    .HasColumnName("login")
                    .HasColumnType("varchar(150)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");

                _ = entity.Property(e => e.LastVisit)
                    .HasColumnName("lastVisit")
                    .HasColumnType("datetime");

                _ = entity.Property(e => e.Online).HasColumnName("online");

                _ = entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddResAuthors(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<ResAuthors>(entity =>
            {
                _ = entity.HasKey(e => e.Login)
                    .HasName("res_authors_pkey");

                _ = entity.ToTable("res_authors", "onlyoffice");

                _ = entity.Property(e => e.Login)
                    .HasColumnName("login")
                    .HasMaxLength(150);

                _ = entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");

                _ = entity.Property(e => e.LastVisit).HasColumnName("lastVisit");

                _ = entity.Property(e => e.Online).HasColumnName("online");

                _ = entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(50);
            });
        }
    }
}
