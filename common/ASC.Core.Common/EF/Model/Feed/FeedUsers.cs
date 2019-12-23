using System;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("feed_users")]
    public class FeedUsers : BaseEntity
    {
        public string FeedId { get; set; }
        public Guid UserId { get; set; }

        internal override object[] GetKeys() => new object[] { FeedId, UserId };
    }

    public static class FeedUsersExtension
    {
        public static ModelBuilder AddFeedUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedUsers>()
                .HasKey(c => new { c.FeedId, c.UserId });

            return modelBuilder;
        }
    }
}
