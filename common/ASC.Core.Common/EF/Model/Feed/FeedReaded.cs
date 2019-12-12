using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("feed_readed")]
    public class FeedReaded
    {
        [Column("user_id")]
        public Guid UserId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Module { get; set; }

        [Column("tenant_id")]
        public int Tenant { get; set; }
    }

    public static class FeedReadedExtension
    {
        public static ModelBuilder AddFeedReaded(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedReaded>()
                .HasKey(c => new { c.Tenant, c.UserId, c.Module });

            return modelBuilder;
        }
    }
}
