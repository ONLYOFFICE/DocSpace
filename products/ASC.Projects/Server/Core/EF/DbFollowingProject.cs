
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbFollowingProject
    {
        public int ProjectId { get; set; }
        public string ParticipantId { get; set; }
    }

    public static class ProjectToParticipantExtension
    {
        public static ModelBuilderWrapper AddProjectToParticipant(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddProjectToParticipant, Provider.MySql)
                .Add(PgSqlAddProjectToParticipant, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddProjectToParticipant(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbFollowingProject>(entity =>
            {
                entity.HasKey(e => new { e.ProjectId, e.ParticipantId })
                    .HasName("PRIMARY");

                entity.ToTable("projects_following_project_participant");

                entity
                .Property(e => e.ProjectId)
                .HasColumnName("project_id");

                entity
                .Property(e => e.ParticipantId)
                .HasColumnName("participant_id")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            });
        }
        public static void PgSqlAddProjectToParticipant(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
