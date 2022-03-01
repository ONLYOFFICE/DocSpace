namespace ASC.Core.Common.EF;

public class UserGroup : BaseEntity, IMapFrom<UserGroupRef>
{
    public int Tenant { get; set; }
    public Guid UserId { get; set; }
    public Guid GroupId { get; set; }
    public UserGroupRefType RefType { get; set; }
    public bool Removed { get; set; }
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
        modelBuilder
            .Add(MySqlAddUserGroup, Provider.MySql)
            .Add(PgSqlAddUserGroup, Provider.PostgreSql)
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
        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => new { e.Tenant, e.UserId, e.GroupId, e.RefType })
                .HasName("PRIMARY");

            entity.ToTable("core_usergroup");

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified");

            entity.Property(e => e.Tenant).HasColumnName("tenant");

            entity.Property(e => e.UserId)
                .HasColumnName("userid")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.GroupId)
                .HasColumnName("groupid")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.RefType).HasColumnName("ref_type");

            entity.Property(e => e.LastModified)
                .HasColumnName("last_modified")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.Removed).HasColumnName("removed");
        });
    }
    public static void PgSqlAddUserGroup(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => new { e.Tenant, e.UserId, e.GroupId, e.RefType })
                .HasName("core_usergroup_pkey");

            entity.ToTable("core_usergroup", "onlyoffice");

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified_core_usergroup");

            entity.Property(e => e.Tenant).HasColumnName("tenant");

            entity.Property(e => e.UserId)
                .HasColumnName("userid")
                .HasMaxLength(38);

            entity.Property(e => e.GroupId)
                .HasColumnName("groupid")
                .HasMaxLength(38);

            entity.Property(e => e.RefType).HasColumnName("ref_type");

            entity.Property(e => e.LastModified)
                .HasColumnName("last_modified")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.Removed).HasColumnName("removed");
        });

    }
}
