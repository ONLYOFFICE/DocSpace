using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    [Table("core_usergroup")]
    public class UserGroup : BaseEntity
    {
        public int Tenant { get; set; }

        public Guid UserId { get; set; }

        public Guid GroupId { get; set; }

        [Column("ref_type")]
        public UserGroupRefType RefType { get; set; }

        public bool Removed { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        internal override object[] GetKeys()
        {
            return new object[] { Tenant, UserId, GroupId, RefType };
        }
    }

    public static class DbUserGroupExtension
    {
        public static void AddUserGroup(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>()
                .HasKey(c => new { c.Tenant, c.UserId, c.GroupId, c.RefType });
        }
    }
}
