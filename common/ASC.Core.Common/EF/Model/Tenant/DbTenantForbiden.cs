using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("tenants_forbiden")]
    public class DbTenantForbiden
    {
        [Key]
        public string Address { get; set; }
    }
}
