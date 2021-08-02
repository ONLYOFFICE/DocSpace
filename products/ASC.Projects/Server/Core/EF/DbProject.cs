using System;
using System.Linq.Expressions;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbProject : ISearchItem
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public DateTime StatusChanged { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ResponsibleId { get; set; }
        public int TenantId { get; set; }
        public int Private { get; set; }
        public DateTime CreateOn { get; set; }
        public string CreateBy { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }

        public string IndexName => "projects_projects";

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Title, Description };
        }
    }

    public static class ProjectExtension
    {
        public static ModelBuilderWrapper AddProject(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddProject, Provider.MySql)
                .Add(PgSqlAddProject, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddProject(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbProject>(entity =>
            {
                entity.ToTable("projects_projects");

                entity.Property(e => e.Id)
                 .HasColumnName("id")
                 .ValueGeneratedOnAdd();

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.StatusChanged)
                    .HasColumnName("status_changed")
                    .HasColumnType("datetime");

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

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.Private).HasColumnName("private");

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
        public static void PgSqlAddProject(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
