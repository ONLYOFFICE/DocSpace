using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    public class DbButton : BaseEntity
    {
        public string ButtonUrl { get; set; }
        public int TariffId { get; set; }
        public string PartnerId { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { TariffId, PartnerId };
        }
    }

    public static class DbButtonExtension
    {
        public static ModelBuilderWrapper AddDbButton(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbButton, Provider.MySql)
                .Add(PgSqlAddDbButton, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddDbButton(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbButton>(entity =>
            {
                entity.HasKey(e => new { e.TariffId, e.PartnerId })
                    .HasName("PRIMARY");

                entity.ToTable("tenants_buttons");

                entity.Property(e => e.TariffId).HasColumnName("tariff_id");

                entity.Property(e => e.PartnerId)
                    .HasColumnName("partner_id")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ButtonUrl)
                    .IsRequired()
                    .HasColumnName("button_url")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddDbButton(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbButton>(entity =>
            {
                entity.HasKey(e => new { e.TariffId, e.PartnerId })
                    .HasName("tenants_buttons_pkey");

                entity.ToTable("tenants_buttons", "onlyoffice");

                entity.Property(e => e.TariffId).HasColumnName("tariff_id");

                entity.Property(e => e.PartnerId)
                    .HasColumnName("partner_id")
                    .HasMaxLength(50);

                entity.Property(e => e.ButtonUrl)
                    .IsRequired()
                    .HasColumnName("button_url");
            });
        }
    }
}
