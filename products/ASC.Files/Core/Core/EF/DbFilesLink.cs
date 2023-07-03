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

public class DbFilesLink : BaseEntity, IDbFile
{
    public int TenantId { get; set; }
    public string SourceId { get; set; }
    public string LinkedId { get; set; }
    public Guid LinkedFor { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { TenantId, SourceId, LinkedId };
    }
}

public static class DbFilesLinkExtension
{
    public static ModelBuilderWrapper AddDbFilesLink(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<DbFilesLink>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddDbFilesLink, Provider.MySql)
            .Add(PgSqlAddDbFilesLink, Provider.PostgreSql);

        return modelBuilder;
    }
    public static void MySqlAddDbFilesLink(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFilesLink>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.SourceId, e.LinkedId })
                .HasName("PRIMARY");

            entity.ToTable("files_link")
                .HasCharSet("utf8");

            entity.HasIndex(e => new { e.TenantId, e.SourceId, e.LinkedId, e.LinkedFor })
                .HasDatabaseName("linked_for");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.SourceId)
                .HasColumnName("source_id")
                .HasColumnType("varchar(32)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.LinkedId)
                .HasColumnName("linked_id")
                .HasColumnType("varchar(32)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.LinkedFor)
                .HasColumnName("linked_for")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }

    public static void PgSqlAddDbFilesLink(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFilesLink>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.SourceId, e.LinkedId })
                .HasName("files_link_pkey");

            entity.ToTable("files_link", "onlyoffice");

            entity.HasIndex(e => new { e.TenantId, e.SourceId, e.LinkedId, e.LinkedFor })
                .HasDatabaseName("linked_for_files_link");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.LinkedId)
                .HasColumnName("linked_id")
                .HasMaxLength(32);

            entity.Property(e => e.SourceId)
                .HasColumnName("source_id")
                .HasMaxLength(32);

            entity.Property(e => e.LinkedFor)
                .HasColumnName("linked_for")
                .HasMaxLength(38)
                .IsFixedLength()
                .HasDefaultValueSql("NULL::bpchar");
        });
    }
}
