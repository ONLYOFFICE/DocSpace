using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbReportTemplate
    {
        public int? Id { get; set; }
        public int? Type { get; set; }
        public string Name { get; set; }
        public string Filter { get; set; }
        public string Cron { get; set; }
        public DateTime CreateOn { get; set; }
        public string CreateBy { get; set; }
        public int? TenantId { get; set; }
        public int? Auto { get; set; }
    }

    public static class ReportTemplateExtension
    {
        public static ModelBuilderWrapper AddReportTemplate(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddReportTemplate, Provider.MySql)
                .Add(PgSqlAddReportTemplate, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddReportTemplate(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbReportTemplate>(entity =>
            {
                entity.ToTable("projects_report_template");

                entity
                .Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

                entity
                .Property(e => e.Type)
                .HasColumnName("type");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(1024)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Filter)
                    .HasColumnName("filter")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Cron)
                    .HasColumnName("cron")
                    .HasColumnType("varchar(255)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("timespamp");

                entity.Property(e => e.CreateBy)
                    .HasColumnName("create_by")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity
                .Property(e => e.TenantId)
                .HasColumnName("tenant_id");

                entity
                .Property(e => e.Auto)
                .HasColumnName("auto");
            });
        }
        public static void PgSqlAddReportTemplate(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
