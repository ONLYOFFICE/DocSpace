
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbLink
    {
        public int TenantId{ get; set; }
        public int TaskId { get; set; }
        public int ParentId{ get; set; }
        public int LinkType{ get; set; }    
    }

    public static class LinkExtension
    {
        public static ModelBuilderWrapper AddLink(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddLink, Provider.MySql)
                .Add(PgSqlAddLink, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddLink(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbLink>(entity =>
            {
                entity.ToTable("projects_tasks_links");

                entity
                .Property(e => e.TenantId)
                .HasColumnName("tenant_id");

                entity
                .Property(e => e.TaskId)
                .HasColumnName("task_id");

                entity
                .Property(e => e.ParentId)
                .HasColumnName("parent_id");

                entity
                .Property(e => e.LinkType)
                .HasColumnName("link_type");

            });
        }
        public static void PgSqlAddLink(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
