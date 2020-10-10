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
            _ = modelBuilder
                .Add(MySqlAddDbFilesBunchObjects, Provider.MySql)
                .Add(PgSqlAddDbFilesBunchObjects, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbFilesBunchObjects(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFilesBunchObjects>(entity =>
            {
                _ = entity.HasKey(e => new { e.TenantId, e.RightNode })
                    .HasName("PRIMARY");

                _ = entity.ToTable("files_bunch_objects");

                _ = entity.HasIndex(e => e.LeftNode)
                    .HasName("left_node");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.RightNode)
                    .HasColumnName("right_node")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.LeftNode)
                    .IsRequired()
                    .HasColumnName("left_node")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
    }
        public static void PgSqlAddDbFilesBunchObjects(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbFilesBunchObjects>(entity =>
            {
                _ = entity.HasKey(e => new { e.TenantId, e.RightNode })
                    .HasName("files_bunch_objects_pkey");

                _ = entity.ToTable("files_bunch_objects", "onlyoffice");

                _ = entity.HasIndex(e => e.LeftNode)
                    .HasName("left_node");

                _ = entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                _ = entity.Property(e => e.RightNode)
                    .HasColumnName("right_node")
                    .HasMaxLength(255);

                _ = entity.Property(e => e.LeftNode)
                    .IsRequired()
                    .HasColumnName("left_node")
                    .HasMaxLength(255);
            });
        }
    }
}
