using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

using Nest;

namespace ASC.Files.Core.EF
{

    [ElasticsearchType(RelationName = Tables.Tree)]
    [Table("files_folder_tree")]
    public class DbFolderTree : BaseEntity
    {
        [Column("folder_id")]
        public int FolderId { get; set; }

        [Column("parent_id")]
        public int ParentId { get; set; }

        [Column("level")]
        public int Level { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { ParentId, FolderId };
        }
    }

    public static class DbFolderTreeExtension
    {
        public static ModelBuilderWrapper AddDbFolderTree(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddDbFolderTree, Provider.MySql)
                .Add(PgSqlAddDbFolderTree, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbFolderTree(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFolderTree>(entity =>
            {
                _ = entity.HasKey(e => new { e.ParentId, e.FolderId })
                    .HasName("PRIMARY");

                _ = entity.ToTable("files_folder_tree");

                _ = entity.HasIndex(e => e.FolderId)
                    .HasName("folder_id");

                _ = entity.Property(e => e.ParentId).HasColumnName("parent_id");

                _ = entity.Property(e => e.FolderId).HasColumnName("folder_id");

                _ = entity.Property(e => e.Level).HasColumnName("level");
            });
        }
        public static void PgSqlAddDbFolderTree(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFolderTree>(entity =>
            {
                _ = entity.HasKey(e => new { e.ParentId, e.FolderId })
                    .HasName("files_folder_tree_pkey");

                _ = entity.ToTable("files_folder_tree", "onlyoffice");

                _ = entity.HasIndex(e => e.FolderId)
                    .HasName("folder_id_files_folder_tree");

                _ = entity.Property(e => e.ParentId).HasColumnName("parent_id");

                _ = entity.Property(e => e.FolderId).HasColumnName("folder_id");

                _ = entity.Property(e => e.Level).HasColumnName("level");
            });

        }
    }

}
