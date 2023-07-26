// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

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
    public Guid? OwnerId { get; set; }
    public string PaymentId { get; set; }
    public TenantIndustry Industry { get; set; }
    public DateTime LastModified { get; set; }
    public bool Spam { get; set; }
    public bool Calls { get; set; }

    //        public DbTenantPartner Partner { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Tenant, DbTenant>()
            .ForMember(dest => dest.TrustedDomainsEnabled, opt => opt.MapFrom(dest => dest.TrustedDomainsType))
            .ForMember(dest => dest.TrustedDomainsRaw, opt => opt.MapFrom(dest => dest.GetTrustedDomains()))
            .ForMember(dest => dest.Alias, opt => opt.MapFrom(dest => dest.Alias.ToLowerInvariant()))
            .ForMember(dest => dest.LastModified, opt => opt.MapFrom(dest => DateTime.UtcNow))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(dest => dest.Name ?? ""))
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
                OwnerId = Guid.Parse("66faa6e4-f133-11ea-b126-00ffeec8b4ef"),
                LastModified = new DateTime(2022, 7, 8)
            }
            )
            .HasData(
            new DbTenant
            {
                Id = -1,
                Alias = "settings",
                Name = "Web Office",
                CreationDateTime = new DateTime(2021, 3, 9, 17, 46, 59, 97, DateTimeKind.Utc).AddTicks(4317),
                OwnerId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                LastModified = new DateTime(2022, 7, 8),
                Status = TenantStatus.Suspended
            });

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
            entity.ToTable("tenants_tenants")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified");

            entity.HasIndex(e => e.MappedDomain)
                .HasDatabaseName("mappeddomain");

            entity.HasIndex(e => e.Version)
                .HasDatabaseName("version");

            entity.HasIndex(e => e.Alias)
                .HasDatabaseName("alias")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Alias)
                .IsRequired()
                .HasColumnName("alias")
                .HasColumnType("varchar(100)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Calls)
                .HasColumnName("calls")
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(1)");

            entity.Property(e => e.CreationDateTime)
                .HasColumnName("creationdatetime")
                .HasColumnType("datetime");

            entity.Property(e => e.Industry)
                .HasColumnName("industry")
                .IsRequired()
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Language)
                .IsRequired()
                .HasColumnName("language")
                .HasColumnType("char(10)")
                .HasDefaultValueSql("'en-US'")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.LastModified)
                .HasColumnName("last_modified")
                .HasColumnType("timestamp");

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
                .IsRequired(false)
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.PaymentId)
                .HasColumnName("payment_id")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Spam)
                .HasColumnName("spam")
                .HasDefaultValueSql("'1'")
                .HasColumnType("tinyint(1)");

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasDefaultValueSql("'0'");

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
