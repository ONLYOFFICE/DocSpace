
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbCrmToProject
    {
        public int ProjectId { get; set; }
        public int ContactId { get; set; }
        public int TenantId { get; set; }
    }

    public static class CrmToProjectExtension
    {
        public static ModelBuilderWrapper AddCrmToProject(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddCrmToProject, Provider.MySql)
                .Add(PgSqlAddCrmToProject, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddCrmToProject(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbCrmToProject>(entity =>
            {
                entity.HasKey(e => new { e.ProjectId, e.ContactId, e.TenantId })
                    .HasName("PRIMARY");

                entity.ToTable("crm_projects");

                entity
                .Property(e => e.ProjectId)
                .HasColumnName("project_id");

                entity
               .Property(e => e.ContactId)
               .HasColumnName("contact_id");

                entity
               .Property(e => e.TenantId)
               .HasColumnName("tenant_id");

            });
        }
        public static void PgSqlAddCrmToProject(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
