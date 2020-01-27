using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{

    [Table("files_folder_tree")]
    public class DbFolderTree
    {
        [Column("folder_id")]
        public int FolderId { get; set; }

        [Column("parent_id")]
        public int ParentId { get; set; }

        [Column("level")]
        public int Level { get; set; }
    }

    public static class DbFolderTreeExtension
    {
        public static ModelBuilder AddDbFolderTree(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFolderTree>()
                .HasKey(c => new { c.ParentId, c.FolderId });

            return modelBuilder;
        }
    }
}
