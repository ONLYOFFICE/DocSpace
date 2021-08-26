using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ASC.Webhooks.Core.Dao.Models
{
    public partial class WebhooksPayload
    {
        public int Id { get; set; }
        public int ConfigId { get; set; }
        public int TenantId { get; set; }
        public string Data { get; set; }
        public DateTime CreationTime { get; set; }
        public string Event { get; set; }
        public ProcessStatus Status { get; set; }
    }

    public static class WebhooksPayloadExtension
    {
        public static ModelBuilderWrapper AddWebhooksPayload(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddWebhooksPayload, Provider.MySql);
                //.Add(PgSqlAddUser, Provider.Postgre)

            return modelBuilder;
        }

        private static void MySqlAddWebhooksPayload(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WebhooksPayload>(entity =>
            {
                entity.HasKey(e => new { e.Id })
                   .HasName("PRIMARY");

                entity.ToTable("webhooks_payload");

                entity.Property(e => e.Id)
                    .HasColumnType("int")
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.ConfigId)
                   .HasColumnType("int")
                   .HasColumnName("ConfigID");

                entity.Property(e => e.Data)
                    .IsRequired()
                    .HasColumnType("json");

                entity.Property(e => e.Event)
                    .HasColumnType("varchar")
                    .HasColumnName("Event")
                    .HasMaxLength(100);

                entity.Property(e => e.Status)
                    .HasColumnType("varchar")
                    .HasColumnName("Status")
                    .HasMaxLength(50);

                entity.Property(e => e.TenantId)
                    .HasColumnType("int unsigned")
                    .HasColumnName("TenantID");
            });      
        }
    }
}
