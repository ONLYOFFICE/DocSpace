
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbTagToProject
    {
        public int? TagId { get; set; }
        public int ProjectId { get; set; }
    }

    public static class TagToProjectExtension
    {
        public static ModelBuilderWrapper AddTagToProject(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddTagToProject, Provider.MySql)
                .Add(PgSqlAddTagToProject, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddTagToProject(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbTagToProject>(entity =>
            {
                entity.ToTable("projects_project_tag");

                entity
                .Property(e => e.TagId)
                .HasColumnName("tag_id");

                entity
                .Property(e => e.ProjectId)
                .HasColumnName("project_id");

            });
        }
        public static void PgSqlAddTagToProject(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
