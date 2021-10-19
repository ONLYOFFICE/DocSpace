using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ASC.Webhooks.Core.Dao.Models
{
    public partial class WebhooksLog
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public string Uid { get; set; }
        public int TenantId { get; set; }
        public string RequestPayload { get; set; }
        public string RequestHeaders { get; set; }
        public string ResponsePayload { get; set; }
        public string ResponseHeaders { get; set; }
        public DateTime CreationTime { get; set; }
        public string Event { get; set; }
        public ProcessStatus Status { get; set; }
    }

    public static class WebhooksPayloadExtension
    {
        public static ModelBuilderWrapper AddWebhooksLog(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddWebhooksLog, Provider.MySql);
            //.Add(PgSqlAddUser, Provider.Postgre)

            return modelBuilder;
        }

        private static void MySqlAddWebhooksLog(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebhooksLog>(entity =>
            {
                entity.HasKey(e => new { e.Id })
                   .HasName("PRIMARY");

                entity.ToTable("webhooks_logs");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ConfigId)
                   .HasColumnType("int")
                   .HasColumnName("config_id");

                entity.Property(e => e.Uid)
                    .HasColumnType("varchar")
                    .HasColumnName("uid")
                    .HasMaxLength(50);

                entity.Property(e => e.TenantId)
                    .HasColumnName("tenant_id")
                    .HasColumnType("int unsigned");

                entity.Property(e => e.RequestPayload)
                    .IsRequired()
                    .HasColumnName("request_payload")
                    .HasColumnType("json");

                entity.Property(e => e.RequestHeaders)
                    .HasColumnName("request_headers")
                    .HasColumnType("json");

                entity.Property(e => e.ResponsePayload)
                    .HasColumnName("response_payload")
                    .HasColumnType("json");

                entity.Property(e => e.ResponseHeaders)
                    .HasColumnName("response_headers")
                    .HasColumnType("json");

                entity.Property(e => e.Event)
                    .HasColumnType("varchar")
                    .HasColumnName("event")
                    .HasMaxLength(100);

                entity.Property(e => e.CreationTime)
                   .HasColumnType("datetime")
                   .HasColumnName("creation_time");

                entity.Property(e => e.Status)
                    .HasColumnType("varchar")
                    .HasColumnName("status")
                    .HasMaxLength(50);
            });
        }
    }
}
