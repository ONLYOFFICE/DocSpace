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

public class DbWebPlugin : BaseEntity
{
    public int Id { get; set; }
    public int TenantId { get; set; }

    public string Name { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
    public string License { get; set; }
    public string Author { get; set; }
    public string HomePage {  get; set; }
    public string PluginName { get; set; }
    public string Scopes { get; set; }
    public string Image { get; set; }

    public Guid CreateBy { get; set; }
    public DateTime CreateOn { get; set; }

    public bool Enabled { get; set; }
    public bool System { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }
}

public static class WebPluginExtension
{
    public static ModelBuilderWrapper AddDbWebPlugins(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<DbWebPlugin>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddDbWebPlugins, Provider.MySql)
            .Add(PgSqlAddDbWebPlugins, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddDbWebPlugins(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbWebPlugin>(entity =>
        {
            entity.HasKey(e => new { e.Id })
                .HasName("PRIMARY");

            entity.ToTable("webplugins")
                .HasCharSet("utf8mb4");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("tenant");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("varchar(255)");

            entity.Property(e => e.Version)
                .HasColumnName("version")
                .HasColumnType("varchar(255)");

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("text");

            entity.Property(e => e.License)
                .HasColumnName("license")
                .HasColumnType("varchar(255)");

            entity.Property(e => e.Author)
                .HasColumnName("author")
                .HasColumnType("varchar(255)");

            entity.Property(e => e.HomePage)
                .HasColumnName("home_page")
                .HasColumnType("varchar(255)");

            entity.Property(e => e.PluginName)
                .IsRequired()
                .HasColumnName("plugin_name")
                .HasColumnType("varchar(255)");

            entity.Property(e => e.Scopes)
                .HasColumnName("scopes")
                .HasColumnType("text");

            entity.Property(e => e.Image)
                .HasColumnName("image")
                .HasColumnType("varchar(255)");

            entity.Property(e => e.CreateBy)
                .IsRequired()
                .HasColumnName("create_by")
                .HasColumnType("char(36)");

            entity.Property(e => e.CreateOn)
                .HasColumnName("create_on")
                .HasColumnType("datetime");

            entity.Property(e => e.Enabled)
                .HasColumnName("enabled")
                .HasColumnType("tinyint(1)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.System)
                .HasColumnName("system")
                .HasColumnType("tinyint(1)")
                .HasDefaultValueSql("'0'");
        });
    }
    public static void PgSqlAddDbWebPlugins(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbWebPlugin>(entity =>
        {
            entity.HasKey(e => new { e.Id })
                .HasName("webplugins_pkey");

            entity.ToTable("webplugins", "onlyoffice");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("tenant_webplugins");

            entity.Property(e => e.TenantId)
                .HasColumnName("tenant_id");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(255);

            entity.Property(e => e.Version)
                .HasColumnName("version")
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .HasColumnName("description");

            entity.Property(e => e.License)
                .HasColumnName("license")
                .HasMaxLength(255);

            entity.Property(e => e.Author)
                .HasColumnName("author")
                .HasMaxLength(255);

            entity.Property(e => e.HomePage)
                .HasColumnName("home_page")
                .HasMaxLength(255);

            entity.Property(e => e.PluginName)
                .IsRequired()
                .HasColumnName("plugin_name")
                .HasMaxLength(255);

            entity.Property(e => e.Scopes)
                .HasColumnName("scopes");

            entity.Property(e => e.Image)
                .HasColumnName("image")
                .HasMaxLength(255);

            entity.Property(e => e.CreateBy)
                .IsRequired()
                .HasColumnName("create_by")
                .HasMaxLength(36)
                .IsFixedLength();

            entity.Property(e => e.CreateOn)
                .HasColumnName("create_on");

            entity.Property(e => e.Enabled)
                .HasColumnName("enabled");

            entity.Property(e => e.System)
                .HasColumnName("system");
        });
    }
}
