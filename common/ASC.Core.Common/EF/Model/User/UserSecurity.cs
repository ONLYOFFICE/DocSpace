using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("core_usersecurity")]
    public class UserSecurity : BaseEntity
    {
        public int Tenant { get; set; }

        [Key]
        public Guid UserId { get; set; }

        public string PwdHash { get; set; }

        public string PwdHashSha512 { get; set; }

        internal override object[] GetKeys()
        {
            return new object[] { UserId };
        }
    }
}
