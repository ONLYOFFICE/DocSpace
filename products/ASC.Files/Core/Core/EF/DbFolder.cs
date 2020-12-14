using System;
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
    [Transient]
    [ElasticsearchType(RelationName = Tables.Folder)]
    [Table("files_folder")]
    public class DbFolder : IDbFile, IDbSearch, ISearchItem
    {
        public int Id { get; set; }

        [Column("parent_id")]
        public int ParentId { get; set; }

        [Text(Analyzer = "whitespacecustom")]
        public string Title { get; set; }

        [Column("folder_type")]
        public FolderType FolderType { get; set; }

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
        public int FoldersCount { get; set; }
        public int FilesCount { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get => Tables.Folder;
        }

        [Ignore]
        public Expression<Func<ISearchItem, object[]>> SearchContentFields
        {
            get
            {
                return (a) => new[] { Title };
            }
        }
    }
    public static class DbFolderExtension
    {
        public static ModelBuilderWrapper AddDbFolder(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbFolder, Provider.MySql)
                    .Add(PgSqlAddDbFolder, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbFolder(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFolder>(entity =>
            {
                entity.ToTable("files_folder");

                entity.HasIndex(e => e.ModifiedOn)
                    .HasName("modified_on");

                entity.HasIndex(e => new { e.TenantId, e.ParentId })
                    .HasName("parent_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateBy)
                    .IsRequired()
                    .HasColumnName("create_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.FilesCount).HasColumnName("filesCount");

                entity.Property(e => e.FolderType).HasColumnName("folder_type");

                entity.Property(e => e.FoldersCount).HasColumnName("foldersCount");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnName("modified_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("modified_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasColumnType("varchar(400)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddDbFolder(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFolder>(entity =>
            {
                entity.ToTable("files_folder", "onlyoffice");

                entity.HasIndex(e => e.ModifiedOn)
                    .HasName("modified_on_files_folder");

                entity.HasIndex(e => new { e.TenantId, e.ParentId })
                    .HasName("parent_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateBy)
                    .IsRequired()
                    .HasColumnName("create_by")
                    .HasMaxLength(38)
                    .IsFixedLength();

                entity.Property(e => e.CreateOn).HasColumnName("create_on");

                entity.Property(e => e.FilesCount).HasColumnName("filesCount");

                entity.Property(e => e.FolderType).HasColumnName("folder_type");

                entity.Property(e => e.FoldersCount).HasColumnName("foldersCount");

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasColumnName("modified_by")
                    .HasMaxLength(38)
                    .IsFixedLength();

                entity.Property(e => e.ModifiedOn).HasColumnName("modified_on");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnName("title")
                    .HasMaxLength(400);
            });
        }
    }

}
