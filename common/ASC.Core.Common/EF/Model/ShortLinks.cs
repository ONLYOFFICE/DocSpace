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
public class ShortLink
{
    public ulong Id { get; set; }
    public int TenantId { get; set; }
    public string Short { get; set; }
    public string Link { get; set; }

    public DbTenant Tenant { get; set; }
}

public static class ShortLinksExtension
{
    public static ModelBuilderWrapper AddShortLinks(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<ShortLink>().Navigation(e => e.Tenant).AutoInclude(false);
        modelBuilder
            .Add(MySqlAddShortLinks, Provider.MySql)
            .Add(PgSqlAddShortLinks, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddShortLinks(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortLink>(entity =>
        {
            entity.ToTable("short_links")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Short)
                .IsUnique();

            entity.HasKey(e => e.Id)
                .HasName("PRIMARY");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("tenant_id");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Short)
                .HasColumnName("short")
                .HasColumnType("char(15)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci")
                .IsRequired(false);

            entity.Property(e => e.TenantId)
                .IsRequired()
                .HasColumnName("tenant_id")
                .HasColumnType("int(10)")
                .HasDefaultValue("-1");

            entity.Property(e => e.Link)
                .HasColumnName("link")
                .HasColumnType("text")
                .UseCollation("utf8_bin")
                .IsRequired(false);
        });
    }

    public static void PgSqlAddShortLinks(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortLink>(entity =>
        {
            entity.ToTable("short_links")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasIndex(e => e.Short)
                .IsUnique();

            entity.HasKey(e => e.Id)
                .HasName("PRIMARY");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("tenant_id");

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Short)
                .HasColumnName("short")
                .HasColumnType("char(15)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci")
                .IsRequired(false);

            entity.Property(e => e.Link)
                .HasColumnName("link")
                .HasColumnType("text")
                .UseCollation("utf8_bin")
                .IsRequired(false);

            entity.Property(e => e.TenantId)
                .IsRequired()
                .HasColumnName("tenant_id")
                .HasColumnType("int(10)")
                .HasDefaultValue("-1");
        });
    }
}
