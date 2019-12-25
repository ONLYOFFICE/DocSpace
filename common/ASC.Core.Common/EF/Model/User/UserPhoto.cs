using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("core_userphoto")]
    public class UserPhoto : BaseEntity
    {
        public int Tenant { get; set; }

        [Key]
        public Guid UserId { get; set; }

        public byte[] Photo { get; set; }

        internal override object[] GetKeys()
        {
            return new object[] { UserId };
        }
    }
}
