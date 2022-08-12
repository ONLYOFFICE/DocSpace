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

public class NotifyInfo
{
    public int NotifyId { get; set; }
    public int State { get; set; }
    public int Attempts { get; set; }
    public DateTime ModifyDate { get; set; }
    public int Priority { get; set; }
}
public static class NotifyInfoExtension
{
    public static ModelBuilderWrapper AddNotifyInfo(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddNotifyInfo, Provider.MySql)
            .Add(PgSqlAddNotifyInfo, Provider.PostgreSql);

        return modelBuilder;
    }
    public static void MySqlAddNotifyInfo(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotifyInfo>(entity =>
        {
            entity.HasKey(e => e.NotifyId)
                .HasName("PRIMARY");

            entity.ToTable("notify_info")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.State)
                .HasDatabaseName("state");

            entity.Property(e => e.NotifyId)
                .HasColumnName("notify_id")
                .ValueGeneratedNever();

            entity.Property(e => e.Attempts)
                .HasColumnName("attempts")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.ModifyDate)
                .HasColumnName("modify_date")
                .HasColumnType("datetime");

            entity.Property(e => e.Priority)
                .HasColumnName("priority")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.State)
                .HasColumnName("state")
                .HasDefaultValueSql("'0'");
        });
    }
    public static void PgSqlAddNotifyInfo(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotifyInfo>(entity =>
        {
            entity.HasKey(e => e.NotifyId)
                .HasName("notify_info_pkey");

            entity.ToTable("notify_info", "onlyoffice");

            entity.HasIndex(e => e.State)
                .HasDatabaseName("state");

            entity.Property(e => e.NotifyId)
                .HasColumnName("notify_id")
                .ValueGeneratedNever();

            entity.Property(e => e.Attempts).HasColumnName("attempts");

            entity.Property(e => e.ModifyDate).HasColumnName("modify_date");

            entity.Property(e => e.Priority).HasColumnName("priority");

            entity.Property(e => e.State).HasColumnName("state");
        });
    }
}
