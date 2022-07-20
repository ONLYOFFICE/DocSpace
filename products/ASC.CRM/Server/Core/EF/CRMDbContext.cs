// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.CRM.Core.EF
{
    public partial class CrmDbContext : BaseDbContext
    {
        public virtual DbSet<DbCase> Cases { get; set; }
        public virtual DbSet<DbContact> Contacts { get; set; }
        public virtual DbSet<DbContactInfo> ContactsInfo { get; set; }
        public virtual DbSet<DbCurrencyInfo> CurrencyInfo { get; set; }
        public virtual DbSet<DbCurrencyRate> CurrencyRate { get; set; }
        public virtual DbSet<DbDeal> Deals { get; set; }
        public virtual DbSet<DbDealMilestone> DealMilestones { get; set; }
        public virtual DbSet<DbEntityContact> EntityContact { get; set; }
        public virtual DbSet<DbEntityTag> EntityTags { get; set; }
        public virtual DbSet<DbFieldDescription> FieldDescription { get; set; }
        public virtual DbSet<DbFieldValue> FieldValue { get; set; }
        public virtual DbSet<DbInvoice> Invoices { get; set; }
        public virtual DbSet<DbInvoiceItem> InvoiceItem { get; set; }
        public virtual DbSet<DbInvoiceLine> InvoiceLine { get; set; }
        public virtual DbSet<DbInvoiceTax> InvoiceTax { get; set; }
        public virtual DbSet<DbListItem> ListItem { get; set; }
        public virtual DbSet<DbOrganisationLogo> OrganisationLogo { get; set; }
        public virtual DbSet<DbProjects> Projects { get; set; }
        public virtual DbSet<DbRelationshipEvent> RelationshipEvent { get; set; }
        public virtual DbSet<DbReportFile> ReportFile { get; set; }
        public virtual DbSet<DbTag> Tags { get; set; }
        public virtual DbSet<DbTask> Tasks { get; set; }
        public virtual DbSet<DbTaskTemplate> TaskTemplates { get; set; }
        public virtual DbSet<DbTaskTemplateContainer> TaskTemplateContainer { get; set; }
        public virtual DbSet<DbTaskTemplateTask> TaskTemplateTask { get; set; }
        public virtual DbSet<DbTenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper.From(modelBuilder, _provider)
                               .AddDbFieldValue()
                               .AddDbContact()
                               .AddDbContactInfo()
                               .AddDbCase()
                               .AddDbRelationshipEvent()
                               .AddDbDeal()
                               .AddDbTask()
                               .AddDbTenant();

            modelBuilder.Entity<DbCurrencyInfo>(entity =>
            {
                entity.HasKey(e => e.Abbreviation)
                    .HasName("PRIMARY");

                entity.Property(e => e.Abbreviation)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CultureName)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ResourceKey)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Symbol)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DbCurrencyRate>(entity =>
            {
                entity.HasIndex(e => e.FromCurrency)
                    .HasDatabaseName("from_currency");

                entity.HasIndex(e => e.TenantId)
                    .HasDatabaseName("tenant_id");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.FromCurrency)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ToCurrency)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            ;

            modelBuilder.Entity<DbDealMilestone>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasDatabaseName("tenant_id");

                entity.Property(e => e.Color)
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DbEntityContact>(entity =>
            {
                entity.HasKey(e => new { e.EntityId, e.EntityType, e.ContactId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.ContactId)
                    .HasDatabaseName("IX_Contact");
            });

            modelBuilder.Entity<DbEntityTag>(entity =>
            {
                entity.HasKey(e => new { e.EntityId, e.EntityType, e.TagId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.TagId)
                    .HasDatabaseName("tag_id");
            });

            modelBuilder.Entity<DbFieldDescription>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.EntityType, e.SortOrder })
                    .HasDatabaseName("entity_type");

                entity.Property(e => e.Label)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Mask)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });


            modelBuilder.Entity<DbInvoice>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasDatabaseName("tenant_id");

                entity.Property(e => e.ConsigneeId).HasDefaultValueSql("'-1'");

                entity.Property(e => e.ContactId).HasDefaultValueSql("'-1'");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Currency)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ExchangeRate).HasDefaultValueSql("'1.00'");

                entity.Property(e => e.FileId).HasDefaultValueSql("'-1'");

                entity.Property(e => e.JsonData)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Language)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Number)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.PurchaseOrderNumber)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasDefaultValueSql("'1'");

                entity.Property(e => e.Terms)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DbInvoiceItem>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasDatabaseName("tenant_id");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Currency)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.StockKeepingUnit)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DbInvoiceLine>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasDatabaseName("tenant_id");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DbInvoiceTax>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasDatabaseName("tenant_id");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Name)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DbListItem>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.ListType })
                    .HasDatabaseName("list_type");

                entity.Property(e => e.AdditionalParams)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Color)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DbOrganisationLogo>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasDatabaseName("tenant_id");

                entity.Property(e => e.Content)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DbProjects>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.ContactId, e.ProjectId })
                    .HasName("PRIMARY");

                entity.HasIndex(e => new { e.TenantId, e.ContactId })
                    .HasDatabaseName("contact_id");

                entity.HasIndex(e => new { e.TenantId, e.ProjectId })
                    .HasDatabaseName("project_id");
            });


            modelBuilder.Entity<DbReportFile>(entity =>
            {
                entity.HasKey(e => e.FileId)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.CreateBy)
                    .HasDatabaseName("create_by");

                entity.HasIndex(e => e.CreateOn)
                    .HasDatabaseName("create_on");

                entity.HasIndex(e => e.TenantId)
                    .HasDatabaseName("tenant_id");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DbTag>(entity =>
            {
                entity.HasIndex(e => e.TenantId)
                    .HasDatabaseName("tenant_id");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_bin");
            });


            modelBuilder.Entity<DbTaskTemplate>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.ContainerId })
                    .HasDatabaseName("template_id");

                entity.Property(e => e.CreateBy)
                    .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ResponsibleId)
                    .HasDefaultValueSql("'00000000-0000-0000-0000-000000000000'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DbTaskTemplateContainer>(entity =>
            {
                entity.HasIndex(e => new { e.TenantId, e.EntityType })
                    .HasDatabaseName("entity_type");

                entity.Property(e => e.CreateBy)
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasDefaultValueSql("'0'")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });

            modelBuilder.Entity<DbTaskTemplateTask>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.TaskId, e.TaskTemplateId })
                    .HasName("PRIMARY");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }


    public static class CrmDbContextExtention
    {
        public static DIHelper AddCRMDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<CrmDbContext>();
        }
    }
}