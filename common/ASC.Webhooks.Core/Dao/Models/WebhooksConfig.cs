using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ASC.Webhooks.Core.Dao.Models
{
    public partial class WebhooksConfig
    {
        public int ConfigId { get; set; }
        public int TenantId { get; set; }
        public string Uri { get; set; }
        public string SecretKey { get; set; }
    }

    public static class WebhooksConfigExtension
    {
        public static ModelBuilderWrapper AddWebhooksConfig(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddWebhooksConfig, Provider.MySql);
            //.Add(PgSqlAddLoginEvents, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddWebhooksConfig(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebhooksConfig>(entity =>
            {
                entity.HasKey(e => new { e.ConfigId })
                    .HasName("PRIMARY");

                entity.ToTable("webhooks_config");

                entity.Property(e => e.ConfigId)
                   .HasColumnType("int")
                   .HasColumnName("config_id");

                entity.Property(e => e.TenantId)
                    .HasColumnName("tenant_id")
                    .HasColumnType("int unsigned");

                entity.Property(e => e.Uri)
                    .HasMaxLength(50)
                    .HasColumnName("uri")
                    .HasDefaultValueSql("''");

                entity.Property(e => e.SecretKey)
                    .HasMaxLength(50)
                    .HasColumnName("secret_key")
                    .HasDefaultValueSql("''");
            });
        }
    }
}
