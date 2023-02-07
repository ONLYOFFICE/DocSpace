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

public class ResFiles
{
    public int Id { get; set; }
    public string ProjectName { get; set; }
    public string ModuleName { get; set; }
    public string ResName { get; set; }
    public bool IsLock { get; set; }
    public DateTime LastUpdate { get; set; }
    public DateTime CreationDate { get; set; }
}

public static class ResFilesExtension
{
    public static ModelBuilderWrapper AddResFiles(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddResFiles, Provider.MySql)
            .Add(PgSqlAddResFiles, Provider.PostgreSql);

        return modelBuilder;
    }

    public static void MySqlAddResFiles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResFiles>(entity =>
        {
            entity.ToTable("res_files")
                .HasCharSet("utf8");

            entity.HasIndex(e => e.ResName)
                .HasDatabaseName("resname")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.CreationDate)
                .HasColumnName("creationDate")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("'0000-00-00 00:00:00'");

            entity.Property(e => e.IsLock)
                .HasColumnName("isLock")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.LastUpdate)
                .HasColumnName("lastUpdate")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.ModuleName)
                .IsRequired()
                .HasColumnName("moduleName")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ProjectName)
                .IsRequired()
                .HasColumnName("projectName")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.ResName)
                .IsRequired()
                .HasColumnName("resName")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }
    public static void PgSqlAddResFiles(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ResFiles>(entity =>
        {
            entity.ToTable("res_files", "onlyoffice");

            entity.HasIndex(e => e.ResName)
                .HasDatabaseName("resname")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.CreationDate)
                .HasColumnName("creationDate")
                .HasDefaultValueSql("'1975-03-03 00:00:00'");

            entity.Property(e => e.IsLock)
                .HasColumnName("isLock")
                .HasDefaultValueSql("'0'");

            entity.Property(e => e.LastUpdate)
                .HasColumnName("lastUpdate")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.ModuleName)
                .IsRequired()
                .HasColumnName("moduleName")
                .HasMaxLength(50);

            entity.Property(e => e.ProjectName)
                .IsRequired()
                .HasColumnName("projectName")
                .HasMaxLength(50);

            entity.Property(e => e.ResName)
                .IsRequired()
                .HasColumnName("resName")
                .HasMaxLength(50);
        });
    }
}
