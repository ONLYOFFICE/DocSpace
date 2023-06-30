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

namespace ASC.Feed.Model;

public class FeedReaded : BaseEntity
{
    public Guid UserId { get; set; }
    public DateTime TimeStamp { get; set; }
    public string Module { get; set; }
    public int TenantId { get; set; }

    public DbTenant Tenant { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { TenantId, UserId, Module };
    }
}

public static class FeedReadedExtension
{
    public static ModelBuilderWrapper AddFeedReaded(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<FeedReaded>().Navigation(e => e.Tenant).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddFeedReaded, Provider.MySql)
            .Add(PgSqlAddFeedReaded, Provider.PostgreSql);
        return modelBuilder;
    }
    public static void MySqlAddFeedReaded(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedReaded>(entity =>
        {
            entity.HasKey(e => new { e.TenantId, e.UserId, e.Module })
                .HasName("PRIMARY");

            entity.ToTable("feed_readed")
                .HasCharSet("utf8");

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasColumnType("varchar(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Module)
                .HasColumnName("module")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TimeStamp)
                .HasColumnName("timestamp")
                .HasColumnType("datetime");
        });
    }
    public static void PgSqlAddFeedReaded(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedReaded>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.TenantId, e.Module })
                .HasName("feed_readed_pkey");

            entity.ToTable("feed_readed", "onlyoffice");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(38);

            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.Property(e => e.Module)
                .HasColumnName("module")
                .HasMaxLength(50);

            entity.Property(e => e.TimeStamp).HasColumnName("timestamp");
        });
    }
}
