using System;
using System.Linq.Expressions;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbMilestone : ISearchItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string ResponsibleId { get; set; }
        public int Status { get; set; }
        public DateTime StatusChanged { get; set; }
        public int ProjectId { get; set; }
        public int TenantId { get; set; }
        public int IsNotify { get; set; }
        public int IsKey { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }

        public string IndexName => "projects_milestones";

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Title, Description };
        }
    }

    public static class MilestoneExtension
    {
        public static ModelBuilderWrapper AddMilestone(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddMilestone, Provider.MySql)
                .Add(PgSqlAddMilestone, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddMilestone(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbMilestone>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PRIMARY");

                entity.ToTable("projects_milestones");

                entity.Property(e => e.Id)
                 .HasColumnName("id")
                 .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

                entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

                entity.Property(e => e.Deadline)
                .HasColumnName("deadline")
                .HasColumnType("datetime");


                entity.Property(e => e.ResponsibleId)
                .HasColumnName("responsible_id")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StatusChanged)
                .HasColumnName("status_changed")
                .HasColumnType("datetime");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.IsNotify).HasColumnName("is_notify");

                entity.Property(e => e.IsKey).HasColumnName("is_key");

                entity.Property(e => e.CreateBy)
                .HasColumnName("create_by")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastModifiedOn)
                    .HasColumnName("last_modified_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastModifiedBy)
                .HasColumnName("last_modified_by")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            });
        }
        public static void PgSqlAddMilestone(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
