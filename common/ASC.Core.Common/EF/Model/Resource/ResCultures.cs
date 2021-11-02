using System;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model.Resource
{
    public class ResCultures
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public bool Available { get; set; }
        public DateTime CreationDate { get; set; }
    }
    public static class ResCulturesExtension
    {
        public static ModelBuilderWrapper AddResCultures(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddResCultures, Provider.MySql)
                .Add(PgSqlAddResCultures, Provider.PostgreSql);
            return modelBuilder;
        }
        public static void MySqlAddResCultures(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResCultures>(entity =>
            {
                entity.HasKey(e => e.Title)
                    .HasName("PRIMARY");

                entity.ToTable("res_cultures");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("varchar(120)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.Available).HasColumnName("available");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationDate")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasColumnType("varchar(120)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");
            });
        }
        public static void PgSqlAddResCultures(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResCultures>(entity =>
            {
                entity.HasKey(e => e.Title)
                    .HasName("res_cultures_pkey");

                entity.ToTable("res_cultures", "onlyoffice");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasColumnType("character varying");

                entity.Property(e => e.Available)
                    .HasColumnName("available")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.CreationDate)
                    .HasColumnName("creationDate")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasColumnType("character varying");
            });
        }
    }
}
