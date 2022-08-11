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

public class MobileAppInstall
{
    public string UserEmail { get; set; }
    public int AppType { get; set; }
    public DateTime RegisteredOn { get; set; }
    public DateTime? LastSign { get; set; }
}

public static class MobileAppInstallExtension
{
    public static ModelBuilderWrapper AddMobileAppInstall(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddMobileAppInstall, Provider.MySql)
            .Add(PgSqlAddMobileAppInstall, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddMobileAppInstall(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MobileAppInstall>(entity =>
        {
            entity.HasKey(e => new { e.UserEmail, e.AppType })
                .HasName("PRIMARY");

            entity.ToTable("mobile_app_install")
                .HasCharSet("utf8");

            entity.Property(e => e.UserEmail)
                .HasColumnName("user_email")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.AppType)
                .HasColumnName("app_type");

            entity.Property(e => e.LastSign)
                .HasColumnName("last_sign")
                .IsRequired(false)
                .HasColumnType("datetime")
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.RegisteredOn)
                .HasColumnName("registered_on")
                .HasColumnType("datetime");
        });
    }
    public static void PgSqlAddMobileAppInstall(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MobileAppInstall>(entity =>
        {
            entity.HasKey(e => new { e.UserEmail, e.AppType })
                .HasName("mobile_app_install_pkey");

            entity.ToTable("mobile_app_install", "onlyoffice");

            entity.Property(e => e.UserEmail)
                .HasColumnName("user_email")
                .HasMaxLength(255);

            entity.Property(e => e.AppType).HasColumnName("app_type");

            entity.Property(e => e.LastSign).HasColumnName("last_sign");

            entity.Property(e => e.RegisteredOn).HasColumnName("registered_on");
        });
    }
}
