using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbReport
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public int FileId { get; set; }
        public DateTime CreateOn { get; set; }
        public string CreateBy { get; set; }
        public int TenantId { get; set; }
    }

    public static class ReportExtension
    {
        public static ModelBuilderWrapper AddReport(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddReport, Provider.MySql)
                .Add(PgSqlAddReport, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddReport(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbReport>(entity =>
            {
                entity.HasKey(e => e.Id)
                       .HasName("PRIMARY");

                entity.ToTable("projects_reports");

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

                entity
                .Property(e => e.FileId)
                .HasColumnName("fileId");

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

            });
        }
        public static void PgSqlAddReport(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
