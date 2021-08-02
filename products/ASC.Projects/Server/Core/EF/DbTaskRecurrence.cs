using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbTaskRecurrence
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string Cron { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TenantId { get; set; }
    }

    public static class TaskRecurrenceExtension
    {
        public static ModelBuilderWrapper AddTaskRecurrence(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddTaskRecurrence, Provider.MySql)
                .Add(PgSqlAddTaskRecurrence, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddTaskRecurrence(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTaskRecurrence>(entity =>
            {
                entity.ToTable("projects_tasks_recurrence");

                entity
                .Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

                entity
                .Property(e => e.TaskId)
                .HasColumnName("task_id");

                entity.Property(e => e.Cron)
                    .HasColumnName("cron")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.EndDate)
                   .HasColumnName("end_date")
                   .HasColumnType("datetime");

                entity.Property(e => e.StartDate)
                   .HasColumnName("start_date")
                   .HasColumnType("datetime");

                entity
                .Property(e => e.TenantId)
                .HasColumnName("tenant_id");
            });
        }
        public static void PgSqlAddTaskRecurrence(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
