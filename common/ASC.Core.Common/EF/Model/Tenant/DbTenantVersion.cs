
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    public class DbTenantVersion
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
        public int DefaultVersion { get; set; }
        public bool Visible { get; set; }
    }
    public static class DbTenantVersionExtension
    {
        public static ModelBuilderWrapper AddDbTenantVersion(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbTenantVersion, Provider.MySql)
                .Add(PgSqlAddDbTenantVersion, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddDbTenantVersion(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTenantVersion>(entity =>
            {
                entity.ToTable("tenants_version");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.DefaultVersion).HasColumnName("default_version");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("url")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasColumnName("version")
                    .HasColumnType("varchar(64)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Visible).HasColumnName("visible");
            });

        }
        public static void PgSqlAddDbTenantVersion(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTenantVersion>(entity =>
            {
                entity.ToTable("tenants_version", "onlyoffice");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.DefaultVersion).HasColumnName("default_version");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("url")
                    .HasMaxLength(64);

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasColumnName("version")
                    .HasMaxLength(64);

                entity.Property(e => e.Visible).HasColumnName("visible");
            });

        }
    }
}
