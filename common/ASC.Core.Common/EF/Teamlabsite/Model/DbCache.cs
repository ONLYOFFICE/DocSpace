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

namespace ASC.Core.Common.EF.Teamlabsite.Model;
public class DbCache
{
    public string TenantAlias { get; set; }
}

public static class DbCacheExtension
{
    public static ModelBuilderWrapper AddDbCache(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddDbCache, Provider.MySql)
            .Add(PgSqlAddDbCache, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddDbCache(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbCache>(entity =>
        {
            entity.ToTable("tenants_cache")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasKey(e => e.TenantAlias)
                .HasName("PRIMARY");

            entity.Property(e => e.TenantAlias)
                .HasColumnName("tenant_alias")
                .HasColumnType("varchar(100)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

        });
    }

    public static void PgSqlAddDbCache(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbCache>(entity =>
        {
            entity.ToTable("tenants_cache")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.HasKey(e => e.TenantAlias)
                .HasName("PRIMARY");

            entity.Property(e => e.TenantAlias)
                .HasColumnName("tenant_alias")
                .HasColumnType("varchar(100)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }
}
