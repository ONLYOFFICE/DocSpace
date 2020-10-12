using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("notify_info")]
    public class NotifyInfo
    {
        [Key]
        [Column("notify_id")]
        public int NotifyId { get; set; }
        public int State { get; set; }
        public int Attempts { get; set; }

        [Column("modify_date")]
        public DateTime ModifyDate { get; set; }
        public int Priority { get; set; }
    }
    public static class NotifyInfoExtension
    {
        public static ModelBuilderWrapper AddNotifyInfo(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddNotifyInfo, Provider.MySql)
                .Add(PgSqlAddNotifyInfo, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddNotifyInfo(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotifyInfo>(entity =>
            {
                entity.HasKey(e => e.NotifyId)
                    .HasName("PRIMARY");

                entity.ToTable("notify_info");

                entity.HasIndex(e => e.State)
                    .HasName("state");

                entity.Property(e => e.NotifyId).HasColumnName("notify_id");

                entity.Property(e => e.Attempts).HasColumnName("attempts");

                entity.Property(e => e.ModifyDate)
                    .HasColumnName("modify_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.State).HasColumnName("state");
            });
        }
        public static void PgSqlAddNotifyInfo(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NotifyInfo>(entity =>
            {
                entity.HasKey(e => e.NotifyId)
                    .HasName("notify_info_pkey");

                entity.ToTable("notify_info", "onlyoffice");

                entity.HasIndex(e => e.State)
                    .HasName("state");

                entity.Property(e => e.NotifyId)
                    .HasColumnName("notify_id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Attempts).HasColumnName("attempts");

                entity.Property(e => e.ModifyDate).HasColumnName("modify_date");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.State).HasColumnName("state");
            });
        }
    }
}
