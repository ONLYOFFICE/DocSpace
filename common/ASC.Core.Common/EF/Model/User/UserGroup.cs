using System;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    [Table("core_usergroup")]
    public class UserGroup : BaseEntity
    {
        public int Tenant { get; set; }

        public Guid UserId { get; set; }

        public Guid GroupId { get; set; }

        [Column("ref_type")]
        public UserGroupRefType RefType { get; set; }

        public bool Removed { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { Tenant, UserId, GroupId, RefType };
        }
    }

    public static class DbUserGroupExtension
    {
        public static ModelBuilderWrapper AddUserGroup(this ModelBuilderWrapper modelBuilder)
        {
            _ = modelBuilder
                .Add(MySqlAddUserGroup, Provider.MySql)
                .Add(PgSqlAddUserGroup, Provider.Postgre)
                .HasData(
                new UserGroup
                {
                    Tenant = 1,
                    UserId = Guid.Parse("66faa6e4-f133-11ea-b126-00ffeec8b4ef"),
                    GroupId = Guid.Parse("cd84e66b-b803-40fc-99f9-b2969a54a1de"),
                    RefType = 0
        }
                );

            return modelBuilder;
    }

        public static void MySqlAddUserGroup(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<UserGroup>(entity =>
            {
                _ = entity.HasKey(e => new { e.Tenant, e.UserId, e.GroupId, e.RefType })
                    .HasName("PRIMARY");

                _ = entity.ToTable("core_usergroup");

                _ = entity.HasIndex(e => e.LastModified)
                    .HasName("last_modified");

                _ = entity.Property(e => e.Tenant).HasColumnName("tenant");

                _ = entity.Property(e => e.UserId)
                    .HasColumnName("userid")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.GroupId)
                    .HasColumnName("groupid")
                    .HasColumnType("varchar(38)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                _ = entity.Property(e => e.RefType).HasColumnName("ref_type");

                _ = entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                _ = entity.Property(e => e.Removed).HasColumnName("removed");
            });
        }
        public static void PgSqlAddUserGroup(this ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<UserGroup>(entity =>
            {
                _ = entity.HasKey(e => new { e.Tenant, e.UserId, e.GroupId, e.RefType })
                    .HasName("core_usergroup_pkey");

                _ = entity.ToTable("core_usergroup", "onlyoffice");

                _ = entity.HasIndex(e => e.LastModified)
                    .HasName("last_modified_core_usergroup");

                _ = entity.Property(e => e.Tenant).HasColumnName("tenant");

                _ = entity.Property(e => e.UserId)
                    .HasColumnName("userid")
                    .HasMaxLength(38);

                _ = entity.Property(e => e.GroupId)
                    .HasColumnName("groupid")
                    .HasMaxLength(38);

                _ = entity.Property(e => e.RefType).HasColumnName("ref_type");

                _ = entity.Property(e => e.LastModified)
                    .HasColumnName("last_modified")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                _ = entity.Property(e => e.Removed).HasColumnName("removed");
            });

        }
    }
}
