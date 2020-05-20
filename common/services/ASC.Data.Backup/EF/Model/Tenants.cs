

using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Data.Backup.EF.Model
{
    [Table("tenants_tenants")]
    class Tenants
    {
        public int Id { get; set; }
        public int Status { get; set; }
    }
}
