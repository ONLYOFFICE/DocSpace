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

public class FeedUsers : BaseEntity
{
    public string FeedId { get; set; }
    public Guid UserId { get; set; }

    public FeedAggregate Feed { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { FeedId, UserId };
    }
}

public static class FeedUsersExtension
{
    public static ModelBuilderWrapper AddFeedUsers(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder.Entity<FeedUsers>().Navigation(e => e.Feed).AutoInclude(false);

        modelBuilder
            .Add(MySqlAddFeedUsers, Provider.MySql)
            .Add(PgSqlAddFeedUsers, Provider.PostgreSql);
        return modelBuilder;
    }

    public static void MySqlAddFeedUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedUsers>(entity =>
        {
            entity.HasKey(e => new { e.FeedId, e.UserId })
                .HasName("PRIMARY");

            entity.ToTable("feed_users")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("user_id");

            entity.Property(e => e.FeedId)
                .HasColumnName("feed_id")
                .HasColumnType("varchar(88)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasColumnType("char(38)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }
    public static void PgSqlAddFeedUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedUsers>(entity =>
        {
            entity.HasKey(e => new { e.FeedId, e.UserId })
                .HasName("feed_users_pkey");

            entity.ToTable("feed_users", "onlyoffice");

            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("user_id_feed_users");

            entity.Property(e => e.FeedId)
                .HasColumnName("feed_id")
                .HasMaxLength(88);

            entity.Property(e => e.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(38)
                .IsFixedLength();
        });
    }
}
