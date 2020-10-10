using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("files_thirdparty_account")]
    public class DbFilesThirdpartyAccount : BaseEntity, IDbFile, IDbSearch
    {
        public int Id { get; set; }

        public string Provider { get; set; }

        [Column("customer_title")]
        public string Title { get; set; }

        [Column("user_name")]
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Token { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("folder_type")]
        public FolderType FolderType { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }

        public string Url { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { Id };
        }
    };

    public static class DbFilesThirdpartyAccountExtension
    {
        public static ModelBuilderWrapper AddDbFilesThirdpartyAccount(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddDbFilesThirdpartyAccount, Provider.MySql)
                .Add(PgSqlAddDbFilesThirdpartyAccount, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesThirdpartyAccount(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFilesThirdpartyAccount>(entity =>
            {
                _ = entity.ToTable("files_thirdparty_account");

                _ = entity.Property(e => e.Id).HasColumnName("id");

                _ = entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("datetime");

                _ = entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("customer_title")
                    .HasColumnType("varchar(400)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.FolderType).HasColumnName("folder_type");

                _ = entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasColumnName("provider")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddDbFilesThirdpartyAccount(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFilesThirdpartyAccount>(entity =>
            {
                _ = entity.ToTable("files_thirdparty_account", "onlyoffice");

                _ = entity.Property(e => e.Id).HasColumnName("id");

                _ = entity.Property(e => e.CreateOn).HasColumnName("create_on");

                _ = entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("customer_title")
                    .HasMaxLength(400);

                _ = entity.Property(e => e.FolderType).HasColumnName("folder_type");

                _ = entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(100);

                _ = entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasColumnName("provider")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'0'::character varying");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.Token).HasColumnName("token");

                _ = entity.Property(e => e.Url).HasColumnName("url");

                _ = entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasMaxLength(38);

                _ = entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasMaxLength(100);
            });
        }
    }
}
