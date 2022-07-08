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

public class FilesConverts
{
    public string Input { get; set; }
    public string Output { get; set; }
}

public static class FilesConvertsExtension
{
    public static ModelBuilderWrapper AddFilesConverts(this ModelBuilderWrapper modelBuilder)
    {
        modelBuilder
            .Add(MySqlAddFilesConverts, Provider.MySql)
            .Add(PgSqlAddFilesConverts, Provider.PostgreSql)
            .HasData( 
           new FilesConverts { Input = ".csv", Output = ".ods" },
           new FilesConverts { Input = ".csv", Output = ".pdf" },
           new FilesConverts { Input = ".csv", Output = ".xlsx" },
           new FilesConverts { Input = ".doc", Output = ".docx" },
           new FilesConverts { Input = ".doc", Output = ".odt" },
           new FilesConverts { Input = ".doc", Output = ".pdf" },
           new FilesConverts { Input = ".doc", Output = ".rtf" },
           new FilesConverts { Input = ".doc", Output = ".txt" },
           new FilesConverts { Input = ".docm", Output = ".docx" },
           new FilesConverts { Input = ".docm", Output = ".odt" },
           new FilesConverts { Input = ".docm", Output = ".pdf" },
           new FilesConverts { Input = ".docm", Output = ".rtf" },
           new FilesConverts { Input = ".docm", Output = ".txt" },
           new FilesConverts { Input = ".doct", Output = ".docx" },
           new FilesConverts { Input = ".docx", Output = ".odt" },
           new FilesConverts { Input = ".docx", Output = ".pdf" },
           new FilesConverts { Input = ".docx", Output = ".rtf" },
           new FilesConverts { Input = ".docx", Output = ".txt" },

           new FilesConverts { Input = ".docx", Output = ".docxf" },
           new FilesConverts { Input = ".docxf", Output = ".docx" },
           new FilesConverts { Input = ".docxf", Output = ".odt" },
           new FilesConverts { Input = ".docxf", Output = ".oform" },
           new FilesConverts { Input = ".docxf", Output = ".pdf" },
           new FilesConverts { Input = ".docxf", Output = ".rtf" },
           new FilesConverts { Input = ".docxf", Output = ".txt" },

           new FilesConverts { Input = ".dot", Output = ".docx" },
           new FilesConverts { Input = ".dot", Output = ".odt" },
           new FilesConverts { Input = ".dot", Output = ".pdf" },
           new FilesConverts { Input = ".dot", Output = ".rtf" },
           new FilesConverts { Input = ".dot", Output = ".txt" },
           new FilesConverts { Input = ".dotm", Output = ".docx" },
           new FilesConverts { Input = ".dotm", Output = ".odt" },
           new FilesConverts { Input = ".dotm", Output = ".pdf" },
           new FilesConverts { Input = ".dotm", Output = ".rtf" },
           new FilesConverts { Input = ".dotm", Output = ".txt" },
           new FilesConverts { Input = ".dotx", Output = ".docx" },
           new FilesConverts { Input = ".dotx", Output = ".odt" },
           new FilesConverts { Input = ".dotx", Output = ".pdf" },
           new FilesConverts { Input = ".dotx", Output = ".rtf" },
           new FilesConverts { Input = ".dotx", Output = ".txt" },
           new FilesConverts { Input = ".epub", Output = ".docx" },
           new FilesConverts { Input = ".epub", Output = ".odt" },
           new FilesConverts { Input = ".epub", Output = ".pdf" },
           new FilesConverts { Input = ".epub", Output = ".rtf" },
           new FilesConverts { Input = ".epub", Output = ".txt" },
           new FilesConverts { Input = ".fb2", Output = ".docx" },
           new FilesConverts { Input = ".fb2", Output = ".odt" },
           new FilesConverts { Input = ".fb2", Output = ".pdf" },
           new FilesConverts { Input = ".fb2", Output = ".rtf" },
           new FilesConverts { Input = ".fb2", Output = ".txt" },
           new FilesConverts { Input = ".fodp", Output = ".odp" },
           new FilesConverts { Input = ".fodp", Output = ".pdf" },
           new FilesConverts { Input = ".fodp", Output = ".pptx" },
           new FilesConverts { Input = ".fods", Output = ".csv" },
           new FilesConverts { Input = ".fods", Output = ".ods" },
           new FilesConverts { Input = ".fods", Output = ".pdf" },
           new FilesConverts { Input = ".fods", Output = ".xlsx" },
           new FilesConverts { Input = ".fodt", Output = ".docx" },
           new FilesConverts { Input = ".fodt", Output = ".odt" },
           new FilesConverts { Input = ".fodt", Output = ".pdf" },
           new FilesConverts { Input = ".fodt", Output = ".rtf" },
           new FilesConverts { Input = ".fodt", Output = ".txt" },
           new FilesConverts { Input = ".html", Output = ".docx" },
           new FilesConverts { Input = ".html", Output = ".odt" },
           new FilesConverts { Input = ".html", Output = ".pdf" },
           new FilesConverts { Input = ".html", Output = ".rtf" },
           new FilesConverts { Input = ".html", Output = ".txt" },
           new FilesConverts { Input = ".mht", Output = ".docx" },
           new FilesConverts { Input = ".mht", Output = ".odt" },
           new FilesConverts { Input = ".mht", Output = ".pdf" },
           new FilesConverts { Input = ".mht", Output = ".rtf" },
           new FilesConverts { Input = ".mht", Output = ".txt" },
           new FilesConverts { Input = ".odp", Output = ".pdf" },
           new FilesConverts { Input = ".odp", Output = ".pptx" },
           new FilesConverts { Input = ".otp", Output = ".odp" },
           new FilesConverts { Input = ".otp", Output = ".pdf" },
           new FilesConverts { Input = ".otp", Output = ".pptx" },
           new FilesConverts { Input = ".ods", Output = ".csv" },
           new FilesConverts { Input = ".ods", Output = ".pdf" },
           new FilesConverts { Input = ".ods", Output = ".xlsx" },
           new FilesConverts { Input = ".ots", Output = ".csv" },
           new FilesConverts { Input = ".ots", Output = ".ods" },
           new FilesConverts { Input = ".ots", Output = ".pdf" },
           new FilesConverts { Input = ".ots", Output = ".xlsx" },
           new FilesConverts { Input = ".odt", Output = ".docx" },
           new FilesConverts { Input = ".odt", Output = ".pdf" },
           new FilesConverts { Input = ".odt", Output = ".rtf" },
           new FilesConverts { Input = ".odt", Output = ".txt" },
           new FilesConverts { Input = ".ott", Output = ".docx" },
           new FilesConverts { Input = ".ott", Output = ".odt" },
           new FilesConverts { Input = ".ott", Output = ".pdf" },
           new FilesConverts { Input = ".ott", Output = ".rtf" },
           new FilesConverts { Input = ".ott", Output = ".txt" },
           new FilesConverts { Input = ".pot", Output = ".odp" },
           new FilesConverts { Input = ".pot", Output = ".pdf" },
           new FilesConverts { Input = ".pot", Output = ".pptx" },
           new FilesConverts { Input = ".potm", Output = ".odp" },
           new FilesConverts { Input = ".potm", Output = ".pdf" },
           new FilesConverts { Input = ".potm", Output = ".pptx" },
           new FilesConverts { Input = ".potx", Output = ".odp" },
           new FilesConverts { Input = ".potx", Output = ".pdf" },
           new FilesConverts { Input = ".potx", Output = ".pptx" },
           new FilesConverts { Input = ".pps", Output = ".odp" },
           new FilesConverts { Input = ".pps", Output = ".pdf" },
           new FilesConverts { Input = ".pps", Output = ".pptx" },
           new FilesConverts { Input = ".ppsm", Output = ".odp" },
           new FilesConverts { Input = ".ppsm", Output = ".pdf" },
           new FilesConverts { Input = ".ppsm", Output = ".pptx" },
           new FilesConverts { Input = ".ppsx", Output = ".odp" },
           new FilesConverts { Input = ".ppsx", Output = ".pdf" },
           new FilesConverts { Input = ".ppsx", Output = ".pptx" },
           new FilesConverts { Input = ".ppt", Output = ".odp" },
           new FilesConverts { Input = ".ppt", Output = ".pdf" },
           new FilesConverts { Input = ".ppt", Output = ".pptx" },
           new FilesConverts { Input = ".pptm", Output = ".odp" },
           new FilesConverts { Input = ".pptm", Output = ".pdf" },
           new FilesConverts { Input = ".pptm", Output = ".pptx" },
           new FilesConverts { Input = ".pptt", Output = ".pptx" },
           new FilesConverts { Input = ".pptx", Output = ".odp" },
           new FilesConverts { Input = ".pptx", Output = ".pdf" },
           new FilesConverts { Input = ".rtf", Output = ".odt" },
           new FilesConverts { Input = ".rtf", Output = ".pdf" },
           new FilesConverts { Input = ".rtf", Output = ".docx" },
           new FilesConverts { Input = ".rtf", Output = ".txt" },
           new FilesConverts { Input = ".txt", Output = ".pdf" },
           new FilesConverts { Input = ".txt", Output = ".docx" },
           new FilesConverts { Input = ".txt", Output = ".odt" },
           new FilesConverts { Input = ".txt", Output = ".rtf" },
           new FilesConverts { Input = ".xls", Output = ".csv" },
           new FilesConverts { Input = ".xls", Output = ".ods" },
           new FilesConverts { Input = ".xls", Output = ".pdf" },
           new FilesConverts { Input = ".xls", Output = ".xlsx" },
           new FilesConverts { Input = ".xlsm", Output = ".csv" },
           new FilesConverts { Input = ".xlsm", Output = ".pdf" },
           new FilesConverts { Input = ".xlsm", Output = ".ods" },
           new FilesConverts { Input = ".xlsm", Output = ".xlsx" },
           new FilesConverts { Input = ".xlst", Output = ".pdf" },
           new FilesConverts { Input = ".xlst", Output = ".xlsx" },
           new FilesConverts { Input = ".xlst", Output = ".csv" },
           new FilesConverts { Input = ".xlst", Output = ".ods" },
           new FilesConverts { Input = ".xlt", Output = ".csv" },
           new FilesConverts { Input = ".xlt", Output = ".ods" },
           new FilesConverts { Input = ".xlt", Output = ".pdf" },
           new FilesConverts { Input = ".xlt", Output = ".xlsx" },
           new FilesConverts { Input = ".xltm", Output = ".csv" },
           new FilesConverts { Input = ".xltm", Output = ".ods" },
           new FilesConverts { Input = ".xltm", Output = ".pdf" },
           new FilesConverts { Input = ".xltm", Output = ".xlsx" },
           new FilesConverts { Input = ".xltx", Output = ".pdf" },
           new FilesConverts { Input = ".xltx", Output = ".csv" },
           new FilesConverts { Input = ".xltx", Output = ".ods" },
           new FilesConverts { Input = ".xltx", Output = ".xlsx" },
           new FilesConverts { Input = ".xps", Output = ".pdf" }
           );

        return modelBuilder;
    }

    public static void MySqlAddFilesConverts(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FilesConverts>(entity =>
        {
            entity.HasKey(e => new { e.Input, e.Output })
                .HasName("PRIMARY");

            entity.ToTable("files_converts")
                .HasCharSet("utf8");

            entity.Property(e => e.Input)
                .HasColumnName("input")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            entity.Property(e => e.Output)
                .HasColumnName("output")
                .HasColumnType("varchar(50)")
                .HasCharSet("utf8")
                .UseCollation("utf8_general_ci");
        });
    }
    public static void PgSqlAddFilesConverts(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FilesConverts>(entity =>
        {
            entity.HasKey(e => new { e.Input, e.Output })
                .HasName("files_converts_pkey");

            entity.ToTable("files_converts", "onlyoffice");

            entity.Property(e => e.Input)
                .HasColumnName("input")
                .HasMaxLength(50);

            entity.Property(e => e.Output)
                .HasColumnName("output")
                .HasMaxLength(50);
        });
    }
}
