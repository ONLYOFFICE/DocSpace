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
}
