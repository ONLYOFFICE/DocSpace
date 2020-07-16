using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Files.Core.EF
{
    [Table("files_tag")]
    public class DbFilesTag : IDbFile
    {
        [Column("tenant_id")]
        public int TenantId { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public Guid Owner { get; set; }

        public TagType Flag { get; set; }
    }
}
