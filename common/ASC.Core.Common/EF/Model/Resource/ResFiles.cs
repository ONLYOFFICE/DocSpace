using System;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model.Resource
{
    public class ResFiles
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ModuleName { get; set; }
        public string ResName { get; set; }
        public bool IsLock { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime CreationDate { get; set; }
    }
    public static class ResFilesExtension
    {
        public static ModelBuilderWrapper AddResFiles(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddResFiles, Provider.MySql)
                .Add(PgSqlAddResFiles, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddResFiles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResFiles>(entity =>
            {
                entity.ToTable("res_files");

                entity.HasIndex(e => e.ResName)
                    .HasDatabaseName("resname")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationDate")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                entity.Property(e => e.IsLock).HasColumnName("isLock");

                entity.Property(e => e.LastUpdate)
                    .HasColumnName("lastUpdate")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ModuleName)
                    .IsRequired()
                    .HasColumnName("moduleName")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasColumnName("projectName")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.ResName)
                    .IsRequired()
                    .HasColumnName("resName")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddResFiles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResFiles>(entity =>
            {
                entity.ToTable("res_files", "onlyoffice");

                entity.HasIndex(e => e.ResName)
                    .HasDatabaseName("resname")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationDate")
                    .HasDefaultValueSql("'1975-03-03 00:00:00'");

                entity.Property(e => e.IsLock)
                    .HasColumnName("isLock")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.LastUpdate)
                    .HasColumnName("lastUpdate")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.ModuleName)
                    .IsRequired()
                    .HasColumnName("moduleName")
                    .HasMaxLength(50);

                entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasColumnName("projectName")
                    .HasMaxLength(50);

                entity.Property(e => e.ResName)
                    .IsRequired()
                    .HasColumnName("resName")
                    .HasMaxLength(50);
            });
        }
    }
}
