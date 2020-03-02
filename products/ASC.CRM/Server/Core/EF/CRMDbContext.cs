using System;
using ASC.Core.Common.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ASC.CRM.Core.EF
{
    public partial class CRMDbContext : BaseDbContext
    {
        //public CRMDbContext()
        //{
        //}

        //public CRMDbContext(DbContextOptions<CRMDbContext> options)
        //    : base(options)
        //{
        //}

        public virtual DbSet<CrmCase> CrmCase { get; set; }
        public virtual DbSet<CrmContact> CrmContact { get; set; }
        public virtual DbSet<CrmContactInfo> CrmContactInfo { get; set; }
        public virtual DbSet<CrmCurrencyInfo> CrmCurrencyInfo { get; set; }
        public virtual DbSet<CrmCurrencyRate> CrmCurrencyRate { get; set; }
        public virtual DbSet<CrmDeal> CrmDeal { get; set; }
        public virtual DbSet<CrmDealMilestone> CrmDealMilestone { get; set; }
        public virtual DbSet<CrmEntityContact> CrmEntityContact { get; set; }
        public virtual DbSet<CrmEntityTag> CrmEntityTag { get; set; }
        public virtual DbSet<CrmFieldDescription> CrmFieldDescription { get; set; }
        public virtual DbSet<CrmFieldValue> CrmFieldValue { get; set; }
        public virtual DbSet<CrmInvoice> CrmInvoice { get; set; }
        public virtual DbSet<CrmInvoiceItem> CrmInvoiceItem { get; set; }
        public virtual DbSet<CrmInvoiceLine> CrmInvoiceLine { get; set; }
        public virtual DbSet<CrmInvoiceTax> CrmInvoiceTax { get; set; }
        public virtual DbSet<CrmListItem> CrmListItem { get; set; }
        public virtual DbSet<CrmOrganisationLogo> CrmOrganisationLogo { get; set; }
        public virtual DbSet<CrmProjects> CrmProjects { get; set; }
        public virtual DbSet<CrmRelationshipEvent> CrmRelationshipEvent { get; set; }
        public virtual DbSet<CrmReportFile> CrmReportFile { get; set; }
        public virtual DbSet<CrmTag> CrmTag { get; set; }
        public virtual DbSet<CrmTask> CrmTask { get; set; }
        public virtual DbSet<CrmTaskTemplate> CrmTaskTemplate { get; set; }
        public virtual DbSet<CrmTaskTemplateContainer> CrmTaskTemplateContainer { get; set; }
        public virtual DbSet<CrmTaskTemplateTask> CrmTaskTemplateTask { get; set; }
        public virtual DbSet<CrmVoipCalls> CrmVoipCalls { get; set; }
        public virtual DbSet<CrmVoipNumber> CrmVoipNumber { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CrmCase>(entity =>
            {
                entity.HasIndex(e => e.CreateOn)
                    .HasName("create_on");

                entity.HasIndex(e => e.LastModifedOn)
                    .HasName("last_modifed_on");

                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmContact>(entity =>
            {
                entity.HasIndex(e => e.CreateOn)
                    .HasName("create_on");

                entity.HasIndex(e => new { e.LastModifedOn, e.TenantId })
                    .HasName("last_modifed_on");

                entity.HasIndex(e => new { e.TenantId, e.CompanyId })
                    .HasName("company_id");

                entity.HasIndex(e => new { e.TenantId, e.DisplayName })
                    .HasName("display_name");

                entity.Property(e => e.CompanyName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Currency)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.DisplayName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FirstName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Industry)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Notes)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmContactInfo>(entity =>
            {
                entity.HasIndex(e => e.LastModifedOn)
                    .HasName("last_modifed_on");

                entity.HasIndex(e => new { e.TenantId, e.ContactId })
                    .HasName("IX_Contact");

                entity.Property(e => e.Data)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmCurrencyInfo>(entity =>
            {
                entity.HasKey(e => e.Abbreviation)
                    .HasName("PRIMARY");

                entity.Property(e => e.Abbreviation)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CultureName)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ResourceKey)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Symbol)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmCurrencyRate>(entity =>
            {
                entity.HasIndex(e => e.FromCurrency)
                    .HasName("from_currency");

                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.FromCurrency)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ToCurrency)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmDeal>(entity =>
            {
                entity.HasIndex(e => e.CreateOn)
                    .HasName("create_on");

                entity.HasIndex(e => e.DealMilestoneId)
                    .HasName("deal_milestone_id");

                entity.HasIndex(e => e.LastModifedOn)
                    .HasName("last_modifed_on");

                entity.HasIndex(e => new { e.TenantId, e.ContactId })
                    .HasName("contact_id");

                entity.Property(e => e.BidCurrency)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ResponsibleId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmDealMilestone>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.Color)
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmEntityContact>(entity =>
            {
                entity.HasKey(e => new { e.EntityId, e.EntityType, e.ContactId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ContactId)
                    .HasName("IX_Contact");
            });

            modelBuilder.Entity<CrmEntityTag>(entity =>
            {
                entity.HasKey(e => new { e.EntityId, e.EntityType, e.TagId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.TagId)
                    .HasName("tag_id");
            });

            modelBuilder.Entity<CrmFieldDescription>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.EntityType, e.SortOrder })
                    .HasName("entity_type");

                entity.Property(e => e.Label)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Mask)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmFieldValue>(entity =>
            {
                entity.HasIndex(e => e.FieldId)
                    .HasName("field_id");

                entity.HasIndex(e => e.LastModifedOn)
                    .HasName("last_modifed_on");

                entity.HasIndex(e => new { e.TenantId, e.EntityId, e.EntityType, e.FieldId })
                    .HasName("tenant_id");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Value)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmInvoice>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.ConsigneeId).HasDefaultValueSql("'-1'");

                entity.Property(e => e.ContactId).HasDefaultValueSql("'-1'");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Currency)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ExchangeRate).HasDefaultValueSql("'1.00'");

                entity.Property(e => e.FileId).HasDefaultValueSql("'-1'");

                entity.Property(e => e.JsonData)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Language)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Number)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.PurchaseOrderNumber)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasDefaultValueSql("'1'");

                entity.Property(e => e.Terms)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmInvoiceItem>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Currency)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.StockKeepingUnit)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmInvoiceLine>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmInvoiceTax>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmListItem>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.ListType })
                    .HasName("list_type");

                entity.Property(e => e.AdditionalParams)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Color)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmOrganisationLogo>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.Content)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmProjects>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.ContactId, e.ProjectId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.TenantId, e.ContactId })
                    .HasName("contact_id");

                entity.HasIndex(e => new { e.TenantId, e.ProjectId })
                    .HasName("project_id");
            });

            modelBuilder.Entity<CrmRelationshipEvent>(entity =>
            {
                entity.HasIndex(e => e.ContactId)
                    .HasName("IX_Contact");

                entity.HasIndex(e => e.LastModifedOn)
                    .HasName("last_modifed_on");

                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.HasIndex(e => new { e.EntityId, e.EntityType })
                    .HasName("IX_Entity");

                entity.Property(e => e.Content)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmReportFile>(entity =>
            {
                entity.HasKey(e => e.FileId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.CreateBy)
                    .HasName("create_by");

                entity.HasIndex(e => e.CreateOn)
                    .HasName("create_on");

                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmTag>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_bin");
            });

            modelBuilder.Entity<CrmTask>(entity =>
            {
                entity.HasIndex(e => e.CreateOn)
                    .HasName("create_on");

                entity.HasIndex(e => e.Deadline)
                    .HasName("deadline");

                entity.HasIndex(e => e.LastModifedOn)
                    .HasName("last_modifed_on");

                entity.HasIndex(e => new { e.TenantId, e.ContactId })
                    .HasName("IX_Contact");

                entity.HasIndex(e => new { e.TenantId, e.ResponsibleId })
                    .HasName("responsible_id");

                entity.HasIndex(e => new { e.TenantId, e.EntityId, e.EntityType })
                    .HasName("IX_Entity");

                entity.Property(e => e.ContactId).HasDefaultValueSql("'-1'");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ResponsibleId)
                    .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmTaskTemplate>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.ContainerId })
                    .HasName("template_id");

                entity.Property(e => e.CreateBy)
                    .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ResponsibleId)
                    .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmTaskTemplateContainer>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.EntityType })
                    .HasName("entity_type");

                entity.Property(e => e.CreateBy)
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmTaskTemplateTask>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.TaskId, e.TaskTemplateId })
                    .HasName("PRIMARY");
            });

            modelBuilder.Entity<CrmVoipCalls>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.HasIndex(e => new { e.ParentCallId, e.TenantId })
                    .HasName("parent_call_id");

                entity.Property(e => e.Id)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.AnsweredBy)
                    .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NumberFrom)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.NumberTo)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.ParentCallId)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RecordSid)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.RecordUrl)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            modelBuilder.Entity<CrmVoipNumber>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasName("tenant_id");

                entity.Property(e => e.Id)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Alias)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Number)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Settings)
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}