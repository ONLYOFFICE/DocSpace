using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.CRM.Core.Enums;
using ASC.ElasticSearch;

using Microsoft.EntityFrameworkCore;

using Nest;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace ASC.CRM.Core.EF
{
    [ElasticsearchType(RelationName = "crm_task")]
    [Table("crm_task")]
    public partial class DbTask: IDbCrm, ISearchItem
    {
        public int Id { get; set; }
        
        [Required]
        [Text(Analyzer = "whitespacecustom")]
        public string Title { get; set; }

        [Text(Analyzer = "whitespacecustom")]
        public string Description { get; set; }

        [Column("deadline", TypeName = "datetime")]
        public DateTime Deadline { get; set; }

        [Required]
        [Column("responsible_id", TypeName = "char(38)")]
        public Guid ResponsibleId { get; set; }

        [Column("contact_id", TypeName = "int(11)")]
        public int ContactId { get; set; }

        [Column("is_closed", TypeName = "int(1)")]
        public bool IsClosed { get; set; }

        [Column("tenant_id", TypeName = "int(11)")]
        public int TenantId { get; set; }

        [Column("entity_type", TypeName = "int(11)")]
        public EntityType EntityType { get; set; }

        [Column("entity_id", TypeName = "int(11)")]
        public int EntityId { get; set; }

        [Column("category_id", TypeName = "int(11)")]
        public int CategoryId { get; set; }

        [Column("create_on", TypeName = "datetime")]
        public DateTime CreateOn { get; set; }

        [Required]
        [Column("create_by", TypeName = "char(38)")]
        public Guid CreateBy { get; set; }

        [Column("last_modifed_on", TypeName = "datetime")]
        public DateTime? LastModifedOn { get; set; }

        [Column("last_modifed_by", TypeName = "char(38)")]
        public Guid? LastModifedBy { get; set; }

        [Column("alert_value", TypeName = "int(10)")]
        public int AlertValue { get; set; }

        [Column("exec_alert", TypeName = "int(10)")]
        public int ExecAlert { get; set; }

        [NotMapped]
        [Ignore]
        public string IndexName
        {
            get => "crm_task";
        }

        [Ignore]
        public Expression<Func<ISearchItem, object[]>> SearchContentFields
        {
            get
            {
                return (a) => new[] { Title, Description };
            }
        }
    }

    public static class DbTaskExtension
    {
        public static ModelBuilderWrapper AddDbTask(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddDbTask, Provider.MySql);

            return modelBuilder;
        }

        public static void MySqlAddDbTask(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTask>(entity =>
            {
                entity.ToTable("crm_task");

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
        }
    }
}