using System;
using System.Linq.Expressions;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbSubtask : ISearchItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ResponsibleId { get; set; }
        public int TaskId { get; set; }
        public int? Status { get; set; }
        public DateTime StatusChanged { get; set; }
        public int TenantId { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }

        public string IndexName => "projects_subtasks";

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new object[] { Title, TaskId };
        }
    }

    public static class SubtasksExtension
    {
        public static ModelBuilderWrapper AddSubtasks(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddSubtasks, Provider.MySql)
                .Add(PgSqlAddSubtasks, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddSubtasks(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbSubtask>(entity =>
            {
                entity.HasKey(e => e.Id)
                       .HasName("PRIMARY");

                entity.ToTable("projects_subtasks");

                entity
                .Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ResponsibleId)
                   .HasColumnName("responsible_id")
                   .HasColumnType("char(38)")
                   .HasCharSet("utf8")
                   .UseCollation("utf8_general_ci");

                entity
                .Property(e => e.TaskId)
                .HasColumnName("task_id");

                entity
                .Property(e => e.Status)
                .HasColumnName("status");

                entity.Property(e => e.StatusChanged)
                   .HasColumnName("status_changed")
                   .HasColumnType("datetime");

                entity
                .Property(e => e.TenantId)
                .HasColumnName("tenant_id");

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
        public static void PgSqlAddSubtasks(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
