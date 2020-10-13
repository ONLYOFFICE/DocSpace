using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("crm_voip_calls")]
    public class DbVoipCall
    {
        public string Id { get; set; }

        [Column("parent_call_id")]
        public string ParentCallId { get; set; }

        [Column("number_from")]
        public string NumberFrom { get; set; }

        [Column("number_to")]
        public string NumberTo { get; set; }
        public int Status { get; set; }

        [Column("answered_by")]
        public Guid AnsweredBy { get; set; }

        [Column("dial_date")]
        public DateTime DialDate { get; set; }

        [Column("dial_duration")]
        public int DialDuration { get; set; }

        [Column("record_sid")]
        public string RecordSid { get; set; }

        [Column("record_url")]
        public string RecordUrl { get; set; }

        [Column("record_duration")]
        public int RecordDuration { get; set; }

        [Column("record_price")]
        public decimal RecordPrice { get; set; }

        [Column("contact_id")]
        public int ContactId { get; set; }

        public decimal Price { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        public CrmContact CrmContact { get; set; }
    }
    public static class DbVoipCallExtension
    {
        public static ModelBuilderWrapper AddDbVoipCall(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbVoipCall, Provider.MySql)
                .Add(PgSqlAddDbVoipCall, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddDbVoipCall(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbVoipCall>(entity =>
            {
                entity.ToTable("crm_voip_calls");

                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.HasIndex(e => new { e.ParentCallId, e.TenantId })
                    .HasName("parent_call_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AnsweredBy)
                    .IsRequired()
                    .HasColumnName("answered_by")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ContactId).HasColumnName("contact_id");

                entity.Property(e => e.DialDate)
                    .HasColumnName("dial_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.DialDuration).HasColumnName("dial_duration");

                entity.Property(e => e.NumberFrom)
                    .IsRequired()
                    .HasColumnName("number_from")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NumberTo)
                    .IsRequired()
                    .HasColumnName("number_to")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ParentCallId)
                    .IsRequired()
                    .HasColumnName("parent_call_id")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10,4)");

                entity.Property(e => e.RecordDuration).HasColumnName("record_duration");

                entity.Property(e => e.RecordPrice)
                    .HasColumnName("record_price")
                    .HasColumnType("decimal(10,4)");

                entity.Property(e => e.RecordSid)
                    .HasColumnName("record_sid")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RecordUrl)
                    .HasColumnName("record_url")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
        }
        public static void PgSqlAddDbVoipCall(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbVoipCall>(entity =>
            {
                entity.ToTable("crm_voip_calls", "onlyoffice");

                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id_crm_voip_calls");

                entity.HasIndex(e => new { e.ParentCallId, e.TenantId })
                    .HasName("parent_call_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(50);

                entity.Property(e => e.AnsweredBy)
                    .IsRequired()
                    .HasColumnName("answered_by")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'");

                entity.Property(e => e.ContactId).HasColumnName("contact_id");

                entity.Property(e => e.DialDate).HasColumnName("dial_date");

                entity.Property(e => e.DialDuration).HasColumnName("dial_duration");

                entity.Property(e => e.NumberFrom)
                    .IsRequired()
                    .HasColumnName("number_from")
                    .HasMaxLength(50);

                entity.Property(e => e.NumberTo)
                    .IsRequired()
                    .HasColumnName("number_to")
                    .HasMaxLength(50);

                entity.Property(e => e.ParentCallId)
                    .IsRequired()
                    .HasColumnName("parent_call_id")
                    .HasMaxLength(50);

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("numeric(10,4)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.RecordDuration).HasColumnName("record_duration");

                entity.Property(e => e.RecordPrice)
                    .HasColumnName("record_price")
                    .HasColumnType("numeric(10,4)");

                entity.Property(e => e.RecordSid)
                    .HasColumnName("record_sid")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.RecordUrl).HasColumnName("record_url");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
        }
    }
}
