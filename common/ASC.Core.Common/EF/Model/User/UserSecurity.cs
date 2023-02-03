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

public class UserSecurity : BaseEntity
{
    public int TenantId { get; set; }
    public Guid UserId { get; set; }
    public string PwdHash { get; set; }
    public DateTime? LastModified { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { UserId };
    }
}

public static class UserSecurityExtension
{
    public static ModelBuilderWrapper AddUserSecurity(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<UserSecurity>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddUserSecurity, Provider.MySql)
            .Add(PgSqlAddUserSecurity, Provider.PostgreSql)
            .HasData(
            new UserSecurity
            {
                TenantId = 1,
                UserId = Guid.Parse("66faa6e4-f133-11ea-b126-00ffeec8b4ef"),
                PwdHash = "jGl25bVBBBW96Qi9Te4V37Fnqchz/Eu4qB9vKrRIqRg=",
                LastModified = new DateTime(2022, 7, 8)
            });

        return modelBuilder;
    }

    public static void MySqlAddUserSecurity(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSecurity>(entity =>
        {
            entity.HasKey(e => e.UserId)
                .HasName("PRIMARY");

            entity.ToTable("core_usersecurity")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.PwdHash)
                .HasDatabaseName("pwdhash");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("tenant");

            entity.Property(e => e.UserId)
                .HasColumnName("userid")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.LastModified)
                .HasColumnType("timestamp")
                .IsRequired();

            entity.Property(e => e.PwdHash)
                .HasColumnName("pwdhash")
                .HasColumnType("varchar(512)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TenantId).HasColumnName("tenant");
        });
    }
    public static void PgSqlAddUserSecurity(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserSecurity>(entity =>
        {
            entity.HasKey(e => e.UserId)
                .HasName("core_usersecurity_pkey");

            entity.ToTable("core_usersecurity", "onlyoffice");

            entity.HasIndex(e => e.PwdHash)
                .HasDatabaseName("pwdhash");

            entity.HasIndex(e => e.TenantId)
                .HasDatabaseName("tenant_core_usersecurity");

            entity.Property(e => e.UserId)
                .HasColumnName("userid")
                .HasMaxLength(38);

            entity.Property(e => e.LastModified).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.PwdHash)
                .HasColumnName("pwdhash")
                .HasMaxLength(512)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.TenantId).HasColumnName("tenant");
        });
    }
}
