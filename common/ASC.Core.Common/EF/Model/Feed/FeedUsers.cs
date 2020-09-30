using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("feed_users")]
    public class FeedUsers : BaseEntity
    {
        [Column("feed_id")]
        public string FeedId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { FeedId, UserId };
        }
    }

    public static class FeedUsersExtension
    {
        public static ModelBuilder AddFeedUsers(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<FeedUsers>()
                .HasKey(c => new { c.FeedId, c.UserId });

            return modelBuilder;
        }
    }
}
