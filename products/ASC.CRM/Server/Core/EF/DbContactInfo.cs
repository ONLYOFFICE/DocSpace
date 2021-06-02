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
    [ElasticsearchType(RelationName = "crm_contact_info")]
    [Table("crm_contact_info")]
    public sealed class DbContactInfo : IDbCrm, ISearchItem
    {
        public int Id { get; set; }

        [Required]
        [Text(Analyzer = "whitespacecustom")]
        public string Data { get; set; }

        [Column("category", TypeName = "int(255)")]
        public int Category { get; set; }

        [Column("tenant_id", TypeName = "int(255)")]
        public int TenantId { get; set; }

        [Column("is_primary", TypeName = "tinyint(4)")]
        public bool IsPrimary { get; set; }

        [Column("contact_id", TypeName = "int(11)")]
        public int ContactId { get; set; }

        [Column("type", TypeName = "int(255)")]
        public ContactInfoType Type { get; set; }

        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }

        [Column("last_modifed_by", TypeName = "char(38)")]
        public Guid LastModifedBy { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get
            {
                return "crm_field_value";
            }
        }

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Data };
        }
    }

    public static class DbContactInfoExtension
    {
        public static ModelBuilderWrapper AddDbContactInfo(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbContactInfo, Provider.MySql);

            return modelBuilder;
        }

        private static void MySqlAddDbContactInfo(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbContactInfo>(entity =>
            {
                entity.HasIndex(e => e.LastModifedOn)
                    .HasDatabaseName("last_modifed_on");

                entity.HasIndex(e => new { e.TenantId, e.ContactId })
                    .HasDatabaseName("IX_Contact");

                entity.Property(e => e.Data)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
    }




}