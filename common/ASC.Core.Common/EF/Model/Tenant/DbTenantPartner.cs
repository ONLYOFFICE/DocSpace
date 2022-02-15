namespace ASC.Core.Common.EF.Model;

public class DbTenantPartner
{
    public int TenantId { get; set; }
    public string PartnerId { get; set; }
    public string AffiliateId { get; set; }
    public string Campaign { get; set; }

    //public DbTenant Tenant { get; set; }
}

public static class DbTenantPartnerExtension
{
    public static ModelBuilderWrapper AddDbTenantPartner(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddDbTenantPartner, Provider.MySql)
            .Add(PgSqlAddDbTenantPartner, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddDbTenantPartner(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbTenantPartner>(entity =>
        {
            entity.HasKey(e => e.TenantId)
                .HasName("PRIMARY");

            entity.ToTable("tenants_partners");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.AffiliateId)
                .HasColumnName("affiliate_id")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Campaign)
                .HasColumnName("campaign")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.PartnerId)
                .HasColumnName("partner_id")
                .HasColumnType("varchar(36)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });

    }
    public static void PgSqlAddDbTenantPartner(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbTenantPartner>(entity =>
        {
            entity.HasKey(e => e.TenantId)
                .HasName("tenants_partners_pkey");

            entity.ToTable("tenants_partners", "onlyoffice");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id")
                .ValueGeneratedNever();

            entity.Property(e => e.AffiliateId)
                .HasColumnName("affiliate_id")
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Campaign)
                .HasColumnName("campaign")
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.PartnerId)
                .HasColumnName("partner_id")
                .HasMaxLength(36)
                .HasDefaultValueSql("NULL");
        });

    }
}
