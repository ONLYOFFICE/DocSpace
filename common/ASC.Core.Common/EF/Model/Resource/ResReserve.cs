
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model.Resource
{
    public class ResReserve
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public string Title { get; set; }
        public string CultureTitle { get; set; }
        public string TextValue { get; set; }
        public int Flag { get; set; }
    }
    public static class ResReserveExtension
    {
        public static ModelBuilderWrapper AddResReserve(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddResReserve, Provider.MySql)
                .Add(PgSqlAddResReserve, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddResReserve(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResReserve>(entity =>
            {
                entity.HasKey(e => new { e.FileId, e.Title, e.CultureTitle })
                    .HasName("PRIMARY");

                entity.ToTable("res_reserve");

                entity.HasIndex(e => e.CultureTitle)
                    .HasDatabaseName("resources_FK2");

                entity.HasIndex(e => e.Id)
                    .HasDatabaseName("id")
                    .IsUnique();

                entity.Property(e => e.FileId).HasColumnName("fileid");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("varchar(120)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CultureTitle)
                    .HasColumnName("cultureTitle")
                    .HasColumnType("varchar(20)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Flag).HasColumnName("flag");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TextValue)
                    .HasColumnName("textValue")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddResReserve(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResReserve>(entity =>
            {
                entity.HasKey(e => new { e.FileId, e.Title, e.CultureTitle })
                    .HasName("res_reserve_pkey");

                entity.ToTable("res_reserve", "onlyoffice");

                entity.Property(e => e.FileId).HasColumnName("fileid");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(120);

                entity.Property(e => e.CultureTitle)
                    .HasColumnName("cultureTitle")
                    .HasMaxLength(20);

                entity.Property(e => e.Flag).HasColumnName("flag");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TextValue).HasColumnName("textValue");
            });
        }
    }
}
