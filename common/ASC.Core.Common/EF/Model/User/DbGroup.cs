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

public class DbGroup : BaseEntity, IMapFrom<Group>
{
    public int TenantId { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? ParentId { get; set; }
    public string Sid { get; set; }
    public bool Removed { get; set; }
    public DateTime LastModified { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }
}

public static class DbGroupExtension
{
    public static ModelBuilderWrapper AddDbGroup(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<DbGroup>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddDbGroup, Provider.MySql)
            .Add(PgSqlAddDbGroup, Provider.PostgreSql);

        return modelBuilder;
    }

    private static void MySqlAddDbGroup(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbGroup>(entity =>
        {
            entity.ToTable("core_group")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified");

            entity.HasIndex(e => new { e.TenantId, e.ParentId })
                .HasDatabaseName("parentid");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CategoryId)
                .HasColumnName("categoryid")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.LastModified)
                .HasColumnName("last_modified")
                .HasColumnType("timestamp");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("varchar(128)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ParentId)
                .HasColumnName("parentid")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Removed)
            .HasColumnName("removed")
            .HasColumnType("tinyint(1)")
            .HasDefaultValueSql("'0'");

            entity.Property(e => e.Sid)
                .HasColumnName("sid")
                .HasColumnType("varchar(512)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TenantId).HasColumnName("tenant");
        });
    }
    private static void PgSqlAddDbGroup(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbGroup>(entity =>
        {
            entity.ToTable("core_group");

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified");

            entity.HasIndex(e => new { e.TenantId, e.ParentId })
                .HasDatabaseName("parentid");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasMaxLength(38);

            entity.Property(e => e.CategoryId)
                .HasColumnName("categoryid")
                .HasMaxLength(38)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.LastModified)
                .HasColumnName("last_modified")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(128);

            entity.Property(e => e.ParentId)
                .HasColumnName("parentid")
                .HasMaxLength(38)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.Removed).HasColumnName("removed");

            entity.Property(e => e.Sid)
                .HasColumnName("sid")
                .HasMaxLength(512)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.TenantId).HasColumnName("tenant");
        });
    }
}
