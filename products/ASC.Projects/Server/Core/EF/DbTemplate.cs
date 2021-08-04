using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbTemplate
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public int TenantId { get; set; }
    }

    public static class TemplateExtension
    {
        public static ModelBuilderWrapper AddTemplate(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddTemplate, Provider.MySql)
                .Add(PgSqlAddTemplate, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddTemplate(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTemplate>(entity =>
            {
                entity.HasKey(e => e.Id)
                      .HasName("PRIMARY");

                entity.ToTable("projects_templates");

                entity
                .Property(e => e.Id)
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
        public static void PgSqlAddTemplate(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
