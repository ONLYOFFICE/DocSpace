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

public class DbTariff : BaseEntity
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public DateTime Stamp { get; set; }
    public string CustomerId { get; set; }
    public string Comment { get; set; }
    public DateTime CreateOn { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }
}

public static class DbTariffExtension
{
    public static ModelBuilderWrapper AddDbTariff(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<DbTariff>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddDbTariff, Provider.MySql)
            .Add(PgSqlAddDbTariff, Provider.PostgreSql);

        return modelBuilder;
    }
    public static void MySqlAddDbTariff(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbTariff>(entity =>
        {
            entity.ToTable("tenants_tariff")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("tenant");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Comment)
                .HasColumnName("comment")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CreateOn)
                .HasColumnName("create_on")
                .HasColumnType("timestamp");

            entity.Property(e => e.Stamp)
                .HasColumnName("stamp")
                .HasColumnType("datetime");

            entity.Property(e => e.CustomerId)
                .IsRequired()
                .HasColumnName("customer_id")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TenantId).HasColumnName("tenant");
        });
    }
    public static void PgSqlAddDbTariff(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbTariff>(entity =>
        {
            entity.ToTable("tenants_tariff", "onlyoffice");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("tenant_tenants_tariff");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Comment)
                .HasColumnName("comment")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.CreateOn)
                .HasColumnName("create_on")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.Stamp).HasColumnName("stamp");

            entity.Property(e => e.CustomerId)
                .IsRequired()
                .HasColumnName("customer_id")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.TenantId).HasColumnName("tenant");
        });
    }
}
