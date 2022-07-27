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
           new FilesConverts { Input = ".csv", Output = ".ots" },
           new FilesConverts { Input = ".csv", Output = ".xlsx" },
           new FilesConverts { Input = ".csv", Output = ".xlsm" },
           new FilesConverts { Input = ".csv", Output = ".xltm" },
           new FilesConverts { Input = ".csv", Output = ".xltx" },
           new FilesConverts { Input = ".doc", Output = ".docx" },
           new FilesConverts { Input = ".doc", Output = ".docm" },
           new FilesConverts { Input = ".doc", Output = ".dotm" },
           new FilesConverts { Input = ".doc", Output = ".dotx" },
           new FilesConverts { Input = ".doc", Output = ".epub" },
           new FilesConverts { Input = ".doc", Output = ".fb2" },
           new FilesConverts { Input = ".doc", Output = ".html" },
           new FilesConverts { Input = ".doc", Output = ".ott" },
           new FilesConverts { Input = ".doc", Output = ".odt" },
           new FilesConverts { Input = ".doc", Output = ".pdf" },
           new FilesConverts { Input = ".doc", Output = ".rtf" },
           new FilesConverts { Input = ".doc", Output = ".txt" },
           new FilesConverts { Input = ".docm", Output = ".docx" },
           new FilesConverts { Input = ".docm", Output = ".dotm" },
           new FilesConverts { Input = ".docm", Output = ".html" },
           new FilesConverts { Input = ".docm", Output = ".dotx" },
           new FilesConverts { Input = ".docm", Output = ".epub" },
           new FilesConverts { Input = ".docm", Output = ".fb2" },
           new FilesConverts { Input = ".docm", Output = ".ott" },
           new FilesConverts { Input = ".docm", Output = ".odt" },
           new FilesConverts { Input = ".docm", Output = ".pdf" },
           new FilesConverts { Input = ".docm", Output = ".rtf" },
           new FilesConverts { Input = ".docm", Output = ".txt" },
           new FilesConverts { Input = ".doct", Output = ".docx" },
           new FilesConverts { Input = ".docx", Output = ".odt" },
           new FilesConverts { Input = ".docx", Output = ".pdf" },
           new FilesConverts { Input = ".docx", Output = ".rtf" },
           new FilesConverts { Input = ".docx", Output = ".txt" },
           new FilesConverts { Input = ".docx", Output = ".html" },
           new FilesConverts { Input = ".docx", Output = ".dotm" },
           new FilesConverts { Input = ".docx", Output = ".dotx" },
           new FilesConverts { Input = ".docx", Output = ".epub" },
           new FilesConverts { Input = ".docx", Output = ".fb2" },
           new FilesConverts { Input = ".docx", Output = ".ott" },
           new FilesConverts { Input = ".docx", Output = ".docm" },

           new FilesConverts { Input = ".docx", Output = ".docxf" },
           new FilesConverts { Input = ".docxf", Output = ".docx" },
           new FilesConverts { Input = ".docxf", Output = ".odt" },
           new FilesConverts { Input = ".docxf", Output = ".oform" },
           new FilesConverts { Input = ".docxf", Output = ".pdf" },
           new FilesConverts { Input = ".docxf", Output = ".rtf" },
           new FilesConverts { Input = ".docxf", Output = ".txt" },
           new FilesConverts { Input = ".docxf", Output = ".dotx" },
           new FilesConverts { Input = ".docxf", Output = ".epub" },
           new FilesConverts { Input = ".docxf", Output = ".fb2" },
           new FilesConverts { Input = ".docxf", Output = ".html" },
           new FilesConverts { Input = ".docxf", Output = ".ott" },

           new FilesConverts { Input = ".dot", Output = ".docx" },
           new FilesConverts { Input = ".dot", Output = ".odt" },
           new FilesConverts { Input = ".dot", Output = ".pdf" },
           new FilesConverts { Input = ".dot", Output = ".rtf" },
           new FilesConverts { Input = ".dot", Output = ".txt" },
           new FilesConverts { Input = ".dot", Output = ".docm" },
           new FilesConverts { Input = ".dot", Output = ".dotm" },
           new FilesConverts { Input = ".dot", Output = ".dotx" },
           new FilesConverts { Input = ".dot", Output = ".epub" },
           new FilesConverts { Input = ".dot", Output = ".fb2" },
           new FilesConverts { Input = ".dot", Output = ".html" },
           new FilesConverts { Input = ".dot", Output = ".ott" },
           new FilesConverts { Input = ".dotm", Output = ".docx" },
           new FilesConverts { Input = ".dotm", Output = ".odt" },
           new FilesConverts { Input = ".dotm", Output = ".pdf" },
           new FilesConverts { Input = ".dotm", Output = ".rtf" },
           new FilesConverts { Input = ".dotm", Output = ".txt" },
           new FilesConverts { Input = ".dotm", Output = ".docm" },
           new FilesConverts { Input = ".dotm", Output = ".dotx" },
           new FilesConverts { Input = ".dotm", Output = ".epub" },
           new FilesConverts { Input = ".dotm", Output = ".fb2" },
           new FilesConverts { Input = ".dotm", Output = ".html" },
           new FilesConverts { Input = ".dotm", Output = ".ott" },
           new FilesConverts { Input = ".dotx", Output = ".docx" },
           new FilesConverts { Input = ".dotx", Output = ".odt" },
           new FilesConverts { Input = ".dotx", Output = ".pdf" },
           new FilesConverts { Input = ".dotx", Output = ".rtf" },
           new FilesConverts { Input = ".dotx", Output = ".txt" },
           new FilesConverts { Input = ".dotx", Output = ".docm" },
           new FilesConverts { Input = ".dotx", Output = ".dotm" },
           new FilesConverts { Input = ".dotx", Output = ".epub" },
           new FilesConverts { Input = ".dotx", Output = ".fb2" },
           new FilesConverts { Input = ".dotx", Output = ".html" },
           new FilesConverts { Input = ".dotx", Output = ".ott" },
           new FilesConverts { Input = ".epub", Output = ".docx" },
           new FilesConverts { Input = ".epub", Output = ".odt" },
           new FilesConverts { Input = ".epub", Output = ".pdf" },
           new FilesConverts { Input = ".epub", Output = ".rtf" },
           new FilesConverts { Input = ".epub", Output = ".txt" },
           new FilesConverts { Input = ".epub", Output = ".docm" },
           new FilesConverts { Input = ".epub", Output = ".dotm" },
           new FilesConverts { Input = ".epub", Output = ".dotx" },
           new FilesConverts { Input = ".epub", Output = ".fb2" },
           new FilesConverts { Input = ".epub", Output = ".html" },
           new FilesConverts { Input = ".epub", Output = ".ott" },
           new FilesConverts { Input = ".fb2", Output = ".docx" },
           new FilesConverts { Input = ".fb2", Output = ".odt" },
           new FilesConverts { Input = ".fb2", Output = ".pdf" },
           new FilesConverts { Input = ".fb2", Output = ".rtf" },
           new FilesConverts { Input = ".fb2", Output = ".txt" },
           new FilesConverts { Input = ".fb2", Output = ".docm" },
           new FilesConverts { Input = ".fb2", Output = ".dotm" },
           new FilesConverts { Input = ".fb2", Output = ".dotx" },
           new FilesConverts { Input = ".fb2", Output = ".epub" },
           new FilesConverts { Input = ".fb2", Output = ".html" },
           new FilesConverts { Input = ".fb2", Output = ".ott" },
           new FilesConverts { Input = ".fodp", Output = ".odp" },
           new FilesConverts { Input = ".fodp", Output = ".pdf" },
           new FilesConverts { Input = ".fodp", Output = ".pptx" },
           new FilesConverts { Input = ".fodp", Output = ".otp" },
           new FilesConverts { Input = ".fodp", Output = ".potm" },
           new FilesConverts { Input = ".fodp", Output = ".potx" },
           new FilesConverts { Input = ".fodp", Output = ".pptm" },
           new FilesConverts { Input = ".fods", Output = ".csv" },
           new FilesConverts { Input = ".fods", Output = ".ods" },
           new FilesConverts { Input = ".fods", Output = ".pdf" },
           new FilesConverts { Input = ".fods", Output = ".xlsx" },
           new FilesConverts { Input = ".fods", Output = ".xlsm" },
           new FilesConverts { Input = ".fods", Output = ".xltm" },
           new FilesConverts { Input = ".fods", Output = ".xltx" },
           new FilesConverts { Input = ".fods", Output = ".ots" },
           new FilesConverts { Input = ".fodt", Output = ".docx" },
           new FilesConverts { Input = ".fodt", Output = ".odt" },
           new FilesConverts { Input = ".fodt", Output = ".docm" },
           new FilesConverts { Input = ".fodt", Output = ".pdf" },
           new FilesConverts { Input = ".fodt", Output = ".rtf" },
           new FilesConverts { Input = ".fodt", Output = ".txt" },
           new FilesConverts { Input = ".fodt", Output = ".dotm" },
           new FilesConverts { Input = ".fodt", Output = ".dotx" },
           new FilesConverts { Input = ".fodt", Output = ".epub" },
           new FilesConverts { Input = ".fodt", Output = ".fb2" },
           new FilesConverts { Input = ".fodt", Output = ".html" },
           new FilesConverts { Input = ".fodt", Output = ".ott" },
           new FilesConverts { Input = ".html", Output = ".docx" },
           new FilesConverts { Input = ".html", Output = ".odt" },
           new FilesConverts { Input = ".html", Output = ".pdf" },
           new FilesConverts { Input = ".html", Output = ".rtf" },
           new FilesConverts { Input = ".html", Output = ".txt" },
           new FilesConverts { Input = ".html", Output = ".docm" },
           new FilesConverts { Input = ".html", Output = ".dotm" },
           new FilesConverts { Input = ".html", Output = ".dotx" },
           new FilesConverts { Input = ".html", Output = ".epub" },
           new FilesConverts { Input = ".html", Output = ".fb2" },
           new FilesConverts { Input = ".html", Output = ".ott" },
           new FilesConverts { Input = ".mht", Output = ".docx" },
           new FilesConverts { Input = ".mht", Output = ".odt" },
           new FilesConverts { Input = ".mht", Output = ".pdf" },
           new FilesConverts { Input = ".mht", Output = ".rtf" },
           new FilesConverts { Input = ".mht", Output = ".txt" },
           new FilesConverts { Input = ".mht", Output = ".docm" },
           new FilesConverts { Input = ".mht", Output = ".dotm" },
           new FilesConverts { Input = ".mht", Output = ".dotx" },
           new FilesConverts { Input = ".mht", Output = ".epub" },
           new FilesConverts { Input = ".mht", Output = ".fb2" },
           new FilesConverts { Input = ".mht", Output = ".ott" },
           new FilesConverts { Input = ".odp", Output = ".pdf" },
           new FilesConverts { Input = ".odp", Output = ".pptx" },
           new FilesConverts { Input = ".odp", Output = ".otp" },
           new FilesConverts { Input = ".odp", Output = ".potm" },
           new FilesConverts { Input = ".odp", Output = ".potx" },
           new FilesConverts { Input = ".odp", Output = ".pptm" },
           new FilesConverts { Input = ".otp", Output = ".odp" },
           new FilesConverts { Input = ".otp", Output = ".pdf" },
           new FilesConverts { Input = ".otp", Output = ".potm" },
           new FilesConverts { Input = ".otp", Output = ".potx" },
           new FilesConverts { Input = ".otp", Output = ".pptm" },
           new FilesConverts { Input = ".otp", Output = ".pptx" },
           new FilesConverts { Input = ".ods", Output = ".csv" },
           new FilesConverts { Input = ".ods", Output = ".pdf" },
           new FilesConverts { Input = ".ods", Output = ".xlsx" },
           new FilesConverts { Input = ".ods", Output = ".ots" },
           new FilesConverts { Input = ".ods", Output = ".xlsm" },
           new FilesConverts { Input = ".ods", Output = ".xltm" },
           new FilesConverts { Input = ".ods", Output = ".xltx" },
           new FilesConverts { Input = ".ots", Output = ".csv" },
           new FilesConverts { Input = ".ots", Output = ".ods" },
           new FilesConverts { Input = ".ots", Output = ".pdf" },
           new FilesConverts { Input = ".ots", Output = ".xlsm" },
           new FilesConverts { Input = ".ots", Output = ".xltm" },
           new FilesConverts { Input = ".ots", Output = ".xltx" },
           new FilesConverts { Input = ".ots", Output = ".xlsx" },
           new FilesConverts { Input = ".oxps", Output = ".pdf" },
           new FilesConverts { Input = ".odt", Output = ".docx" },
           new FilesConverts { Input = ".odt", Output = ".pdf" },
           new FilesConverts { Input = ".odt", Output = ".rtf" },
           new FilesConverts { Input = ".odt", Output = ".txt" },
           new FilesConverts { Input = ".odt", Output = ".docm" },
           new FilesConverts { Input = ".odt", Output = ".dotm" },
           new FilesConverts { Input = ".odt", Output = ".dotx" },
           new FilesConverts { Input = ".odt", Output = ".epub" },
           new FilesConverts { Input = ".odt", Output = ".fb2" },
           new FilesConverts { Input = ".odt", Output = ".html" },
           new FilesConverts { Input = ".odt", Output = ".ott" },
           new FilesConverts { Input = ".ott", Output = ".docx" },
           new FilesConverts { Input = ".ott", Output = ".odt" },
           new FilesConverts { Input = ".ott", Output = ".pdf" },
           new FilesConverts { Input = ".ott", Output = ".rtf" },
           new FilesConverts { Input = ".ott", Output = ".txt" },
           new FilesConverts { Input = ".ott", Output = ".docm" },
           new FilesConverts { Input = ".ott", Output = ".dotm" },
           new FilesConverts { Input = ".ott", Output = ".dotx" },
           new FilesConverts { Input = ".ott", Output = ".epub" },
           new FilesConverts { Input = ".ott", Output = ".fb2" },
           new FilesConverts { Input = ".ott", Output = ".html" },
           new FilesConverts { Input = ".pot", Output = ".odp" },
           new FilesConverts { Input = ".pot", Output = ".pdf" },
           new FilesConverts { Input = ".pot", Output = ".pptx" },
           new FilesConverts { Input = ".pot", Output = ".otp" },
           new FilesConverts { Input = ".pot", Output = ".potm" },
           new FilesConverts { Input = ".pot", Output = ".potx" },
           new FilesConverts { Input = ".pot", Output = ".pptm" },
           new FilesConverts { Input = ".potm", Output = ".odp" },
           new FilesConverts { Input = ".potm", Output = ".pdf" },
           new FilesConverts { Input = ".potm", Output = ".pptx" },
           new FilesConverts { Input = ".potm", Output = ".otp" },
           new FilesConverts { Input = ".potm", Output = ".potx" },
           new FilesConverts { Input = ".potm", Output = ".pptm" },
           new FilesConverts { Input = ".potx", Output = ".odp" },
           new FilesConverts { Input = ".potx", Output = ".pdf" },
           new FilesConverts { Input = ".potx", Output = ".pptx" },
           new FilesConverts { Input = ".potx", Output = ".otp" },
           new FilesConverts { Input = ".potx", Output = ".potm" },
           new FilesConverts { Input = ".potx", Output = ".pptm" },
           new FilesConverts { Input = ".pps", Output = ".odp" },
           new FilesConverts { Input = ".pps", Output = ".pdf" },
           new FilesConverts { Input = ".pps", Output = ".pptx" },
           new FilesConverts { Input = ".pps", Output = ".otp" },
           new FilesConverts { Input = ".pps", Output = ".potm" },
           new FilesConverts { Input = ".pps", Output = ".potx" },
           new FilesConverts { Input = ".pps", Output = ".pptm" },
           new FilesConverts { Input = ".ppsm", Output = ".odp" },
           new FilesConverts { Input = ".ppsm", Output = ".pdf" },
           new FilesConverts { Input = ".ppsm", Output = ".pptx" },
           new FilesConverts { Input = ".ppsm", Output = ".otp" },
           new FilesConverts { Input = ".ppsm", Output = ".potm" },
           new FilesConverts { Input = ".ppsm", Output = ".potx" },
           new FilesConverts { Input = ".ppsm", Output = ".pptm" },
           new FilesConverts { Input = ".ppsx", Output = ".odp" },
           new FilesConverts { Input = ".ppsx", Output = ".pdf" },
           new FilesConverts { Input = ".ppsx", Output = ".pptx" },
           new FilesConverts { Input = ".ppsx", Output = ".otp" },
           new FilesConverts { Input = ".ppsx", Output = ".potm" },
           new FilesConverts { Input = ".ppsx", Output = ".potx" },
           new FilesConverts { Input = ".ppsx", Output = ".pptm" },
           new FilesConverts { Input = ".ppt", Output = ".odp" },
           new FilesConverts { Input = ".ppt", Output = ".pdf" },
           new FilesConverts { Input = ".ppt", Output = ".pptx" },
           new FilesConverts { Input = ".ppt", Output = ".otp" },
           new FilesConverts { Input = ".ppt", Output = ".potm" },
           new FilesConverts { Input = ".ppt", Output = ".potx" },
           new FilesConverts { Input = ".ppt", Output = ".pptm" },
           new FilesConverts { Input = ".pptm", Output = ".odp" },
           new FilesConverts { Input = ".pptm", Output = ".pdf" },
           new FilesConverts { Input = ".pptm", Output = ".pptx" },
           new FilesConverts { Input = ".pptm", Output = ".otp" },
           new FilesConverts { Input = ".pptm", Output = ".potm" },
           new FilesConverts { Input = ".pptm", Output = ".potx" },
           new FilesConverts { Input = ".pptt", Output = ".pptx" },
           new FilesConverts { Input = ".pptx", Output = ".odp" },
           new FilesConverts { Input = ".pptx", Output = ".pdf" },
           new FilesConverts { Input = ".pptx", Output = ".otp" },
           new FilesConverts { Input = ".pptx", Output = ".potm" },
           new FilesConverts { Input = ".pptx", Output = ".potx" },
           new FilesConverts { Input = ".pptx", Output = ".pptm" },
           new FilesConverts { Input = ".rtf", Output = ".odt" },
           new FilesConverts { Input = ".rtf", Output = ".pdf" },
           new FilesConverts { Input = ".rtf", Output = ".docx" },
           new FilesConverts { Input = ".rtf", Output = ".txt" },
           new FilesConverts { Input = ".rtf", Output = ".docm" },
           new FilesConverts { Input = ".rtf", Output = ".dotm" },
           new FilesConverts { Input = ".rtf", Output = ".dotx" },
           new FilesConverts { Input = ".rtf", Output = ".epub" },
           new FilesConverts { Input = ".rtf", Output = ".fb2" },
           new FilesConverts { Input = ".rtf", Output = ".html" },
           new FilesConverts { Input = ".rtf", Output = ".ott" },
           new FilesConverts { Input = ".txt", Output = ".pdf" },
           new FilesConverts { Input = ".txt", Output = ".docx" },
           new FilesConverts { Input = ".txt", Output = ".odt" },
           new FilesConverts { Input = ".txt", Output = ".rtf" },
           new FilesConverts { Input = ".txt", Output = ".docm" },
           new FilesConverts { Input = ".txt", Output = ".dotm" },
           new FilesConverts { Input = ".txt", Output = ".dotx" },
           new FilesConverts { Input = ".txt", Output = ".epub" },
           new FilesConverts { Input = ".txt", Output = ".fb2" },
           new FilesConverts { Input = ".txt", Output = ".html" },
           new FilesConverts { Input = ".txt", Output = ".ott" },
           new FilesConverts { Input = ".xls", Output = ".csv" },
           new FilesConverts { Input = ".xls", Output = ".ods" },
           new FilesConverts { Input = ".xls", Output = ".pdf" },
           new FilesConverts { Input = ".xls", Output = ".xlsx" },
           new FilesConverts { Input = ".xls", Output = ".ots" },
           new FilesConverts { Input = ".xls", Output = ".xlsm" },
           new FilesConverts { Input = ".xls", Output = ".xltm" },
           new FilesConverts { Input = ".xls", Output = ".xltx" },
           new FilesConverts { Input = ".xlsm", Output = ".csv" },
           new FilesConverts { Input = ".xlsm", Output = ".xltm" },
           new FilesConverts { Input = ".xlsm", Output = ".xltx" },
           new FilesConverts { Input = ".xlsm", Output = ".ots" },
           new FilesConverts { Input = ".xlsm", Output = ".pdf" },
           new FilesConverts { Input = ".xlsm", Output = ".ods" },
           new FilesConverts { Input = ".xlsm", Output = ".xlsx" },
           new FilesConverts { Input = ".xlsx", Output = ".csv" },
           new FilesConverts { Input = ".xlsx", Output = ".ods" },
           new FilesConverts { Input = ".xlsx", Output = ".ots" },
           new FilesConverts { Input = ".xlsx", Output = ".pdf" },
           new FilesConverts { Input = ".xlsx", Output = ".xlsm" },
           new FilesConverts { Input = ".xlsx", Output = ".xltm" },
           new FilesConverts { Input = ".xlsx", Output = ".xltx" },
           new FilesConverts { Input = ".xlst", Output = ".xlsx" },
           new FilesConverts { Input = ".xlt", Output = ".csv" },
           new FilesConverts { Input = ".xlt", Output = ".ods" },
           new FilesConverts { Input = ".xlt", Output = ".pdf" },
           new FilesConverts { Input = ".xlt", Output = ".xlsx" },
           new FilesConverts { Input = ".xlt", Output = ".ots" },
           new FilesConverts { Input = ".xlt", Output = ".xlsm" },
           new FilesConverts { Input = ".xlt", Output = ".xltm" },
           new FilesConverts { Input = ".xlt", Output = ".xltx" },
           new FilesConverts { Input = ".xltm", Output = ".csv" },
           new FilesConverts { Input = ".xltm", Output = ".ods" },
           new FilesConverts { Input = ".xltm", Output = ".pdf" },
           new FilesConverts { Input = ".xltm", Output = ".ots" },
           new FilesConverts { Input = ".xltm", Output = ".xlsm" },
           new FilesConverts { Input = ".xltm", Output = ".xltx" },
           new FilesConverts { Input = ".xltm", Output = ".xlsx" },
           new FilesConverts { Input = ".xltx", Output = ".pdf" },
           new FilesConverts { Input = ".xltx", Output = ".csv" },
           new FilesConverts { Input = ".xltx", Output = ".ods" },
           new FilesConverts { Input = ".xltx", Output = ".ots" },
           new FilesConverts { Input = ".xltx", Output = ".xlsm" },
           new FilesConverts { Input = ".xltx", Output = ".xltm" },
           new FilesConverts { Input = ".xltx", Output = ".xlsx" },
           new FilesConverts { Input = ".xps", Output = ".pdf" },
           new FilesConverts { Input = ".xml", Output = ".docm" },
           new FilesConverts { Input = ".xml", Output = ".docx" },
           new FilesConverts { Input = ".xml", Output = ".dotm" },
           new FilesConverts { Input = ".xml", Output = ".dotx" },
           new FilesConverts { Input = ".xml", Output = ".epub" },
           new FilesConverts { Input = ".xml", Output = ".fb2" },
           new FilesConverts { Input = ".xml", Output = ".html" },
           new FilesConverts { Input = ".xml", Output = ".odt" },
           new FilesConverts { Input = ".xml", Output = ".ott" },
           new FilesConverts { Input = ".xml", Output = ".pdf" },
           new FilesConverts { Input = ".xml", Output = ".rtf" },
           new FilesConverts { Input = ".xml", Output = ".txt" }
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
