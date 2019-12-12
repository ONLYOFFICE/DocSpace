using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("notify_info")]
    public class NotifyInfo
    {
        [Key]
        [Column("notify_id")]
        public int NotifyId { get; set; }
        public int State { get; set; }
        public int Attempts { get; set; }

        [Column("modify_date")]
        public DateTime ModifyDate { get; set; }
        public int Priority { get; set; }
    }
}
