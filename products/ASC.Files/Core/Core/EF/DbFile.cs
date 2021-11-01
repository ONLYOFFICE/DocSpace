using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;

using Nest;

namespace ASC.Files.Core.EF
{
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
        public int FolderId { get; set; }

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
        public Thumbnail Thumb { get; set; }


        [Nested]
        public List<DbFolderTree> Folders { get; set; }

        [Ignore]
        public string IndexName
        {
            get => Tables.File;
        }

        public Document Document { get; set; }

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            if (searchSettings.CanSearchByContent(GetType()))
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
                .Add(PgSqlAddDbFiles, Provider.Postgre);
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

                entity.ToTable("files_file");

                entity.HasIndex(e => e.FolderId)
                    .HasDatabaseName("folder_id");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id");

                entity.HasIndex(e => e.ModifiedOn)
                    .HasDatabaseName("modified_on");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Version).HasColumnName("version");

                entity.Property(e => e.Category).HasColumnName("category");

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

                entity.Property(e => e.ContentLength).HasColumnName("content_length");

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

                entity.Property(e => e.CurrentVersion).HasColumnName("current_version");

                entity.Property(e => e.Thumb).HasColumnName("thumb");

                entity.Property(e => e.Encrypted).HasColumnName("encrypted");

                entity.Property(e => e.FileStatus).HasColumnName("file_status");

                entity.Property(e => e.FolderId).HasColumnName("folder_id");

                entity.Property(e => e.Forcesave).HasColumnName("forcesave");

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

                entity.HasIndex(e => e.FolderId)
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

                entity.Property(e => e.Thumb).HasColumnName("thumb");

                entity.Property(e => e.Encrypted).HasColumnName("encrypted");

                entity.Property(e => e.FileStatus).HasColumnName("file_status");

                entity.Property(e => e.FolderId).HasColumnName("folder_id");

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
}
