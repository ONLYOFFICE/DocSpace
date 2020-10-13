using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

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

        public override object[] GetKeys()
        {
            return new object[] { TenantId, RightNode };
        }
    }

    public static class DbFilesBunchObjectsExtension
    {
        public static ModelBuilderWrapper AddDbFilesBunchObjects(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbFilesBunchObjects, Provider.MySql)
                .Add(PgSqlAddDbFilesBunchObjects, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesBunchObjects(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesBunchObjects>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.RightNode })
                    .HasName("PRIMARY");

                entity.ToTable("files_bunch_objects");

                entity.HasIndex(e => e.LeftNode)
                    .HasName("left_node");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.RightNode)
                    .HasColumnName("right_node")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LeftNode)
                    .IsRequired()
                    .HasColumnName("left_node")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddDbFilesBunchObjects(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesBunchObjects>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.RightNode })
                    .HasName("files_bunch_objects_pkey");

                entity.ToTable("files_bunch_objects", "onlyoffice");

                entity.HasIndex(e => e.LeftNode)
                    .HasName("left_node");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.RightNode)
                    .HasColumnName("right_node")
                    .HasMaxLength(255);

                entity.Property(e => e.LeftNode)
                    .IsRequired()
                    .HasColumnName("left_node")
                    .HasMaxLength(255);
            });
        }
    }
}
