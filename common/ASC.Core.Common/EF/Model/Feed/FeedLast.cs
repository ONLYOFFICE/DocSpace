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

        public override object[] GetKeys()
        {
            return new object[] { LastKey };
        }
    }
    public static class FeedLastExtension
    {
        public static ModelBuilderWrapper AddFeedLast(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddFeedLast, Provider.MySql)
                .Add(PgSqlAddFeedLast, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddFeedLast(this ModelBuilder modelBuilder)
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
        }
        public static void PgSqlAddFeedLast(this ModelBuilder modelBuilder)
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
        }
    }
}
