using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.ElasticSearch;

using Microsoft.EntityFrameworkCore;

using Nest;

using ColumnAttribute = System.ComponentModel.DataAnnotations.Schema.ColumnAttribute;

namespace ASC.Files.Core.EF
{
    public static class Tables
    {
        public const string File = "file";
        public const string Tree = "tree";
        public const string Folder = "folder";
    }

    [Transient]
    [ElasticsearchType(RelationName = Tables.File)]
    [Table("files_file")]
    public class DbFile : BaseEntity, IDbFile, IDbSearch, ISearchItemDocument
    {
        public int Id { get; set; }
        public int Version { get; set; }

        [Column("version_group")]
        public int VersionGroup { get; set; }

        [Column("current_version")]
        public bool CurrentVersion { get; set; }

        [Column("folder_id")]
        public int FolderId { get; set; }

        [Text(Analyzer = "whitespacecustom")]
        public string Title { get; set; }

        [Column("content_length")]
        public long ContentLength { get; set; }

        [Column("file_status")]
        public int FileStatus { get; set; }

        [Column("category")]
        public int Category { get; set; }

        [Column("create_by")]
        public Guid CreateBy { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }

        [Column("modified_by")]
        public Guid ModifiedBy { get; set; }

        [Column("modified_on")]
        public DateTime ModifiedOn { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("converted_type")]
        public string ConvertedType { get; set; }
        public string Comment { get; set; }
        public string Changes { get; set; }
        public bool Encrypted { get; set; }
        public ForcesaveType Forcesave { get; set; }


        [Nested]
        [NotMapped]
        public List<DbFolderTree> Folders { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get => Tables.File;
        }

        [NotMapped]
        public Document Document { get; set; }

        [Ignore]
        public Expression<Func<ISearchItem, object[]>> SearchContentFields
        {
            get => (a) => new[] { Title, Comment, Changes, Document.Attachment.Content };
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
                entity.HasKey(e => new { e.TenantId, e.Id, e.Version })
                    .HasName("PRIMARY");

                entity.ToTable("files_file");

                entity.HasIndex(e => e.FolderId)
                    .HasName("folder_id");

                entity.HasIndex(e => e.Id)
                    .HasName("id");

                entity.HasIndex(e => e.ModifiedOn)
                    .HasName("modified_on");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Version).HasColumnName("version");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.Property(e => e.Changes)
                    .HasColumnName("changes")
                    .HasColumnType("mediumtext")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Comment)
                    .HasColumnName("comment")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ContentLength).HasColumnName("content_length");

                entity.Property(e => e.ConvertedType)
                    .HasColumnName("converted_type")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreateBy)
                    .IsRequired()
                    .HasColumnName("create_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.CurrentVersion).HasColumnName("current_version");

                entity.Property(e => e.Encrypted).HasColumnName("encrypted");

                entity.Property(e => e.FileStatus).HasColumnName("file_status");

                entity.Property(e => e.FolderId).HasColumnName("folder_id");

                entity.Property(e => e.Forcesave).HasColumnName("forcesave");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnName("modified_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modified_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("varchar(400)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.VersionGroup)
                    .HasColumnName("version_group")
                    .HasDefaultValueSql("'1'");
            });

        }
        public static void PgSqlAddDbFiles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFile>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.TenantId, e.Version })
                    .HasName("files_file_pkey");

                entity.ToTable("files_file", "onlyoffice");

                entity.HasIndex(e => e.FolderId)
                    .HasName("folder_id");

                entity.HasIndex(e => e.Id)
                    .HasName("id");

                entity.HasIndex(e => e.ModifiedOn)
                    .HasName("modified_on_files_file");

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
