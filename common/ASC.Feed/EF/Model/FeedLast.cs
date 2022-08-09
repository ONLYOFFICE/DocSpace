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

public class FeedLast : BaseEntity
{
    public string LastKey { get; set; }
    public DateTime LastDate { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { LastKey };
    }
}
public static class FeedLastExtension
{
    public static ModelBuilderWrapper AddFeedLast(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddFeedLast, Provider.MySql)
            .Add(PgSqlAddFeedLast, Provider.PostgreSql);
        return modelBuilder;
    }
    public static void MySqlAddFeedLast(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedLast>(entity =>
        {
            entity.HasKey(e => e.LastKey)
                .HasName("PRIMARY");

            entity.ToTable("feed_last")
                .HasCharSet("utf8");

            entity.Property(e => e.LastKey)
                .HasColumnName("last_key")
                .HasColumnType("varchar(128)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.LastDate)
                .HasColumnName("last_date")
                .HasColumnType("datetime");
        });
    }
    public static void PgSqlAddFeedLast(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedLast>(entity =>
        {
            entity.HasKey(e => e.LastKey)
                .HasName("feed_last_pkey");

            entity.ToTable("feed_last", "onlyoffice");

            entity.Property(e => e.LastKey)
                .HasColumnName("last_key")
                .HasMaxLength(128);

            entity.Property(e => e.LastDate).HasColumnName("last_date");
        });
    }
}
