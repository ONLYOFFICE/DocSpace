using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Resource
{
    [Table("res_files")]
    public class ResFiles
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ModuleName { get; set; }
        public string ResName { get; set; }
        public bool IsLock { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
