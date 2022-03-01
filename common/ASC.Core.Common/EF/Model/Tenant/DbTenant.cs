namespace ASC.Core.Common.EF.Model;

public class DbTenant : IMapFrom<Tenant>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Alias { get; set; }
    public string MappedDomain { get; set; }
    public int Version { get; set; }
    public DateTime? Version_Changed { get; set; }
    public DateTime VersionChanged
    {
        get => Version_Changed ?? DateTime.MinValue;
        set => Version_Changed = value;
    }
    public string Language { get; set; }
    public string TimeZone { get; set; }
    public string TrustedDomainsRaw { get; set; }
    public TenantTrustedDomainsType TrustedDomainsEnabled { get; set; }
    public TenantStatus Status { get; set; }
    public DateTime? StatusChanged { get; set; }
    //hack for DateTime?

    public DateTime StatusChangedHack
    {
        get => StatusChanged ?? DateTime.MinValue;
        set { StatusChanged = value; }
    }
    public DateTime CreationDateTime { get; set; }
    public Guid OwnerId { get; set; }
    public string PaymentId { get; set; }
    public TenantIndustry? Industry { get; set; }
    public DateTime LastModified { get; set; }
    public bool Spam { get; set; }
    public bool Calls { get; set; }

    //        public DbTenantPartner Partner { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Tenant, DbTenant>()
            .ForMember(dest => dest.TrustedDomainsRaw, opt => opt.MapFrom(dest => dest.GetTrustedDomains()))
            .ForMember(dest => dest.Alias, opt => opt.MapFrom(dest => dest.Alias.ToLowerInvariant()))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(dest => DateTime.UtcNow))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(dest => dest.Name ?? dest.Alias))
            .ForMember(dest => dest.MappedDomain, opt => opt.MapFrom(dest =>
                !string.IsNullOrEmpty(dest.MappedDomain) ? dest.MappedDomain.ToLowerInvariant() : null));
    }
}

public static class DbTenantExtension
{
    public static ModelBuilderWrapper AddDbTenant(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddDbTenant, Provider.MySql)
            .Add(PgSqlAddDbTenant, Provider.PostgreSql)
            .HasData(
            new DbTenant
            {
                Id = 1,
                Alias = "localhost",
                Name = "Web Office",
                CreationDateTime = new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317),
                OwnerId = Guid.Parse("66faa6e4-f133-11ea-b126-00ffeec8b4ef")
            }
            );

        return modelBuilder;
    }

    public static void MySqlAddDbTenant(this ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<DbTenant>()
        //    .HasOne(r => r.Partner)
        //    .WithOne(r => r.Tenant)
        //    .HasPrincipalKey<DbTenant>(r => new { r.Id });

        modelBuilder.Entity<DbTenant>(entity =>
        {
            entity.ToTable("tenants_tenants");

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified");

            entity.HasIndex(e => e.MappedDomain)
                .HasDatabaseName("mappeddomain");

            entity.HasIndex(e => e.Version)
                .HasDatabaseName("version");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Alias)
                .IsRequired()
                .HasColumnName("alias")
                .HasColumnType("varchar(100)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Calls)
                .HasColumnName("calls")
                .HasDefaultValueSql("true");

            entity.Property(e => e.CreationDateTime)
                .HasColumnName("creationdatetime")
                .HasColumnType("datetime");

            entity.Property(e => e.Industry).HasColumnName("industry");

            entity.Property(e => e.Language)
                .IsRequired()
                .HasColumnName("language")
                .HasColumnType("char(10)")
                .HasDefaultValueSql("'en-US'")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.LastModified)
                .HasColumnName("last_modified")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.MappedDomain)
                .HasColumnName("mappeddomain")
                .HasColumnType("varchar(100)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.OwnerId)
                .HasColumnName("owner_id")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.PaymentId)
                .HasColumnName("payment_id")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Spam)
                .HasColumnName("spam")
                .HasDefaultValueSql("true");

            entity.Property(e => e.Status).HasColumnName("status");

            entity.Property(e => e.StatusChanged)
                .HasColumnName("statuschanged")
                .HasColumnType("datetime");

            entity.Property(e => e.TimeZone)
                .HasColumnName("timezone")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TrustedDomainsRaw)
                .HasColumnName("trusteddomains")
                .HasColumnType("varchar(1024)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TrustedDomainsEnabled)
                .HasColumnName("trusteddomainsenabled")
                .HasDefaultValueSql("'1'");

            entity.Property(e => e.Version)
                .HasColumnName("version")
                .HasDefaultValueSql("'2'");

            entity.Property(e => e.Version_Changed)
                .HasColumnName("version_changed")
                .HasColumnType("datetime");

            entity.Ignore(c => c.StatusChangedHack);
            entity.Ignore(c => c.VersionChanged);
        });
    }
    public static void PgSqlAddDbTenant(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbTenant>().Ignore(c => c.StatusChangedHack);
        modelBuilder.Entity<DbTenant>(entity =>
        {
            entity.ToTable("tenants_tenants", "onlyoffice");

            entity.HasIndex(e => e.Alias)
                .HasDatabaseName("alias")
                .IsUnique();

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified_tenants_tenants");

            entity.HasIndex(e => e.MappedDomain)
                .HasDatabaseName("mappeddomain");

            entity.HasIndex(e => e.Version)
                .HasDatabaseName("version");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Alias)
                .IsRequired()
                .HasColumnName("alias")
                .HasMaxLength(100);

            entity.Property(e => e.Calls)
                .HasColumnName("calls")
                .HasDefaultValueSql("true");

            entity.Property(e => e.CreationDateTime).HasColumnName("creationdatetime");

            entity.Property(e => e.Industry).HasColumnName("industry");

            entity.Property(e => e.Language)
                .IsRequired()
                .HasColumnName("language")
                .HasMaxLength(10)
                .IsFixedLength()
                .HasDefaultValueSql("'en-US'");

            entity.Property(e => e.LastModified)
                .HasColumnName("last_modified")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.MappedDomain)
                .HasColumnName("mappeddomain")
                .HasMaxLength(100)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(255);

            entity.Property(e => e.OwnerId)
                .HasColumnName("owner_id")
                .HasMaxLength(38)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.PaymentId)
                .HasColumnName("payment_id")
                .HasMaxLength(38)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Spam)
                .HasColumnName("spam")
                .HasDefaultValueSql("true");

            entity.Property(e => e.Status).HasColumnName("status");

            entity.Property(e => e.StatusChanged).HasColumnName("statuschanged");

            entity.Property(e => e.TimeZone)
                .HasColumnName("timezone")
                .HasMaxLength(50)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.TrustedDomainsRaw)
                .HasColumnName("trusteddomains")
                .HasMaxLength(1024)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.TrustedDomainsEnabled)
                .HasColumnName("trusteddomainsenabled")
                .HasDefaultValueSql("1");

            entity.Property(e => e.Version)
                .HasColumnName("version")
                .HasDefaultValueSql("2");

            entity.Property(e => e.Version_Changed).HasColumnName("version_changed");

            entity.Ignore(c => c.StatusChangedHack);
            entity.Ignore(c => c.VersionChanged);
        });
    }
}
