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

using System;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Resource.Manager.EF.Model;

public class ResCultures
{
    public string Title { get; set; }
    public string Value { get; set; }
    public bool Available { get; set; }
    public DateTime CreationDate { get; set; }
}

public static class ResCulturesExtension
{
    public static ModelBuilderWrapper AddResCultures(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddResCultures, Provider.MySql)
            .Add(PgSqlAddResCultures, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddResCultures(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResCultures>(entity =>
        {
            entity.HasKey(e => e.Title)
                .HasName("PRIMARY");

            entity.ToTable("res_cultures")
                .HasCharSet("utf8");

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasColumnType("varchar(120)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Available).HasColumnName("available");

            entity.Property(e => e.CreationDate)
                .HasColumnName("creationDate")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.Value)
                .IsRequired()
                .HasColumnName("value")
                .HasColumnType("varchar(120)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }
    public static void PgSqlAddResCultures(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResCultures>(entity =>
        {
            entity.HasKey(e => e.Title)
                .HasName("res_cultures_pkey");

            entity.ToTable("res_cultures", "onlyoffice");

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasColumnType("character varying");

            entity.Property(e => e.Available)
                .HasColumnName("available")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.CreationDate)
                .HasColumnName("creationDate")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.Value)
                .IsRequired()
                .HasColumnName("value")
                .HasColumnType("character varying");
        });
    }
}
