using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbTimeTracking
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public DateTime Date { get; set; }
        public double Hours { get; set; }
        public int TenantId { get; set; }
        public int RelativeTaskId { get; set; }
        public string PersonId { get; set; }
        public int ProjectId { get; set; }
        public DateTime? CreateOn { get; set; }
        public string CreateBy { get; set; }
        public int PaymentStatus { get; set; }
        public DateTime? StatusChanged { get; set; }
    }

    public static class TimeTrackingExtension
    {
        public static ModelBuilderWrapper AddTimeTracking(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddTimeTracking, Provider.MySql)
                .Add(PgSqlAddTimeTracking, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddTimeTracking(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTimeTracking>(entity =>
            {
                entity.HasKey(e => e.Id)
                      .HasName("PRIMARY");

                entity.ToTable("projects_time_tracking");

                entity
                .Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("datetime");

                entity
                .Property(e => e.Hours)
                .HasColumnName("hours");

                entity
                .Property(e => e.TenantId)
                .HasColumnName("tenant_id");

                entity
                .Property(e => e.RelativeTaskId)
                .HasColumnName("relative_task_id");

                entity.Property(e => e.PersonId)
                    .HasColumnName("person_id")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity
                .Property(e => e.ProjectId)
                .HasColumnName("project_id");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.CreateBy)
                    .HasColumnName("create_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity
               .Property(e => e.PaymentStatus)
               .HasColumnName("payment_status");

                entity.Property(e => e.StatusChanged)
                    .HasColumnName("status_changed")
                    .HasColumnType("datetime");

            });
        }
        public static void PgSqlAddTimeTracking(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
