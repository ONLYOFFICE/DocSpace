using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Resource
{
    [Table("res_authors")]
    public class ResAuthors
    {
        [Key]
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public bool Online { get; set; }
        public DateTime LastVisit { get; set; }
    }
}
