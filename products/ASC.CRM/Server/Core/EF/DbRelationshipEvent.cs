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
    [ElasticsearchType(RelationName = "crm_relationship_event")]
    [Table("crm_relationship_event")]
    public class DbRelationshipEvent : IDbCrm, ISearchItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("contact_id")]
        public int ContactId { get; set; }

        [Column("content")]
        [Text(Analyzer = "whitespacecustom")]
        public string Content { get; set; }

        [Required]
        [Column("create_by", TypeName = "char(38)")]
        public Guid CreateBy { get; set; }

        [Column("create_on", TypeName = "datetime")]
        public DateTime CreateOn { get; set; }

        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }

        [Column("entity_type", TypeName = "int(11)")]
        public EntityType EntityType { get; set; }

        [Column("entity_id", TypeName = "int(11)")]
        public int EntityId { get; set; }

        [Column("category_id", TypeName = "int(11)")]
        public int CategoryId { get; set; }

        [Column("last_modifed_by", TypeName = "char(38)")]
        public Guid? LastModifedBy { get; set; }

        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }

        [Column("have_files", TypeName = "int(11)")]
        public bool HaveFiles { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get
            {
                return "crm_relationship_event";
            }
        }

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Content };
        }
    }

    public static class DbRelationshipEventExtension
    {
        public static ModelBuilderWrapper AddDbRelationshipEvent(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbRelationshipEvent, Provider.MySql)
                .Add(PgSqlAddDbRelationshipEvent, Provider.PostgreSql);

            return modelBuilder;
        }
        private static void MySqlAddDbRelationshipEvent(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbRelationshipEvent>(entity =>
            {
                entity.HasIndex(e => e.ContactId)
                    .HasDatabaseName("IX_Contact");

                entity.HasIndex(e => e.LastModifedOn)
                    .HasDatabaseName("last_modifed_on");

                entity.HasIndex(e => e.TenantId)
                    .HasDatabaseName("tenant_id");

                entity.HasIndex(e => new { e.EntityId, e.EntityType })
                    .HasDatabaseName("IX_Entity");

                entity.Property(e => e.Content)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CreateBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.LastModifedBy)
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }

        private static void PgSqlAddDbRelationshipEvent(this ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }
    }
}