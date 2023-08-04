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

namespace ASC.Files.Core.EF;

public class DbFilesTag : IDbFile, IMapFrom<TagInfo>
{
    public int TenantId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public Guid Owner { get; set; }
    public TagType Type { get; set; }

    public DbTenant Tenant { get; set; }
}
public static class DbFilesTagExtension
{
    public static ModelBuilderWrapper AddDbFilesTag(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<DbFilesTag>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddDbFilesTag, Provider.MySql)
            .Add(PgSqlAddDbFilesTag, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddDbFilesTag(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFilesTag>(entity =>
        {
            entity.ToTable("files_tag")
                .HasCharSet("utf8");

            entity.HasIndex(e => new { e.TenantId, e.Owner, e.Name, e.Type })
                .HasDatabaseName("name");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Type)
                .HasColumnName("flag")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Owner)
                .IsRequired()
                .HasColumnName("owner")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
        });
    }
    public static void PgSqlAddDbFilesTag(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFilesTag>(entity =>
        {
            entity.ToTable("files_tag", "onlyoffice");

            entity.HasIndex(e => new { e.TenantId, e.Owner, e.Name, e.Type })
                .HasDatabaseName("name_files_tag");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Type).HasColumnName("flag");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("name")
                .HasMaxLength(255);

            entity.Property(e => e.Owner)
                .IsRequired()
                .HasColumnName("owner")
                .HasMaxLength(38);

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
        });
    }
}
