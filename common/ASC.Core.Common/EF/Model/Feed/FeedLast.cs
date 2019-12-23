using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("feed_last")]
    public class FeedLast : BaseEntity
    {
        [Key]
        [Column("last_key")]
        public string LastKey { get; set; }

        [Column("last_date")]
        public DateTime LastDate { get; set; }

        internal override object[] GetKeys() => new object[] { LastKey };
    }
}
