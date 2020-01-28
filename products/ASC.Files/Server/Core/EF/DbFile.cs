using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("files_file")]
    public class DbFile : BaseEntity, IDbFile, IDbSearch
    {
        public int Id { get; set; }
        public int Version { get; set; }

        [Column("version_group")]
        public int VersionGroup { get; set; }

        [Column("current_version")]
        public bool CurrentVersion { get; set; }

        [Column("folder_id")]
        public int FolderId { get; set; }

        public string Title { get; set; }

        [Column("content_length")]
        public long ContentLength { get; set; }

        [Column("file_status")]
        public int FileStatus { get; set; }

        [Column("category")]
        public int Category { get; set; }

        [Column("create_by")]
        public Guid CreateBy { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }

        [Column("modified_by")]
        public Guid ModifiedBy { get; set; }

        [Column("modified_on")]
        public DateTime ModifiedOn { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("converted_type")]
        public string ConvertedType { get; set; }

        public string Comment { get; set; }
        public string Changes { get; set; }
        public bool Encrypted { get; set; }
        public ForcesaveType Forcesave { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { TenantId, Id, Version };
        }
    }

    public static class DbFileExtension
    {
        public static ModelBuilder AddDbFiles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFile>()
                .HasKey(c => new { c.TenantId, c.Id, c.Version });

            return modelBuilder;
        }
    }
}
