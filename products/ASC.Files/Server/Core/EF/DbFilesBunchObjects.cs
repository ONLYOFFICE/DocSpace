using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("files_bunch_objects")]
    public class DbFilesBunchObjects : BaseEntity, IDbFile
    {
        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("right_node")]
        public string RightNode { get; set; }

        [Column("left_node")]
        public string LeftNode { get; set; }

        public override object[] GetKeys() => new object[] { TenantId, RightNode };
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
