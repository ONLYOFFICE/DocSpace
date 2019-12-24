using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("feed_aggregate")]
    public class FeedAggregate : BaseEntity
    {
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string Product { get; set; }
        public string Module { get; set; }
        public Guid Author { get; set; }

        [Column("modified_by")]
        public Guid ModifiedBy { get; set; }

        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("modified_date")]
        public DateTime ModifiedDate { get; set; }

        [Column("group_id")]
        public string GroupId { get; set; }

        [Column("aggregated_date")]
        public DateTime AggregateDate { get; set; }
        public string Json { get; set; }
        public string Keywords { get; set; }

        internal override object[] GetKeys() => new object[] { Id };
    }
}
