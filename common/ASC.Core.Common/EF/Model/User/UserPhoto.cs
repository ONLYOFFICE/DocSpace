using ASC.Core.Common.EF.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASC.Core.Common.EF
{
    [Table("core_userphoto")]
    public class UserPhoto : BaseEntity
    {
        public int Tenant { get; set; }

        [Key]
        public Guid UserId { get; set; }

        public byte[] Photo { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { UserId };
        }
    }
    public static class UserPhotoExtension
    {
        public static ModelBuilderWrapper AddUserPhoto(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddUserPhoto, Provider.MySql)
                .Add(PgSqlAddUserPhoto, Provider.Postgre);
            return modelBuilder;
        }
        public static void MySqlAddUserPhoto(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<UserPhoto>(entity =>
            {
                _ = entity.HasKey(e => e.UserId)
                    .HasName("PRIMARY");

                _ = entity.ToTable("core_userphoto");

                _ = entity.HasIndex(e => e.Tenant)
                    .HasName("tenant");

                _ = entity.Property(e => e.UserId)
                    .HasColumnName("userid")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.Photo)
                    .IsRequired()
                    .HasColumnName("photo")
                    .HasColumnType("mediumblob");

                _ = entity.Property(e => e.Tenant).HasColumnName("tenant");
            });
        }
        public static void PgSqlAddUserPhoto(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<UserPhoto>(entity =>
            {
                _ = entity.HasKey(e => e.UserId)
                    .HasName("core_userphoto_pkey");

                _ = entity.ToTable("core_userphoto", "onlyoffice");

                _ = entity.HasIndex(e => e.Tenant)
                    .HasName("tenant_core_userphoto");

                _ = entity.Property(e => e.UserId)
                    .HasColumnName("userid")
                    .HasMaxLength(38);

                _ = entity.Property(e => e.Photo)
                    .IsRequired()
                    .HasColumnName("photo");

                _ = entity.Property(e => e.Tenant).HasColumnName("tenant");
            });
        }
    }
}
