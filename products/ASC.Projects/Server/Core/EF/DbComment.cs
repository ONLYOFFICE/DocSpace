using System;
using System.Linq.Expressions;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;
using ASC.ElasticSearch;
using ASC.ElasticSearch.Core;

using Microsoft.EntityFrameworkCore;


namespace ASC.Projects.EF
{
    public class DbComment : ISearchItem
    {
        public int CommentId { get; set; }
        public string Id { get; set; }
        public string Content { get; set; }
        public int InActive { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateOn { get; set; }
        public string ParentId { get; set; }
        public int TenantId { get; set; }
        public string TargetUniqId { get; set; }

        public string IndexName => "projects_comments";

        int ISearchItem.Id { get => CommentId; set => CommentId = value; }

        public Expression<Func<ISearchItem, object[]>> GetSearchContentFields(SearchSettingsHelper searchSettings)
        {
            return (a) => new object[] { CommentId, Content, TargetUniqId, InActive, CreateOn };
        }
    }

    public static class ProjectsCommentsExtension
    {
        public static ModelBuilderWrapper AddComments(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddProjectsComments, Provider.MySql)
                .Add(PgSqlAddProjectsComments, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddProjectsComments(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbComment>(entity =>
            {
                entity.ToTable("projects_comments");

                entity
                .Property(e => e.CommentId)
                .HasColumnName("comment_id")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Content)
                    .HasColumnName("content")
                    .HasColumnType("text")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.InActive).HasColumnName("inactive");

                entity.Property(e => e.CreateBy)
                    .HasColumnName("create_by")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.CreateOn)
                    .HasColumnName("create_on")
                    .HasColumnType("datetime");

                entity.Property(e => e.ParentId)
                    .HasColumnName("parent_id")
                    .HasColumnType("char(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.TenantId).HasColumnName("tenant_id");

                entity.Property(e => e.TargetUniqId)
                    .HasColumnName("target_uniq_id")
                    .HasColumnType("VARCHAR(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddProjectsComments(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
