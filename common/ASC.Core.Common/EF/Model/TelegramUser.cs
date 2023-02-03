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

namespace ASC.Core.Common.EF.Model;

public class TelegramUser : BaseEntity
{
    public Guid PortalUserId { get; set; }
    public int TenantId { get; set; }
    public long TelegramUserId { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { TenantId, PortalUserId };
    }
}

public static class TelegramUsersExtension
{
    public static ModelBuilderWrapper AddTelegramUsers(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<TelegramUser>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddTelegramUsers, Provider.MySql)
            .Add(PgSqlAddTelegramUsers, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddTelegramUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelegramUser>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.PortalUserId })
                .HasName("PRIMARY");

            entity.ToTable("telegram_users")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.TelegramUserId)
                .HasDatabaseName("tgId");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.PortalUserId)
                .HasColumnName("portal_user_id")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TelegramUserId)
                .HasColumnName("telegram_user_id")
                .HasColumnType("int");
        });
    }
    public static void PgSqlAddTelegramUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TelegramUser>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.PortalUserId })
                .HasName("telegram_users_pkey");

            entity.ToTable("telegram_users", "onlyoffice");

            entity.HasIndex(e => e.TelegramUserId)
                .HasDatabaseName("tgId");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.PortalUserId)
                .HasColumnName("portal_user_id")
                .HasMaxLength(38);

            entity.Property(e => e.TelegramUserId).HasColumnName("telegram_user_id");
        });
    }
}
