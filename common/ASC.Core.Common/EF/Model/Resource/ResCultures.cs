using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF.Model.Resource
{
    [Table("res_cultures")]
    public class ResCultures
    {
        [Key]
        public string Title { get; set; }
        public string Value { get; set; }
        public bool Available { get; set; }
        public DateTime CreationDate { get; set; }
    }
    public static class ResCulturesExtension
    {
        public static ModelBuilderWrapper AddResCultures(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddResCultures, Provider.MySql)
                .Add(PgSqlAddResCultures, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddResCultures(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<ResCultures>(entity =>
            {
                _ = entity.HasKey(e => e.Title)
                    .HasName("PRIMARY");

                _ = entity.ToTable("res_cultures");

                _ = entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("varchar(120)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Available).HasColumnName("available");

                _ = entity.Property(e => e.CreationDate)
                    .HasColumnName("creationDate")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                _ = entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasColumnType("varchar(120)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddResCultures(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<ResCultures>(entity =>
            {
                _ = entity.HasKey(e => e.Title)
                    .HasName("res_cultures_pkey");

                _ = entity.ToTable("res_cultures", "onlyoffice");

                _ = entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("character varying");

                _ = entity.Property(e => e.Available)
                    .HasColumnName("available")
                    .HasDefaultValueSql("'0'");

                _ = entity.Property(e => e.CreationDate)
                    .HasColumnName("creationDate")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                _ = entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasColumnType("character varying");
            });
        }
    }
}
