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
            _ = modelBuilder
                .Add(MySqlAddNotifyInfo, Provider.MySql)
                .Add(PgSqlAddNotifyInfo, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddNotifyInfo(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<NotifyInfo>(entity =>
            {
                _ = entity.HasKey(e => e.NotifyId)
                    .HasName("PRIMARY");

                _ = entity.ToTable("notify_info");

                _ = entity.HasIndex(e => e.State)
                    .HasName("state");

                _ = entity.Property(e => e.NotifyId).HasColumnName("notify_id");

                _ = entity.Property(e => e.Attempts).HasColumnName("attempts");

                _ = entity.Property(e => e.ModifyDate)
                    .HasColumnName("modify_date")
                    .HasColumnType("datetime");

                _ = entity.Property(e => e.Priority).HasColumnName("priority");

                _ = entity.Property(e => e.State).HasColumnName("state");
            });
        }
        public static void PgSqlAddNotifyInfo(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<NotifyInfo>(entity =>
            {
                _ = entity.HasKey(e => e.NotifyId)
                    .HasName("notify_info_pkey");

                _ = entity.ToTable("notify_info", "onlyoffice");

                _ = entity.HasIndex(e => e.State)
                    .HasName("state");

                _ = entity.Property(e => e.NotifyId)
                    .HasColumnName("notify_id")
                    .ValueGeneratedNever();

                _ = entity.Property(e => e.Attempts).HasColumnName("attempts");

                _ = entity.Property(e => e.ModifyDate).HasColumnName("modify_date");

                _ = entity.Property(e => e.Priority).HasColumnName("priority");

                _ = entity.Property(e => e.State).HasColumnName("state");
            });
        }
    }
}
