using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model
{
    [Table("crm_contact")]
    public class CrmContact
    {
        public int Id { get; set; }

        [Column("tenant_id")]
        public int TenantId { get; set; }

        [Column("is_company")]
        public bool IsCompany { get; set; }
        public string Notes { get; set; }
        public string Title { get; set; }

        [Column("first_name")]
        public string FirstName { get; set; }

        [Column("last_name")]
        public string LastName { get; set; }

        [Column("company_name")]
        public string CompanyName { get; set; }
        public string Industry { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("company_id")]
        public int CompanyId { get; set; }

        [Column("contact_type_id")]
        public int ContactTypeId { get; set; }

        [Column("create_by")]
        public Guid CreateBy { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }

        [Column("last_modifed_by")]
        public Guid LastModifedBy { get; set; }

        [Column("last_modifed_on")]
        public DateTime LastModifedOn { get; set; }

        [Column("display_name")]
        public string DisplayName { get; set; }

        [Column("is_shared")]
        public bool IsShared { get; set; }
        public string Currency { get; set; }
    }
    public static class CrmContactExtension
    {
        public static ModelBuilderWrapper AddCrmContact(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddCrmContact, Provider.MySql)
                .Add(PgSqlAddCrmContact, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddCrmContact(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrmContact>(entity =>
            {
                entity.ToTable("crm_contact");

                entity.HasIndex(e => e.CreateOn)
                    .HasName("create_on");

                entity.HasIndex(e => new { e.LastModifedOn, e.TenantId })
                    .HasName("last_modifed_on");

                entity.HasIndex(e => new { e.TenantId, e.CompanyId })
                    .HasName("company_id");

                entity.HasIndex(e => new { e.TenantId, e.DisplayName })
                    .HasName("display_name");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompanyId).HasColumnName("company_id");

                entity.Property(e => e.CompanyName)
                    .HasColumnName("company_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ContactTypeId).HasColumnName("contact_type_id");

                entity.Property(e => e.CreateBy)
                    .IsRequired()
                    .HasColumnName("create_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.Currency)
                    .HasColumnName("currency")
                    .HasColumnType("varchar(3)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DisplayName)
                    .HasColumnName("display_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FirstName)
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Industry)
                    .HasColumnName("industry")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.IsCompany).HasColumnName("is_company");

                entity.Property(e => e.IsShared).HasColumnName("is_shared");

                entity.Property(e => e.LastModifedBy)
                    .HasColumnName("last_modifed_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedOn)
                    .HasColumnName("last_modifed_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Notes)
                    .HasColumnName("notes")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddCrmContact(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrmContact>(entity =>
            {
                entity.ToTable("crm_contact", "onlyoffice");

                entity.HasIndex(e => e.CreateOn)
                    .HasName("create_on_crm_contact");

                entity.HasIndex(e => new { e.LastModifedOn, e.TenantId })
                    .HasName("last_modifed_on_crm_contact");

                entity.HasIndex(e => new { e.TenantId, e.CompanyId })
                    .HasName("company_id");

                entity.HasIndex(e => new { e.TenantId, e.DisplayName })
                    .HasName("display_name");

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
