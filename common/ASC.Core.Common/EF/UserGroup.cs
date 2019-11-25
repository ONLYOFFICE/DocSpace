using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("core_usergroup")]
    public class UserGroup
    {
        public int Tenant { get; set; }

        public Guid UserId { get; set; }

        public Guid GroupId { get; set; }

        [Column("ref_type")]
        public UserGroupRefType RefType { get; set; }

        public bool Removed { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }
    }
}
