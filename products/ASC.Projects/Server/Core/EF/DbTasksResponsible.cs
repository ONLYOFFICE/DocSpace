
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbTasksResponsible
    {
        public int TenantId { get; set; }
        public int? TaskId { get; set; }
        public string ResponsibleId { get; set; }
    }

    public static class TasksToResponsibleExtension
    {
        public static ModelBuilderWrapper AddTasksToResponsible(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddTasksToResponsible, Provider.MySql)
                .Add(PgSqlAddTasksToResponsible, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddTasksToResponsible(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTasksResponsible>(entity =>
            {
                entity.ToTable("projects_tasks_responsible");

                entity
                .Property(e => e.ResponsibleId)
                .HasColumnName("responsible_id")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

                entity
               .Property(e => e.TaskId)
               .HasColumnName("task_id");

                entity
               .Property(e => e.TenantId)
               .HasColumnName("tenant_id");

            });
        }
        public static void PgSqlAddTasksToResponsible(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
