using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;
using ASC.Files.Core.Security;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("files_security")]
    public class DbFilesSecurity : BaseEntity, IDbFile
    {
        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("entry_id")]
        public string EntryId { get; set; }

        [Column("entry_type")]
        public FileEntryType EntryType { get; set; }

        public Guid Subject { get; set; }
        public Guid Owner { get; set; }
        public FileShare Security { get; set; }
        public DateTime TimeStamp { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { TenantId, EntryId, EntryType, Subject };
        }
    }

    public static class DbFilesSecurityExtension
    {
        public static ModelBuilder AddDbFilesSecurity(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesSecurity>()
                .HasKey(c => new { c.TenantId, c.EntryId, c.EntryType, c.Subject });

            return modelBuilder;
        }
    }
}
