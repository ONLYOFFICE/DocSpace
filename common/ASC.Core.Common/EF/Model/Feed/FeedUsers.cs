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
        public static ModelBuilderWrapper AddFeedUsers(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddFeedUsers, Provider.MySql)
                .Add(PgSqlAddFeedUsers, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddFeedUsers(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<FeedUsers>(entity =>
            {
                _ = entity.HasKey(e => new { e.FeedId, e.UserId })
                    .HasName("PRIMARY");

                _ = entity.ToTable("feed_users");

                _ = entity.HasIndex(e => e.UserId)
                    .HasName("user_id");

                _ = entity.Property(e => e.FeedId)
                    .HasColumnName("feed_id")
                    .HasColumnType("varchar(88)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddFeedUsers(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<FeedUsers>(entity =>
            {
                _ = entity.HasKey(e => new { e.FeedId, e.UserId })
                    .HasName("feed_users_pkey");

                _ = entity.ToTable("feed_users", "onlyoffice");

                _ = entity.HasIndex(e => e.UserId)
                    .HasName("user_id_feed_users");

                _ = entity.Property(e => e.FeedId)
                    .HasColumnName("feed_id")
                    .HasMaxLength(88);

                _ = entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(38)
                    .IsFixedLength();
            });
        }
    }
}
