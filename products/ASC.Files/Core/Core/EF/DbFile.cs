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

using Document = ASC.ElasticSearch.Document;

namespace ASC.Files.Core.EF;

public static class Tables
{
    public const string File = "files_file";
    public const string Tree = "files_folder_tree";
    public const string Folder = "files_folder";
}

[Transient]
[ElasticsearchType(RelationName = Tables.File)]
public class DbFile : BaseEntity, IDbFile, IDbSearch, ISearchItemDocument
{
    public int Id { get; set; }
    public int Version { get; set; }
    public int VersionGroup { get; set; }
    public bool CurrentVersion { get; set; }
    public int ParentId { get; set; }

    [Text(Analyzer = "whitespacecustom")]
    public string Title { get; set; }
    public long ContentLength { get; set; }
    public int FileStatus { get; set; }
    public int Category { get; set; }
    public Guid CreateBy { get; set; }
    public DateTime CreateOn { get; set; }
    public Guid ModifiedBy { get; set; }
    public DateTime ModifiedOn { get; set; }
    public int TenantId { get; set; }
    public string ConvertedType { get; set; }
    public string Comment { get; set; }
    public string Changes { get; set; }
    public bool Encrypted { get; set; }
    public ForcesaveType Forcesave { get; set; }
    public Thumbnail ThumbnailStatus { get; set; }


    [Nested]
    public List<DbFolderTree> Folders { get; set; }

    [Ignore]
    public string IndexName => Tables.File;

    public Document Document { get; set; }

    public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
    {
        if (searchSettings.CanSearchByContentAsync(GetType()).Result)
        {
            return (a) => new[] { Title, Comment, Changes, Document.Attachment.Content };
        }

        return (a) => new[] { Title, Comment, Changes };
    }

    public override object[] GetKeys()
    {
        return new object[] { TenantId, Id, Version };
    }
}

public static class DbFileExtension
{
    public static ModelBuilderWrapper AddDbFiles(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddDbFiles, Provider.MySql)
            .Add(PgSqlAddDbFiles, Provider.PostgreSql);

        return modelBuilder;
    }
    public static void MySqlAddDbFiles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFile>(entity =>
        {
            entity.Ignore(r => r.Folders);
            entity.Ignore(r => r.IndexName);
            entity.Ignore(r => r.Document);

            entity.HasKey(e => new { e.TenantId, e.Id, e.Version })
                .HasName("PRIMARY");

            entity.ToTable("files_file")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.ParentId)
                .HasDatabaseName("folder_id");

            entity.HasIndex(e => e.Id)
                .HasDatabaseName("id");

            entity.HasIndex(e => e.ModifiedOn)
                .HasDatabaseName("modified_on");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Version).HasColumnName("version");

            entity.Property(e => e.Category)
                .HasColumnName("category")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Changes)
                .HasColumnName("changes")
                .HasColumnType("mediumtext")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Comment)
                .HasColumnName("comment")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ContentLength)
                .HasColumnName("content_length")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.ConvertedType)
                .HasColumnName("converted_type")
                .HasColumnType("varchar(10)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CreateBy)
                .IsRequired()
                .HasColumnName("create_by")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CreateOn)
                .HasColumnName("create_on")
                .HasColumnType("datetime");

            entity.Property(e => e.CurrentVersion)
                .HasColumnName("current_version")
                .HasColumnType("tinyint(1)")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.ThumbnailStatus)
                .HasColumnName("thumb")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Encrypted)
                .HasColumnType("tinyint(1)")
                .HasColumnName("encrypted")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.FileStatus)
                .HasColumnName("file_status")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.ParentId)
                .HasColumnName("folder_id")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Forcesave)
                .HasColumnName("forcesave")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.ModifiedBy)
                .IsRequired()
                .HasColumnName("modified_by")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ModifiedOn)
                .HasColumnName("modified_on")
                .HasColumnType("datetime");

            entity.Property(e => e.Title)
                .IsRequired()
                .HasColumnName("title")
                .HasColumnType("varchar(400)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.VersionGroup)
                .HasColumnName("version_group")
                .HasDefaultValueSql("'1'");
        });

    }
    public static void PgSqlAddDbFiles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbFile>(entity =>
        {
            entity.Ignore(r => r.Folders);
            entity.Ignore(r => r.IndexName);
            entity.Ignore(r => r.Document);

            entity.HasKey(e => new { e.Id, e.TenantId, e.Version })
                .HasName("files_file_pkey");

            entity.ToTable("files_file", "onlyoffice");

            entity.HasIndex(e => e.ParentId)
                .HasDatabaseName("folder_id");

            entity.HasIndex(e => e.Id)
                .HasDatabaseName("id");

            entity.HasIndex(e => e.ModifiedOn)
                .HasDatabaseName("modified_on_files_file");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.Version).HasColumnName("version");

            entity.Property(e => e.Category).HasColumnName("category");

            entity.Property(e => e.Changes).HasColumnName("changes");

            entity.Property(e => e.Comment)
                .HasColumnName("comment")
                .HasMaxLength(255)
                .HasDefaultValueSql("NULL::character varying");

            entity.Property(e => e.ContentLength)
                .HasColumnName("content_length")
                .HasDefaultValueSql("'0'::bigint");

            entity.Property(e => e.ConvertedType)
                .HasColumnName("converted_type")
                .HasMaxLength(10)
                .HasDefaultValueSql("NULL::character varying");

            entity.Property(e => e.CreateBy)
                .IsRequired()
                .HasColumnName("create_by")
                .HasMaxLength(38)
                .IsFixedLength();

            entity.Property(e => e.CreateOn).HasColumnName("create_on");

            entity.Property(e => e.CurrentVersion).HasColumnName("current_version");

            entity.Property(e => e.ThumbnailStatus).HasColumnName("thumb");

            entity.Property(e => e.Encrypted).HasColumnName("encrypted");

            entity.Property(e => e.FileStatus).HasColumnName("file_status");

            entity.Property(e => e.ParentId).HasColumnName("folder_id");

            entity.Property(e => e.Forcesave).HasColumnName("forcesave");

            entity.Property(e => e.ModifiedBy)
                .IsRequired()
                .HasColumnName("modified_by")
                .HasMaxLength(38)
                .IsFixedLength();

            entity.Property(e => e.ModifiedOn).HasColumnName("modified_on");

            entity.Property(e => e.Title)
                .IsRequired()
                .HasColumnName("title")
                .HasMaxLength(400);

            entity.Property(e => e.VersionGroup)
                .HasColumnName("version_group")
                .HasDefaultValueSql("1");
        });

    }
}
