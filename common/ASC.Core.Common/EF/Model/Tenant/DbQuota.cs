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

namespace ASC.Core.Common.EF;

public class DbQuota : BaseEntity, IMapFrom<TenantQuota>
{
    public int TenantId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Features { get; set; }
    public decimal Price { get; set; }
    public string ProductId { get; set; }
    public bool Visible { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { TenantId };
    }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<TenantQuota, DbQuota>();
    }
}
public static class DbQuotaExtension
{
    public static ModelBuilderWrapper AddDbQuota(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddDbQuota, Provider.MySql)
            .Add(PgSqlAddDbQuota, Provider.PostgreSql);

        modelBuilder
            .HasData(
                new DbQuota
                {
                    TenantId = -1,
                    Name = "trial",
                    Description = null,
                    Features = "trial,audit,ldap,sso,whitelabel,thirdparty,restore,oauth,total_size:107374182400,file_size:100,manager:1",
                    Price = 0,
                    ProductId = null,
                    Visible = false
                },
                new DbQuota
                {
                    TenantId = -2,
                    Name = "admin",
                    Description = null,
                    Features = "audit,ldap,sso,whitelabel,thirdparty,restore,oauth,contentsearch,total_size:107374182400,file_size:1024,manager:1",
                    Price = 30,
                    ProductId = "1002",
                    Visible = true
                },
                new DbQuota
                {
                    TenantId = -3,
                    Name = "startup",
                    Description = null,
                    Features = "free,total_size:2147483648,manager:3,room:12",
                    Price = 0,
                    ProductId = null,
                    Visible = false
                }
                );

        return modelBuilder;
    }

    public static void MySqlAddDbQuota(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbQuota>(entity =>
        {
            entity.HasKey(e => e.TenantId)
                .HasName("PRIMARY");

            entity.ToTable("tenants_quota")
                .HasCharSet("utf8");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant")
                .ValueGeneratedNever();

            entity.Property(e => e.ProductId)
                .HasColumnName("product_id")
                .HasColumnType("varchar(128)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("varchar(128)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Features)
                .HasColumnName("features")
                .HasColumnType("text");

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasColumnType("varchar(128)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Price)
                .HasColumnName("price")
                .HasDefaultValueSql("'0.00'")
                .HasColumnType("decimal(10,2)");

            entity.Property(e => e.Visible)
                .HasColumnName("visible")
                .HasColumnType("tinyint(1)")
                .HasDefaultValueSql("'0'");
        });
    }
    public static void PgSqlAddDbQuota(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbQuota>(entity =>
        {
            entity.HasKey(e => e.TenantId)
                .HasName("tenants_quota_pkey");

            entity.ToTable("tenants_quota", "onlyoffice");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant")
                .ValueGeneratedNever();

            entity.Property(e => e.ProductId)
                .HasColumnName("product_id")
                .HasMaxLength(128)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("character varying");

            entity.Property(e => e.Features).HasColumnName("features");

            entity.Property(e => e.Name)
                .HasColumnName("name")
                .HasColumnType("character varying");

            entity.Property(e => e.Price)
                .HasColumnName("price")
                .HasColumnType("numeric(10,2)")
                .HasDefaultValueSql("0.00");

            entity.Property(e => e.Visible).HasColumnName("visible");
        });
    }
}
