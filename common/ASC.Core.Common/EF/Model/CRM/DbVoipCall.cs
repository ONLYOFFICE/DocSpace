using System;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    public class DbVoipCall
    {
        public string Id { get; set; }
        public string ParentCallId { get; set; }
        public string NumberFrom { get; set; }
        public string NumberTo { get; set; }
        public int Status { get; set; }
        public Guid AnsweredBy { get; set; }
        public DateTime DialDate { get; set; }
        public int DialDuration { get; set; }
        public string RecordSid { get; set; }
        public string RecordUrl { get; set; }
        public int RecordDuration { get; set; }
        public decimal RecordPrice { get; set; }
        public int ContactId { get; set; }
        public decimal Price { get; set; }
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
                    .HasDatabaseName("tenant_id");

                entity.HasIndex(e => new { e.ParentCallId, e.TenantId })
                    .HasDatabaseName("parent_call_id");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.AnsweredBy)
                    .IsRequired()
                    .HasColumnName("answered_by")
                    .HasColumnType("varchar(50)")
                    .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

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
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.NumberTo)
                    .IsRequired()
                    .HasColumnName("number_to")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ParentCallId)
                    .IsRequired()
                    .HasColumnName("parent_call_id")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

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
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.RecordUrl)
                    .HasColumnName("record_url")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

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
                    .HasDatabaseName("tenant_id_crm_voip_calls");

                entity.HasIndex(e => new { e.ParentCallId, e.TenantId })
                    .HasDatabaseName("parent_call_id");

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
