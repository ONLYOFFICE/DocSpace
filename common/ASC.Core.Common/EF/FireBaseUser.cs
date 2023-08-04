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

/// <summary>
/// </summary>
public class FireBaseUser : BaseEntity
{
    /// <summary>ID</summary>
    /// <type>System.Int32, System</type>
    public int Id { get; set; }

    /// <summary>User ID</summary>
    /// <type>System.Guid, System</type>
    public Guid UserId { get; set; }

    /// <summary>Tenant ID</summary>
    /// <type>System.Int32, System</type>
    public int TenantId { get; set; }

    /// <summary>Firebase device token</summary>
    /// <type>System.String, System</type>
    public string FirebaseDeviceToken { get; set; }

    /// <summary>Application</summary>
    /// <type>System.String, System</type>
    public string Application { get; set; }

    /// <summary>Specifies if the user is subscribed to the push notifications or not</summary>
    /// <type>System.Nullable{System.Boolean}, System</type>
    public bool? IsSubscribed { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }
}

public static class FireBaseUserExtension
{
    public static ModelBuilderWrapper AddFireBaseUsers(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<FireBaseUser>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddFireBaseUsers, Provider.MySql)
            .Add(PgSqlAddFireBaseUsers, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddFireBaseUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FireBaseUser>(entity =>
        {
            entity.HasKey(e => new { e.Id })
                .HasName("PRIMARY");

            entity.ToTable("firebase_users");

            entity.HasIndex(e => new { e.TenantId, e.UserId })
                .HasDatabaseName("user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.IsSubscribed).HasColumnName("is_subscribed");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasColumnType("varchar(36)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.FirebaseDeviceToken)
                .HasColumnName("firebase_device_token")
                .HasColumnType("varchar(255)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Application)
                .HasColumnName("application")
                .HasColumnType("varchar(20)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

        });
    }

    public static void PgSqlAddFireBaseUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FireBaseUser>(entity =>
        {
            entity.HasKey(e => e.Id)
               .HasName("firebase_users_pkey");

            entity.ToTable("firebase_users", "onlyoffice");

            entity.HasIndex(e => new { e.TenantId, e.UserId })
                .HasDatabaseName("user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.IsSubscribed).HasColumnName("is_subscribed");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(36);

            entity.Property(e => e.FirebaseDeviceToken)
                .HasColumnName("firebase_device_token")
                .HasMaxLength(255);

            entity.Property(e => e.Application)
                .HasColumnName("application")
                .HasMaxLength(20);
        });
    }

}
