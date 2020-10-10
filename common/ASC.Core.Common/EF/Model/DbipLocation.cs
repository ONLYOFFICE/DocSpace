using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("dbip_location")]
    public class DbipLocation
    {
        public int Id { get; set; }

        [Column("addr_type")]
        public string AddrType { get; set; }

        [Column("ip_start")]
        public string IPStart { get; set; }

        [Column("ip_end")]
        public string IPEnd { get; set; }

        public string Country { get; set; }

        public string StateProv { get; set; }

        public string District { get; set; }

        public string City { get; set; }

        public string ZipCode { get; set; }

        public long Latitude { get; set; }

        public long Longitude { get; set; }

        [Column("geoname_id")]
        public int GeonameId { get; set; }

        [Column("timezone_offset")]
        public double TimezoneOffset { get; set; }

        [Column("timezone_name")]
        public string TimezoneName { get; set; }

        public int Processed { get; set; }
    }
    public static class DbipLocationExtension
    {
        public static ModelBuilderWrapper AddDbipLocation(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddDbipLocation, Provider.MySql)
                .Add(PgSqlAddDbipLocation, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbipLocation(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbipLocation>(entity =>
            {
                _ = entity.ToTable("dbip_location");

                _ = entity.HasIndex(e => e.IPStart)
                    .HasName("ip_start");

                _ = entity.Property(e => e.Id).HasColumnName("id");

                _ = entity.Property(e => e.AddrType)
                    .IsRequired()
                    .HasColumnName("addr_type")
                    .HasColumnType("enum('ipv4','ipv6')")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnName("city")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Country)
                    .IsRequired()
                    .HasColumnName("country")
                    .HasColumnType("varchar(2)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.GeonameId).HasColumnName("geoname_id");

                _ = entity.Property(e => e.IPEnd)
                    .IsRequired()
                    .HasColumnName("ip_end")
                    .HasColumnType("varchar(39)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.IPStart)
                    .IsRequired()
                    .HasColumnName("ip_start")
                    .HasColumnType("varchar(39)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Latitude).HasColumnName("latitude");

                _ = entity.Property(e => e.Longitude).HasColumnName("longitude");

                _ = entity.Property(e => e.Processed)
                    .HasColumnName("processed")
                    .HasDefaultValueSql("'1'");

                _ = entity.Property(e => e.StateProv)
                    .IsRequired()
                    .HasColumnName("stateprov")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.TimezoneName)
                    .HasColumnName("timezone_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.TimezoneOffset).HasColumnName("timezone_offset");

                _ = entity.Property(e => e.ZipCode)
                    .HasColumnName("zipcode")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

        }
        public static void PgSqlAddDbipLocation(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.HasPostgresEnum("onlyoffice", "enum_dbip_location", new[] { "ipv4", "ipv6" });
            _ = modelBuilder.Entity<DbipLocation>(entity =>
            {
                _ = entity.ToTable("dbip_location", "onlyoffice");

                _ = entity.HasIndex(e => e.IPStart)
                    .HasName("ip_start");

                _ = entity.Property(e => e.Id).HasColumnName("id");

                _ = entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnName("city")
                    .HasMaxLength(255);

                _ = entity.Property(e => e.Country)
                    .IsRequired()
                    .HasColumnName("country")
                    .HasMaxLength(2);

                _ = entity.Property(e => e.District)
                    .HasColumnName("district")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.GeonameId).HasColumnName("geoname_id");

                _ = entity.Property(e => e.IPEnd)
                    .IsRequired()
                    .HasColumnName("ip_end")
                    .HasMaxLength(39);

                _ = entity.Property(e => e.IPStart)
                    .IsRequired()
                    .HasColumnName("ip_start")
                    .HasMaxLength(39);

                _ = entity.Property(e => e.Latitude).HasColumnName("latitude");

                _ = entity.Property(e => e.Longitude).HasColumnName("longitude");

                _ = entity.Property(e => e.Processed)
                    .HasColumnName("processed")
                    .HasDefaultValueSql("1");

                _ = entity.Property(e => e.StateProv)
                    .IsRequired()
                    .HasColumnName("stateprov")
                    .HasMaxLength(255);

                _ = entity.Property(e => e.TimezoneName)
                    .HasColumnName("timezone_name")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                _ = entity.Property(e => e.TimezoneOffset).HasColumnName("timezone_offset");

                _ = entity.Property(e => e.ZipCode)
                    .HasColumnName("zipcode")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");
            });
        }
    }
}
