using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore;

namespace ASC.Files.Core.EF
{
    [Table("files_thirdparty_app")]
    public class DbFilesThirdpartyApp : BaseEntity, IDbFile
    {
        [Column("user_id")]
        public Guid UserId { get; set; }

        public string App { get; set; }

        public string Token { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("modified_on")]
        public DateTime ModifiedOn { get; set; }

        public override object[] GetKeys() => new object[] { UserId, App };
    }

    public static class DbFilesThirdpartyAppExtension
    {
        public static ModelBuilder AddDbFilesThirdpartyApp(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFilesThirdpartyApp>()
                .HasKey(c => new { c.UserId, c.App });

            return modelBuilder;
        }
    }
}
