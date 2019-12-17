using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("tenants_quota")]
    public class DbQuota : BaseEntity
    {
        [Key]
        public int Tenant { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Column("max_file_size")]
        public long MaxFileSize { get; set; }

        [Column("max_total_size")]
        public long MaxTotalSize { get; set; }

        [Column("active_users")]
        public int ActiveUsers { get; set; }
        public string Features { get; set; }
        public decimal Price { get; set; }
        public decimal Price2 { get; set; }

        [Column("avangate_id")]
        public string AvangateId { get; set; }

        public bool Visible { get; set; }

        internal override object[] GetKeys()
        {
            return new object[] { Tenant };
        }
    }
}
