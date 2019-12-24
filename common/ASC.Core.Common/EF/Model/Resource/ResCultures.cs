using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Resource
{
    [Table("res_cultures")]
    public class ResCultures
    {
        [Key]
        public string Title { get; set; }
        public string Value { get; set; }
        public bool Available { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
