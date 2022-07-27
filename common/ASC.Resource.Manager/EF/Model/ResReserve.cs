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

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Resource.Manager.EF.Model;

public class ResReserve
{
    public int Id { get; set; }
    public int FileId { get; set; }
    public string Title { get; set; }
    public string CultureTitle { get; set; }
    public string TextValue { get; set; }
    public int Flag { get; set; }
}

public static class ResReserveExtension
{
    public static ModelBuilderWrapper AddResReserve(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddResReserve, Provider.MySql)
            .Add(PgSqlAddResReserve, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddResReserve(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResReserve>(entity =>
        {
            entity.HasKey(e => new { e.FileId, e.Title, e.CultureTitle })
                .HasName("PRIMARY");

            entity.HasAlternateKey(e => e.Id).HasName("id");

            entity.ToTable("res_reserve")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.CultureTitle)
                .HasDatabaseName("resources_FK2");

            entity.Property(e => e.FileId).HasColumnName("fileid");

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasColumnType("varchar(120)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.CultureTitle)
                .HasColumnName("cultureTitle")
                .HasColumnType("varchar(20)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Flag)
                .HasColumnName("flag")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.TextValue)
                .HasColumnName("textValue")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }
    public static void PgSqlAddResReserve(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResReserve>(entity =>
        {
            entity.HasKey(e => new { e.FileId, e.Title, e.CultureTitle })
                .HasName("res_reserve_pkey");

            entity.ToTable("res_reserve", "onlyoffice");

            entity.Property(e => e.FileId).HasColumnName("fileid");

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(120);

            entity.Property(e => e.CultureTitle)
                .HasColumnName("cultureTitle")
                .HasMaxLength(20);

            entity.Property(e => e.Flag).HasColumnName("flag");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.TextValue).HasColumnName("textValue");
        });
    }
}
