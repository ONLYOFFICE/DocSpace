using System;
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
    [Transient]
    [ElasticsearchType(RelationName = Tables.Folder)]
    public class DbFolder : IDbFile, IDbSearch, ISearchItem
    {
        public int Id { get; set; }
        public int ParentId { get; set; }

        [Text(Analyzer = "whitespacecustom")]
        public string Title { get; set; }
        public FolderType FolderType { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public int TenantId { get; set; }
        public int FoldersCount { get; set; }
        public int FilesCount { get; set; }

        [Ignore]
        public string IndexName
        {
            get => Tables.Folder;
        }

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Title };
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

                entity.Ignore(r => r.IndexName);

                entity.HasIndex(e => e.ModifiedOn)
                    .HasDatabaseName("modified_on");

                entity.HasIndex(e => new { e.TenantId, e.ParentId })
                    .HasDatabaseName("parent_id");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateBy)
                    .IsRequired()
                    .HasColumnName("create_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

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
                    .UseCollation("utf8_general_ci");

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
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddDbFolder(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFolder>(entity =>
            {
                entity.ToTable("files_folder", "onlyoffice");

                entity.HasIndex(e => e.ModifiedOn)
                    .HasDatabaseName("modified_on_files_folder");

                entity.HasIndex(e => new { e.TenantId, e.ParentId })
                    .HasDatabaseName("parent_id");

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
