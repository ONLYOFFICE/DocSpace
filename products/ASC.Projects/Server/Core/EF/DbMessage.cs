
using System;
using System.Linq.Expressions;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;

using Nest;

namespace ASC.Projects.EF
{
    public class DbMessage : ISearchItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Status { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public string Content { get; set; }
        public int ProjectId { get; set; }
        public int TenantId { get; set; }

        [Nested]
        public string IndexName => "projects_messages";

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new[] { Title, Content };
        }
    }

    public static class MessageExtension
    {
        public static ModelBuilderWrapper AddMessage(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddMessage, Provider.MySql)
                .Add(PgSqlAddMessage, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddMessage(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbMessage>(entity =>
            {
                entity.ToTable("projects_messages");

                entity.Property(e => e.Id)
                 .HasColumnName("id")
                 .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

                entity.Property(e => e.Status).HasColumnName("status");

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

                entity.Property(e => e.Content)
               .HasColumnName("content")
               .HasColumnType("mediumtext")
               .HasCharSet("utf8")
               .UseCollation("utf8_general_ci");

                entity.Property(e => e.ProjectId).HasColumnName("project_id");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            });
        }
        public static void PgSqlAddMessage(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
