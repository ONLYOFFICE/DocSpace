using System;
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    public class DbWebstudioUserVisit
    {
        public int TenantId { get; set; }
        public DateTime VisitDate { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public int VisitCount { get; set; }
        public DateTime FirstVisitTime { get; set; }
        public DateTime LastVisitTime { get; set; }
    }

    public static class WebstudioUserVisitExtension
    {
        public static ModelBuilderWrapper AddWebstudioUserVisit(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddWebstudioUserVisit, Provider.MySql)
                .Add(PgSqlAddWebstudioUserVisit, Provider.Postgre);

            return modelBuilder;
        }

        public static void MySqlAddWebstudioUserVisit(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbWebstudioUserVisit>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.VisitDate, e.ProductId, e.UserId })
                    .HasName("PRIMARY");

                entity.ToTable("webstudio_uservisit");

                entity.HasIndex(e => e.VisitDate)
                    .HasDatabaseName("visitdate");

                entity.Property(e => e.TenantId).HasColumnName("tenantid");

                entity.Property(e => e.VisitDate)
                    .HasColumnName("visitdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ProductId)
                    .HasColumnName("productid")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.UserId)
                    .HasColumnName("userid")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .UseCollation("utf8_general_ci");

                entity.Property(e => e.FirstVisitTime)
                    .HasColumnName("firstvisittime")
                    .HasColumnType("datetime");

                entity.Property(e => e.LastVisitTime)
                    .HasColumnName("lastvisittime")
                    .HasColumnType("datetime");

                entity.Property(e => e.VisitCount).HasColumnName("visitcount");
            });
        }
        public static void PgSqlAddWebstudioUserVisit(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbWebstudioUserVisit>(entity =>
            {
                entity.HasKey(e => new { e.TenantId, e.VisitDate, e.ProductId, e.UserId })
                    .HasName("webstudio_uservisit_pkey");

                entity.ToTable("webstudio_uservisit", "onlyoffice");

                entity.HasIndex(e => e.VisitDate)
                    .HasDatabaseName("visitdate");

                entity.Property(e => e.TenantId).HasColumnName("tenantid");

                entity.Property(e => e.VisitDate).HasColumnName("visitdate");

                entity.Property(e => e.ProductId)
                    .HasColumnName("productid")
                    .HasMaxLength(38);

                entity.Property(e => e.UserId)
                    .HasColumnName("userid")
                    .HasMaxLength(38);

                entity.Property(e => e.FirstVisitTime).HasColumnName("firstvisittime");

                entity.Property(e => e.LastVisitTime).HasColumnName("lastvisittime");

                entity.Property(e => e.VisitCount).HasColumnName("visitcount");
            });
        }
    }
}
