
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbTasksOrder
    {
        public int TenantId { get; set; }
        public int ProjectId { get; set; }
        public string TaskOrder { get; set; }
    }

    public static class TasksOrderExtension
    {
        public static ModelBuilderWrapper AddTasksOrder(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddTasksOrder, Provider.MySql)
                .Add(PgSqlAddTasksOrder, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddTasksOrder(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTasksOrder>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.ProjectId })
                    .HasName("PRIMARY");

                entity.ToTable("projects_tasks_order");

                entity
                .Property(e => e.TenantId)
                .HasColumnName("tenant_id");

                entity
                .Property(e => e.ProjectId)
                .HasColumnName("project_id");

                entity.Property(e => e.TaskOrder)
                    .HasColumnName("task_order")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddTasksOrder(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
