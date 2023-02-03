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

public class DbWebstudioSettings : BaseEntity
{
    public int TenantId { get; set; }
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Data { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { TenantId, Id, UserId };
    }
}

public static class WebstudioSettingsExtension
{
    public static ModelBuilderWrapper AddWebstudioSettings(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<DbWebstudioSettings>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddWebstudioSettings, Provider.MySql)
            .Add(PgSqlAddWebstudioSettings, Provider.PostgreSql)
            .HasData(
            new DbWebstudioSettings
            {
                TenantId = 1,
                Id = Guid.Parse("9a925891-1f92-4ed7-b277-d6f649739f06"),
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                Data = "{\"Completed\":false}"
            });

        return modelBuilder;
    }

    public static void MySqlAddWebstudioSettings(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbWebstudioSettings>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id, e.UserId })
                .HasName("PRIMARY");

            entity.ToTable("webstudio_settings")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.Id)
                .HasDatabaseName("ID");

            entity.Property(e => e.TenantId).HasColumnName("TenantID");

            entity.Property(e => e.Id)
                .HasColumnName("ID")
                .HasColumnType("varchar(64)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.UserId)
                .HasColumnName("UserID")
                .HasColumnType("varchar(64)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Data)
                .IsRequired()
                .HasColumnType("mediumtext")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }
    public static void PgSqlAddWebstudioSettings(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbWebstudioSettings>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Id, e.UserId })
                .HasName("webstudio_settings_pkey");

            entity.ToTable("webstudio_settings", "onlyoffice");

            entity.HasIndex(e => e.Id)
                .HasDatabaseName("ID");

            entity.Property(e => e.TenantId).HasColumnName("TenantID");

            entity.Property(e => e.Id)
                .HasColumnName("ID")
                .HasMaxLength(64);

            entity.Property(e => e.UserId)
                .HasColumnName("UserID")
                .HasMaxLength(64);

            entity.Property(e => e.Data).IsRequired();
        });
    }
}
