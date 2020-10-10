using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Resource
{
    [Table("res_files")]
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
            _ = modelBuilder
                .Add(MySqlAddResFiles, Provider.MySql)
                .Add(PgSqlAddResFiles, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddResFiles(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<ResFiles>(entity =>
            {
                _ = entity.ToTable("res_files");

                _ = entity.HasIndex(e => e.ResName)
                    .HasName("resname")
                    .IsUnique();

                _ = entity.Property(e => e.Id).HasColumnName("id");

                _ = entity.Property(e => e.CreationDate)
                    .HasColumnName("creationDate")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("'0000-00-00 00:00:00'");

                _ = entity.Property(e => e.IsLock).HasColumnName("isLock");

                _ = entity.Property(e => e.LastUpdate)
                    .HasColumnName("lastUpdate")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .ValueGeneratedOnAddOrUpdate();

                _ = entity.Property(e => e.ModuleName)
                    .IsRequired()
                    .HasColumnName("moduleName")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasColumnName("projectName")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.ResName)
                    .IsRequired()
                    .HasColumnName("resName")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddResFiles(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<ResFiles>(entity =>
            {
                _ = entity.ToTable("res_files", "onlyoffice");

                _ = entity.HasIndex(e => e.ResName)
                    .HasName("resname")
                    .IsUnique();

                _ = entity.Property(e => e.Id).HasColumnName("id");

                _ = entity.Property(e => e.CreationDate)
                    .HasColumnName("creationDate")
                    .HasDefaultValueSql("'1975-03-03 00:00:00'");

                _ = entity.Property(e => e.IsLock)
                    .HasColumnName("isLock")
                    .HasDefaultValueSql("'0'");

                _ = entity.Property(e => e.LastUpdate)
                    .HasColumnName("lastUpdate")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                _ = entity.Property(e => e.ModuleName)
                    .IsRequired()
                    .HasColumnName("moduleName")
                    .HasMaxLength(50);

                _ = entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasColumnName("projectName")
                    .HasMaxLength(50);

                _ = entity.Property(e => e.ResName)
                    .IsRequired()
                    .HasColumnName("resName")
                    .HasMaxLength(50);
            });
        }
    }
}
