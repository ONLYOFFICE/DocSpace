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
            entity.ToTable("tenants_version")
                .HasCharSet("utf8");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            entity.Property(e => e.DefaultVersion)
                .HasColumnName("default_version")
                .HasDefaultValueSql("'0'");

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

            entity.Property(e => e.Visible)
                .HasColumnName("visible")
                .HasColumnType("tinyint(1)")
                .HasDefaultValueSql("'0'");
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
