using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Projects.EF
{
    public class DbParticipant
    {
        public int ProjectId { get; set; }
        public string ParticipantId { get; set; }
        public int? Security { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public int Removed { get; set; }
        public int Tenant { get; set; }
    }

    public static class ParticipantExtension
    {
        public static ModelBuilderWrapper AddParticipant(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddParticipant, Provider.MySql)
                .Add(PgSqlAddParticipant, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddParticipant(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbParticipant>(entity =>
            {
                entity
               .HasKey(e => e.ProjectId)
               .HasName("PRIMARY");

                entity
               .HasKey(e => e.ParticipantId)
               .HasName("PRIMARY");

                entity
               .HasKey(e => e.Tenant)
               .HasName("PRIMARY");

                entity.ToTable("projects_project_participant");

                entity
                .Property(e => e.ProjectId)
                .HasColumnName("project_id");

                entity.Property(e => e.ParticipantId)
                .HasColumnName("participant_id")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

                entity
                .Property(e => e.Security)
                .HasColumnName("security");

                entity.Property(e => e.Created)
                    .HasColumnName("created")
                    .HasColumnType("TIMESTAMP");

                entity.Property(e => e.Updated)
                    .HasColumnName("updated")
                    .HasColumnType("TIMESTAMP");

                entity
                .Property(e => e.Removed)
                .HasColumnName("removed");

                entity
                .Property(e => e.Tenant)
                .HasColumnName("tenant");
            });
        }
        public static void PgSqlAddParticipant(this ModelBuilder modelBuilder)
        {
            //todo
        }
    }
}
