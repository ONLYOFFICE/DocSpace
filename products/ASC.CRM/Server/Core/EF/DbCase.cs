using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;

using Nest;

namespace ASC.CRM.Core.EF
{
    [Table("crm_case")]
    public class DbCase : IDbCrm, ISearchItem
    {
        public int Id { get; set; }

        [Required]
        [Text(Analyzer = "whitespacecustom")]
        public string Title { get; set; }

        [Column("is_closed")]
        public bool IsClosed { get; set; }

        [Required]
        [Column("create_by", TypeName = "char(38)")]
        public Guid CreateBy { get; set; }

        [Column("create_on", TypeName = "datetime")]
        public DateTime CreateOn { get; set; }

        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }

        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }

        [Column("last_modifed_by", TypeName = "char(38)")]
        public Guid? LastModifedBy { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get
            {
                return "crm_case";
            }
        }

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Title };
        }
    }

    public static class DbCaseExtension
    {
        public static ModelBuilderWrapper AddDbCase(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbCase, Provider.MySql);

            return modelBuilder;
        }
        private static void MySqlAddDbCase(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbCase>(entity =>
            {
                entity.HasIndex(e => e.CreateOn)
                    .HasDatabaseName("create_on");

                entity.HasIndex(e => e.LastModifedOn)
                    .HasDatabaseName("last_modifed_on");

                entity.HasIndex(e => e.TenantId)
                    .HasDatabaseName("tenant_id");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Title)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
    }

}