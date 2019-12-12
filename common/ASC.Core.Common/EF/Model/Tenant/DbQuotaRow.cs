using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("tenants_quotarow")]
    public class DbQuotaRow
    {
        public int Tenant { get; set; }
        public string Path { get; set; }
        public long Counter { get; set; }
        public string Tag { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }
    }
}
