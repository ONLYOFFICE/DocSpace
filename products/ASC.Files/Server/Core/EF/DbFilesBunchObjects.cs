using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("files_bunch_objects")]
    public class DbFilesBunchObjects
    {
        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("right_node")]
        public string RightNode { get; set; }

        [Column("left_node")]
        public string LeftNode { get; set; }
    }

    public static class DbFilesBunchObjectsExtension
    {
        public static ModelBuilder AddDbFilesBunchObjects(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesBunchObjects>()
                .HasKey(c => new { c.TenantId, c.RightNode });

            return modelBuilder;
        }
    }
}
