using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace ASC.Files.Core.EF
{
    [ElasticsearchType(RelationName = Tables.Tree)]
    public class DbFolderTree : BaseEntity
    {
        public int FolderId { get; set; }
        public int ParentId { get; set; }
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
            modelBuilder
                .Add(MySqlAddDbFolderTree, Provider.MySql)
                .Add(PgSqlAddDbFolderTree, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddDbFolderTree(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFolderTree>(entity =>
            {
                entity.HasKey(e => new { e.ParentId, e.FolderId })
                    .HasName("PRIMARY");

                entity.ToTable("files_folder_tree");

                entity.HasIndex(e => e.FolderId)
                    .HasDatabaseName("folder_id");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.FolderId).HasColumnName("folder_id");

                entity.Property(e => e.Level).HasColumnName("level");
            });
        }
        public static void PgSqlAddDbFolderTree(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFolderTree>(entity =>
            {
                entity.HasKey(e => new { e.ParentId, e.FolderId })
                    .HasName("files_folder_tree_pkey");

                entity.ToTable("files_folder_tree", "onlyoffice");

                entity.HasIndex(e => e.FolderId)
                    .HasDatabaseName("folder_id_files_folder_tree");

                entity.Property(e => e.ParentId).HasColumnName("parent_id");

                entity.Property(e => e.FolderId).HasColumnName("folder_id");

                entity.Property(e => e.Level).HasColumnName("level");
            });

        }
    }

}
