using System;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    public class CrmContact
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public bool IsCompany { get; set; }
        public string Notes { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public int StatusId { get; set; }
        public int CompanyId { get; set; }
        public int ContactTypeId { get; set; }
        public Guid CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public Guid LastModifedBy { get; set; }
        public DateTime LastModifedOn { get; set; }
        public string DisplayName { get; set; }
        public bool IsShared { get; set; }
        public string Currency { get; set; }
    }
    public static class CrmContactExtension
    {
        public static ModelBuilderWrapper AddCrmContact(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddCrmContact, Provider.MySql)
                .Add(PgSqlAddCrmContact, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddCrmContact(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrmContact>(entity =>
            {
                entity.ToTable("crm_contact");

                entity.HasIndex(e => e.CreateOn)
                    .HasDatabaseName("create_on");

                entity.HasIndex(e => new { e.LastModifedOn, e.TenantId })
                    .HasDatabaseName("last_modifed_on");

                entity.HasIndex(e => new { e.TenantId, e.CompanyId })
                    .HasDatabaseName("company_id");

                entity.HasIndex(e => new { e.TenantId, e.DisplayName })
                    .HasDatabaseName("display_name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.CompanyName)
                    .HasColumnName("company_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ContactTypeId).HasColumnName("contact_type_id");

                entity.Property(e => e.CreateBy)
                    .IsRequired()
                    .HasColumnName("create_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.Currency)
                    .HasColumnName("currency")
                    .HasColumnType("varchar(3)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DisplayName)
                    .HasColumnName("display_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Industry)
                    .HasColumnName("industry")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.IsCompany).HasColumnName("is_company");

                entity.Property(e => e.IsShared).HasColumnName("is_shared");

                entity.Property(e => e.LastModifedBy)
                    .HasColumnName("last_modifed_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedOn)
                    .HasColumnName("last_modifed_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Notes)
                    .HasColumnName("notes")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddCrmContact(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrmContact>(entity =>
            {
                entity.ToTable("crm_contact", "onlyoffice");

                entity.HasIndex(e => e.CreateOn)
                    .HasDatabaseName("create_on_crm_contact");

                entity.HasIndex(e => new { e.LastModifedOn, e.TenantId })
                    .HasDatabaseName("last_modifed_on_crm_contact");

                entity.HasIndex(e => new { e.TenantId, e.CompanyId })
                    .HasDatabaseName("company_id");

                entity.HasIndex(e => new { e.TenantId, e.DisplayName })
                    .HasDatabaseName("display_name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.CompanyName)
                    .HasColumnName("company_name")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL::character varying");

                entity.Property(e => e.ContactTypeId).HasColumnName("contact_type_id");

                entity.Property(e => e.CreateBy)
                    .IsRequired()
                    .HasColumnName("create_by")
                    .HasMaxLength(38)
                    .IsFixedLength();

                entity.Property(e => e.CreateOn).HasColumnName("create_on");

                entity.Property(e => e.Currency)
                    .HasColumnName("currency")
                    .HasMaxLength(3)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DisplayName)
                    .HasColumnName("display_name")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Industry)
                    .HasColumnName("industry")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IsCompany).HasColumnName("is_company");

                entity.Property(e => e.IsShared).HasColumnName("is_shared");

                entity.Property(e => e.LastModifedBy)
                    .HasColumnName("last_modifed_by")
                    .HasMaxLength(38)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.LastModifedOn)
                    .HasColumnName("last_modifed_on")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Notes).HasColumnName("notes");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("NULL");
            });

        }
    }
}
