using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("core_group")]
    public class DbGroup
    {
        public int Tenant { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public Guid ParentId { get; set; }
        public string Sid { get; set; }
        public bool Removed { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        public static implicit operator Group(DbGroup group) => new Group
        {
            Id = group.Id,
            Name = group.Name,
            CategoryId = group.CategoryId,
            ParentId = group.ParentId,
            Sid = group.Sid,
            Removed = group.Removed,
            LastModified = group.LastModified,
            Tenant = group.Tenant
        };

        public static implicit operator DbGroup(Group group) => new DbGroup
        {
            Id = group.Id,
            Name = group.Name,
            CategoryId = group.CategoryId,
            ParentId = group.ParentId,
            Sid = group.Sid,
            Removed = group.Removed,
            LastModified = group.LastModified,
            Tenant = group.Tenant
        };
    }
}
