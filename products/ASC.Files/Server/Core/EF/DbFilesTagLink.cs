using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("files_tag_link")]
    public class DbFilesTagLink : BaseEntity, IDbFile
    {
        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("tag_id")]
        public int TagId { get; set; }

        [Column("entry_type")]
        public FileEntryType EntryType { get; set; }

        [Column("entry_id")]
        public string EntryId { get; set; }

        [Column("create_by")]
        public Guid CreateBy { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }

        [Column("tag_count")]
        public int TagCount { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { TenantId, TagId, EntryId, EntryType };
        }
    }

    public static class DbFilesTagLinkExtension
    {
        public static ModelBuilder AddDbFilesTagLink(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesTagLink>()
                .HasKey(c => new { c.TenantId, c.TagId, c.EntryId, c.EntryType });

            return modelBuilder;
        }
    }
}
