using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("regions")]
    public class Regions
    {
        [Key]
        public string Region { get; set; }
        public string Provider { get; set; }

        [Column("connection_string")]
        public string ConnectionString { get; set; }
    }
}
