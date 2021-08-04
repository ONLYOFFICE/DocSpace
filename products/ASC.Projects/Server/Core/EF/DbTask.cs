using System;
using System.Linq.Expressions;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbTask : ISearchItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ResponsibleId { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public int? StatusId { get; set; }
        public DateTime StatusChanged { get; set; }
        public int ProjectId { get; set; }
        public int? MilestoneId { get; set; }
        public int TenantId { get; set; }
        public int SortOrder { get; set; }
        public DateTime Deadline { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public DateTime StartDate { get; set; }
        public int Progress { get; set; }

        public string IndexName => "projects_tasks";

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Title, Description };
        }
    }

    public static class TaskExtension
    {
        public static ModelBuilderWrapper AddTask(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddTask, Provider.MySql)
                .Add(PgSqlAddTask, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddTask(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTask>(entity =>
            {
                entity.HasKey(e => e.Id)
                       .HasName("PRIMARY");

                entity.ToTable("projects_tasks");

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

                entity.Property(e => e.ResponsibleId)
                .HasColumnName("responsible_id")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

                entity.Property(e => e.Priority)
                 .HasColumnName("priority");

                entity.Property(e => e.Status)
                 .HasColumnName("status");

                entity.Property(e => e.StatusId)
                 .HasColumnName("status_id");

                entity.Property(e => e.StatusChanged)
                 .HasColumnName("status_changed")
                 .HasColumnType("datetime");

                entity.Property(e => e.ProjectId)
                 .HasColumnName("project_id");

                entity.Property(e => e.MilestoneId)
                 .HasColumnName("milestone_id");

                entity.Property(e => e.TenantId)
                 .HasColumnName("tenant_id");

                entity.Property(e => e.SortOrder)
                 .HasColumnName("sort_order");

                 entity.Property(e => e.Deadline)
                 .HasColumnName("deadline")
                 .HasColumnType("datetime");

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

                entity.Property(e => e.StartDate)
                .HasColumnName("start_date")
                .HasColumnType("datetime");

                entity.Property(e => e.Progress)
                 .HasColumnName("progress");
            });
        }
        public static void PgSqlAddTask(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
