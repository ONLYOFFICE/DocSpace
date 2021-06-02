using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.CRM.Core.Enums;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;

using Nest;

namespace ASC.CRM.Core.EF
{
    [ElasticsearchType(RelationName = "crm_contact")]
    [Table("crm_contact")]
    public partial class DbContact : IDbCrm, ISearchItem
    {
        public int Id { get; set; }

        [Column("is_company")]
        public bool IsCompany { get; set; }

        public string Notes { get; set; }

        [Text(Analyzer = "whitespacecustom")]
        public string Title { get; set; }

        [Column("first_name", TypeName = "varchar(255)")]
        public string FirstName { get; set; }

        [Column("last_name", TypeName = "varchar(255)")]
        public string LastName { get; set; }

        [Column("company_name", TypeName = "varchar(255)")]
        public string CompanyName { get; set; }

        [Column("industry", TypeName = "varchar(255)")]
        public string Industry { get; set; }

        [Column("status_id", TypeName = "int(11)")]
        public int StatusId { get; set; }

        [Column("company_id", TypeName = "int(11)")]
        public int CompanyId { get; set; }

        [Column("contact_type_id", TypeName = "int(11)")]
        public int ContactTypeId { get; set; }

        [Required]
        [Column("create_by", TypeName = "char(38)")]
        public Guid CreateBy { get; set; }

        [Column("create_on", TypeName = "datetime")]
        public DateTime CreateOn { get; set; }

        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }

        [Column("last_modifed_by", TypeName = "char(38)")]
        public Guid LastModifedBy { get; set; }

        [Column("display_name", TypeName = "varchar(255)")]
        public string DisplayName { get; set; }

        [Column("is_shared", TypeName = "tinyint(4)")]
        public ShareType? IsShared { get; set; }

        [Column("currency", TypeName = "varchar(3)")]
        public string Currency { get; set; }

        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get
            {
                return "crm_contact";
            }
        }

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Title };
        }
    }

    public static class DbContactExtension
    {
        public static ModelBuilderWrapper AddDbContact(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbContact, Provider.MySql)
                .Add(PgSqlAddDbContact, Provider.Postgre);

            return modelBuilder;
        }

        private static void MySqlAddDbContact(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbContact>(entity =>
            {
                entity.HasIndex(e => e.CreateOn)
                    .HasDatabaseName("create_on");

                entity.HasIndex(e => new { e.LastModifedOn, e.TenantId })
                    .HasDatabaseName("last_modifed_on");

                entity.HasIndex(e => new { e.TenantId, e.CompanyId })
                    .HasDatabaseName("company_id");

                entity.HasIndex(e => new { e.TenantId, e.DisplayName })
                    .HasDatabaseName("display_name");

                entity.Property(e => e.CompanyName)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Currency)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.DisplayName)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.FirstName)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Industry)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastName)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Notes)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }

        private static void PgSqlAddDbContact(this ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }

    }
}