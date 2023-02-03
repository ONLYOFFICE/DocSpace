// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace ASC.Core.Common.EF;

public class UserGroup : BaseEntity, IMapFrom<UserGroupRef>
{
    public int TenantId { get; set; }
    public Guid Userid { get; set; }
    public Guid UserGroupId { get; set; }
    public UserGroupRefType RefType { get; set; }
    public bool Removed { get; set; }
    public DateTime LastModified { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { TenantId, Userid, UserGroupId, RefType };
    }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<UserGroupRef, UserGroup>()
            .ForMember(dest => dest.UserGroupId, opt => opt.MapFrom(src => src.GroupId));
    }
}

public static class DbUserGroupExtension
{
    public static ModelBuilderWrapper AddUserGroup(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<UserGroup>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddUserGroup, Provider.MySql)
            .Add(PgSqlAddUserGroup, Provider.PostgreSql)
            .HasData(
            new UserGroup
            {
                TenantId = 1,
                Userid = Guid.Parse("66faa6e4-f133-11ea-b126-00ffeec8b4ef"),
                UserGroupId = Guid.Parse("cd84e66b-b803-40fc-99f9-b2969a54a1de"),
                RefType = 0,
                LastModified = new DateTime(2022, 7, 8)
            }
            );

        return modelBuilder;
    }

    public static void MySqlAddUserGroup(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Userid, e.UserGroupId, e.RefType })
                .HasName("PRIMARY");

            entity.ToTable("core_usergroup")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified");

            entity.Property(e => e.TenantId).HasColumnName("tenant");

            entity.Property(e => e.Userid)
                .HasColumnName("userid")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.UserGroupId)
                .HasColumnName("groupid")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.RefType).HasColumnName("ref_type");

            entity.Property(e => e.LastModified)
                .HasColumnName("last_modified")
                .HasColumnType("timestamp");

            entity.Property(e => e.Removed)
            .HasColumnName("removed")
            .HasColumnType("tinyint(1)")
            .HasDefaultValueSql("'0'");
        });
    }
    public static void PgSqlAddUserGroup(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.Userid, e.UserGroupId, e.RefType })
                .HasName("core_usergroup_pkey");

            entity.ToTable("core_usergroup", "onlyoffice");

            entity.HasIndex(e => e.LastModified)
                .HasDatabaseName("last_modified_core_usergroup");

            entity.Property(e => e.TenantId).HasColumnName("tenant");

            entity.Property(e => e.Userid)
                .HasColumnName("userid")
                .HasMaxLength(38);

            entity.Property(e => e.UserGroupId)
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
