using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("files_thirdparty_id_mapping")]
    public class DbFilesThirdpartyIdMapping : BaseEntity, IDbFile
    {
        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("hash_id")]
        public string HashId { get; set; }
        public string Id { get; set; }

        public override object[] GetKeys() => new object[] { Id };
    }

    public static class DbDbFilesThirdpartyIdMapping
    {
        public static ModelBuilder AddDbFilesThirdpartyIdMapping(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesThirdpartyIdMapping>()
                .HasKey(c => new { c.HashId });

            return modelBuilder;
        }
    }
}
