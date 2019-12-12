using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("tenants_version")]
    public class DbTenantVersion
    {
        public int Id { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }

        [Column("default_version")]
        public int DefaultVersion { get; set; }
        public bool Visible { get; set; }
    }
}
