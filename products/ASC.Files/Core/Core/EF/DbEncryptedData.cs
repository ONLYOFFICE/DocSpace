using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("encrypted_data")]
    public class DbEncryptedData : BaseEntity, IDbFile
    {
        [Column("public_key")]
        public string PublicKey { get; set; }

        [Column("file_hash")]
        public string FileHash { get; set; }

        public string Data { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        public override object[] GetKeys() => new object[] { PublicKey, FileHash };
    }

    public static class DbEncryptedDataExtension
    {
        public static ModelBuilder AddDbEncryptedData(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<DbEncryptedData>()
                .HasKey(c => new { c.PublicKey, c.FileHash });

            return modelBuilder;
        }
    }
}
