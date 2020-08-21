using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("feed_last")]
    public class FeedLast : BaseEntity
    {
        [Key]
        [Column("last_key")]
        public string LastKey { get; set; }

        [Column("last_date")]
        public DateTime LastDate { get; set; }

        public override object[] GetKeys() => new object[] { LastKey };
    }
    public static class FeedLastExtension
    {
        public static ModelBuilder MySqlAddFeedLast(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedLast>(entity =>
            {
                entity.HasKey(e => e.LastKey)
                    .HasName("PRIMARY");

                entity.ToTable("feed_last");

                entity.Property(e => e.LastKey)
                    .HasColumnName("last_key")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastDate)
                    .HasColumnName("last_date")
                    .HasColumnType("datetime");
            });

            return modelBuilder;
        }
        public static ModelBuilder PgSqlAddFeedLast(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FeedLast>(entity =>
            {
                entity.HasKey(e => e.LastKey)
                    .HasName("feed_last_pkey");

                entity.ToTable("feed_last", "onlyoffice");

                entity.Property(e => e.LastKey)
                    .HasColumnName("last_key")
                    .HasMaxLength(128);

                entity.Property(e => e.LastDate).HasColumnName("last_date");
            });

            return modelBuilder;
        }
    }
}
