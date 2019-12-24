using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("core_group")]
    public class DbGroup : BaseEntity
    {
        public int Tenant { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ParentId { get; set; }
        public string Sid { get; set; }
        public bool Removed { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        internal override object[] GetKeys()
        {
            return new object[] { Id };
        }
    }
}
