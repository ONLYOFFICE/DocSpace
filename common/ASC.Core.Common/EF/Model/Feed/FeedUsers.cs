using System;
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    public class FeedUsers : BaseEntity
    {
        public string FeedId { get; set; }
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
            modelBuilder
                .Add(MySqlAddFeedUsers, Provider.MySql)
                .Add(PgSqlAddFeedUsers, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddFeedUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedUsers>(entity =>
            {
                entity.HasKey(e => new { e.FeedId, e.UserId })
                    .HasName("PRIMARY");

                entity.ToTable("feed_users");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("user_id");

                entity.Property(e => e.FeedId)
                    .HasColumnName("feed_id")
                    .HasColumnType("varchar(88)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddFeedUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedUsers>(entity =>
            {
                entity.HasKey(e => new { e.FeedId, e.UserId })
                    .HasName("feed_users_pkey");

                entity.ToTable("feed_users", "onlyoffice");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("user_id_feed_users");

                entity.Property(e => e.FeedId)
                    .HasColumnName("feed_id")
                    .HasMaxLength(88);

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasMaxLength(38)
                    .IsFixedLength();
            });
        }
    }
}
