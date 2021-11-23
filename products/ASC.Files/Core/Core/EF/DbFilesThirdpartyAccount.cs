using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    public class DbFilesThirdpartyAccount : BaseEntity, IDbFile, IDbSearch
    {
        public int Id { get; set; }
        public string Provider { get; set; }
        public string Title { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public FolderType FolderType { get; set; }
        public DateTime CreateOn { get; set; }
        public string Url { get; set; }
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
            modelBuilder
                .Add(MySqlAddDbFilesThirdpartyAccount, Provider.MySql)
                .Add(PgSqlAddDbFilesThirdpartyAccount, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesThirdpartyAccount(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesThirdpartyAccount>(entity =>
            {
                entity.ToTable("files_thirdparty_account");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("customer_title")
                    .HasColumnType("varchar(400)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.FolderType).HasColumnName("folder_type");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasColumnName("provider")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddDbFilesThirdpartyAccount(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesThirdpartyAccount>(entity =>
            {
                entity.ToTable("files_thirdparty_account", "onlyoffice");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateOn).HasColumnName("create_on");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("customer_title")
                    .HasMaxLength(400);

                entity.Property(e => e.FolderType).HasColumnName("folder_type");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(100);

                entity.Property(e => e.Provider)
                    .IsRequired()
                    .HasColumnName("provider")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'0'::character varying");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.Token).HasColumnName("token");

                entity.Property(e => e.Url).HasColumnName("url");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasColumnName("user_id")
                    .HasMaxLength(38);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnName("user_name")
                    .HasMaxLength(100);
            });
        }
    }
}
