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

public class ResData
{
    public int Id { get; set; }
    public int FileId { get; set; }
    public string Title { get; set; }
    public string CultureTitle { get; set; }
    public string TextValue { get; set; }
    public string Description { get; set; }
    public DateTime TimeChanges { get; set; }
    public string ResourceType { get; set; }
    public int Flag { get; set; }
    public string Link { get; set; }
    public string AuthorLogin { get; set; }
}

public static class ResDataExtension
{
    public static ModelBuilderWrapper AddResData(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddResData, Provider.MySql)
            .Add(PgSqlAddResData, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddResData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResData>(entity =>
        {
            entity.HasKey(e => new { e.FileId, e.CultureTitle, e.Title })
                .HasName("PRIMARY");

            entity.HasAlternateKey(e => e.Id).HasName("id");

            entity.ToTable("res_data")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.CultureTitle)
                .HasDatabaseName("resources_FK2");

            entity.HasIndex(e => e.TimeChanges)
                .HasDatabaseName("dateIndex");

            entity.Property(e => e.FileId).HasColumnName("fileid");

            entity.Property(e => e.CultureTitle)
                .HasColumnName("cultureTitle")
                .HasColumnType("varchar(20)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasColumnType("varchar(120)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.AuthorLogin)
                .IsRequired()
                .HasColumnName("authorLogin")
                .HasColumnType("varchar(50)")
                .HasDefaultValueSql("'Console'")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Description)
                .HasColumnName("description")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Flag)
                .HasColumnName("flag")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Link)
                .HasColumnName("link")
                .HasColumnType("varchar(120)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ResourceType)
                .HasColumnName("resourceType")
                .HasColumnType("varchar(20)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TextValue)
                .HasColumnName("textValue")
                .HasColumnType("text")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.TimeChanges)
                .HasColumnName("timeChanges")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
    public static void PgSqlAddResData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResData>(entity =>
        {
            entity.HasKey(e => new { e.FileId, e.CultureTitle, e.Title })
                .HasName("res_data_pkey");

            entity.ToTable("res_data", "onlyoffice");

            entity.HasIndex(e => e.CultureTitle)
                .HasDatabaseName("resources_FK2");

            entity.HasIndex(e => e.Id)
                .HasDatabaseName("id_res_data")
                .IsUnique();

            entity.HasIndex(e => e.TimeChanges)
                .HasDatabaseName("dateIndex");

            entity.Property(e => e.FileId).HasColumnName("fileid");

            entity.Property(e => e.CultureTitle)
                .HasColumnName("cultureTitle")
                .HasMaxLength(20);

            entity.Property(e => e.Title)
                .HasColumnName("title")
                .HasMaxLength(120);

            entity.Property(e => e.AuthorLogin)
                .IsRequired()
                .HasColumnName("authorLogin")
                .HasMaxLength(50)
                .HasDefaultValueSql("'Console'");

            entity.Property(e => e.Description).HasColumnName("description");

            entity.Property(e => e.Flag).HasColumnName("flag");

            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Link)
                .HasColumnName("link")
                .HasMaxLength(120)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.ResourceType)
                .HasColumnName("resourceType")
                .HasMaxLength(20)
                .HasDefaultValueSql("NULL");

            entity.Property(e => e.TextValue).HasColumnName("textValue");

            entity.Property(e => e.TimeChanges)
                .HasColumnName("timeChanges")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}
