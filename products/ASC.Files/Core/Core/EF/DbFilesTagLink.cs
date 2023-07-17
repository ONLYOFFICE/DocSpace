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

public class DbFilesTagLink : BaseEntity, IDbFile
{
    public int TenantId { get; set; }
    public int TagId { get; set; }
    public FileEntryType EntryType { get; set; }
    public string EntryId { get; set; }
    public Guid? CreateBy { get; set; }
    public DateTime? CreateOn { get; set; }
    public int Count { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { TenantId, TagId, EntryId, EntryType };
    }
}

public static class DbFilesTagLinkExtension
{
    public static ModelBuilderWrapper AddDbFilesTagLink(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<DbFilesTagLink>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddDbFilesTagLink, Provider.MySql)
            .Add(PgSqlAddDbFilesTagLink, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddDbFilesTagLink(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFilesTagLink>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.TagId, e.EntryId, e.EntryType })
                .HasName("PRIMARY");

            entity.ToTable("files_tag_link")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.CreateOn)
                .HasDatabaseName("create_on");

            entity.HasIndex(e => new { e.TenantId, e.EntryId, e.EntryType })
                .HasDatabaseName("entry_id");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.TagId).HasColumnName("tag_id");

            entity.Property(e => e.EntryId)
                .HasColumnName("entry_id")
                .HasColumnType("varchar(32)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.EntryType).HasColumnName("entry_type");

            entity.Property(e => e.CreateBy)
                .HasColumnName("create_by")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CreateOn)
                .HasColumnName("create_on")
                .HasColumnType("datetime");

            entity.Property(e => e.Count)
                .HasColumnName("tag_count")
                .HasDefaultValueSql("'0'");
        });
    }
    public static void PgSqlAddDbFilesTagLink(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFilesTagLink>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.TagId, e.EntryType, e.EntryId })
                .HasName("files_tag_link_pkey");

            entity.ToTable("files_tag_link", "onlyoffice");

            entity.HasIndex(e => e.CreateOn)
                .HasDatabaseName("create_on_files_tag_link");

            entity.HasIndex(e => new { e.TenantId, e.EntryType, e.EntryId })
                .HasDatabaseName("entry_id");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.TagId).HasColumnName("tag_id");

            entity.Property(e => e.EntryType).HasColumnName("entry_type");

            entity.Property(e => e.EntryId)
                .HasColumnName("entry_id")
                .HasMaxLength(32);

            entity.Property(e => e.CreateBy)
                .HasColumnName("create_by")
                .HasMaxLength(38)
                .IsFixedLength()
                .HasDefaultValueSql("NULL::bpchar");

            entity.Property(e => e.CreateOn).HasColumnName("create_on");

            entity.Property(e => e.Count).HasColumnName("tag_count");
        });
    }
}
