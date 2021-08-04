
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbStatus
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int StatusType { get; set; }
        public string Image { get; set; }
        public string ImageType { get; set; }
        public string Color { get; set; }
        public int Order { get; set; }
        public int IsDefault { get; set; }
        public int Available { get; set; }
        public int TenantId { get; set; }
    }


    public static class StatusExtension
    {
        public static ModelBuilderWrapper AddStatus(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddStatus, Provider.MySql)
                .Add(PgSqlAddStatus, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddStatus(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbStatus>(entity =>
            {
                entity.HasKey(e => e.Id)
                       .HasName("PRIMARY");

                entity.ToTable("projects_status");

                entity
                .Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                   .HasColumnName("description")
                   .HasColumnType("varchar(255)")
                   .HasCharSet("utf8")
                   .UseCollation("utf8_general_ci");

                entity
                .Property(e => e.StatusType)
                .HasColumnName("statusType");

                entity.Property(e => e.Image)
                   .HasColumnName("image")
                   .HasColumnType("text")
                   .HasCharSet("utf8")
                   .UseCollation("utf8_general_ci");

                entity.Property(e => e.ImageType)
                   .HasColumnName("imageType")
                   .HasColumnType("varchar(50)")
                   .HasCharSet("utf8")
                   .UseCollation("utf8_general_ci");

                entity.Property(e => e.Color)
                  .HasColumnName("color")
                  .HasColumnType("char(7)")
                  .HasCharSet("utf8")
                  .UseCollation("utf8_general_ci");

                entity.Property(e => e.Color)
                  .HasColumnName("color")
                  .HasColumnType("char(7)")
                  .HasCharSet("utf8")
                  .UseCollation("utf8_general_ci");

                entity
                .Property(e => e.Order)
                .HasColumnName("order");

                entity
                .Property(e => e.IsDefault)
                .HasColumnName("isDefault");

                entity
                .Property(e => e.Available)
                .HasColumnName("available");

                entity
                .Property(e => e.TenantId)
                .HasColumnName("tenant_id");
            });
        }
        public static void PgSqlAddStatus(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
