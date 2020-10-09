using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("files_converts")]
    public class FilesConverts
    {
        [Key]
        public string Input { get; set; }
        public string Ouput { get; set; }
    }

    public static class FilesConvertsExtension
    {
        public static ModelBuilderWrapper AddFilesConverts(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddFilesConverts, Provider.MySql)
                .Add(PgSqlAddFilesConverts, Provider.Postgre)
                .HasData(
               new FilesConverts { Input = ".csv", Ouput = ".ods" },
               new FilesConverts { Input = ".csv", Ouput = ".pdf" },
               new FilesConverts { Input = ".csv", Ouput = ".xlsx" },
               new FilesConverts { Input = ".doc", Ouput = ".docx" },
               new FilesConverts { Input = ".doc", Ouput = ".odt" },
               new FilesConverts { Input = ".doc", Ouput = ".pdf" },
               new FilesConverts { Input = ".doc", Ouput = ".rtf" },
               new FilesConverts { Input = ".doc", Ouput = ".txt" },
               new FilesConverts { Input = ".docm", Ouput = ".docx" },
               new FilesConverts { Input = ".docm", Ouput = ".odt" },
               new FilesConverts { Input = ".docm", Ouput = ".pdf" },
               new FilesConverts { Input = ".docm", Ouput = ".rtf" },
               new FilesConverts { Input = ".docm", Ouput = ".txt" },
               new FilesConverts { Input = ".doct", Ouput = ".docx" },
               new FilesConverts { Input = ".docx", Ouput = ".odt" },
               new FilesConverts { Input = ".docx", Ouput = ".pdf" },
               new FilesConverts { Input = ".docx", Ouput = ".rtf" },
               new FilesConverts { Input = ".docx", Ouput = ".txt" },
               new FilesConverts { Input = ".dot", Ouput = ".docx" },
               new FilesConverts { Input = ".dot", Ouput = ".odt" },
               new FilesConverts { Input = ".dot", Ouput = ".pdf" },
               new FilesConverts { Input = ".dot", Ouput = ".rtf" },
               new FilesConverts { Input = ".dot", Ouput = ".txt" },
               new FilesConverts { Input = ".dotm", Ouput = ".docx" },
               new FilesConverts { Input = ".dotm", Ouput = ".odt" },
               new FilesConverts { Input = ".dotm", Ouput = ".pdf" },
               new FilesConverts { Input = ".dotm", Ouput = ".rtf" },
               new FilesConverts { Input = ".dotm", Ouput = ".txt" },
               new FilesConverts { Input = ".dotx", Ouput = ".docx" },
               new FilesConverts { Input = ".dotx", Ouput = ".odt" },
               new FilesConverts { Input = ".dotx", Ouput = ".pdf" },
               new FilesConverts { Input = ".dotx", Ouput = ".rtf" },
               new FilesConverts { Input = ".dotx", Ouput = ".txt" },
               new FilesConverts { Input = ".epub", Ouput = ".docx" },
               new FilesConverts { Input = ".epub", Ouput = ".odt" },
               new FilesConverts { Input = ".epub", Ouput = ".pdf" },
               new FilesConverts { Input = ".epub", Ouput = ".rtf" },
               new FilesConverts { Input = ".epub", Ouput = ".txt" },
               new FilesConverts { Input = ".fodp", Ouput = ".odp" },
               new FilesConverts { Input = ".fodp", Ouput = ".pdf" },
               new FilesConverts { Input = ".fodp", Ouput = ".pptx" },
               new FilesConverts { Input = ".fods", Ouput = ".csv" },
               new FilesConverts { Input = ".fods", Ouput = ".ods" },
               new FilesConverts { Input = ".fods", Ouput = ".pdf" },
               new FilesConverts { Input = ".fods", Ouput = ".xlsx" },
               new FilesConverts { Input = ".fodt", Ouput = ".docx" },
               new FilesConverts { Input = ".fodt", Ouput = ".odt" },
               new FilesConverts { Input = ".fodt", Ouput = ".pdf" },
               new FilesConverts { Input = ".fodt", Ouput = ".rtf" },
               new FilesConverts { Input = ".fodt", Ouput = ".txt" },
               new FilesConverts { Input = ".html", Ouput = ".docx" },
               new FilesConverts { Input = ".html", Ouput = ".odt" },
               new FilesConverts { Input = ".html", Ouput = ".pdf" },
               new FilesConverts { Input = ".html", Ouput = ".rtf" },
               new FilesConverts { Input = ".html", Ouput = ".txt" },
               new FilesConverts { Input = ".mht", Ouput = ".docx" },
               new FilesConverts { Input = ".mht", Ouput = ".odt" },
               new FilesConverts { Input = ".mht", Ouput = ".pdf" },
               new FilesConverts { Input = ".mht", Ouput = ".rtf" },
               new FilesConverts { Input = ".mht", Ouput = ".txt" },
               new FilesConverts { Input = ".odp", Ouput = ".pdf" },
               new FilesConverts { Input = ".odp", Ouput = ".pptx" },
               new FilesConverts { Input = ".otp", Ouput = ".odp" },
               new FilesConverts { Input = ".otp", Ouput = ".pdf" },
               new FilesConverts { Input = ".otp", Ouput = ".pptx" },
               new FilesConverts { Input = ".ods", Ouput = ".csv" },
               new FilesConverts { Input = ".ods", Ouput = ".pdf" },
               new FilesConverts { Input = ".ods", Ouput = ".xlsx" },
               new FilesConverts { Input = ".ots", Ouput = ".csv" },
               new FilesConverts { Input = ".ots", Ouput = ".ods" },
               new FilesConverts { Input = ".ots", Ouput = ".pdf" },
               new FilesConverts { Input = ".ots", Ouput = ".xlsx" },
               new FilesConverts { Input = ".odt", Ouput = ".docx" },
               new FilesConverts { Input = ".odt", Ouput = ".pdf" },
               new FilesConverts { Input = ".odt", Ouput = ".rtf" },
               new FilesConverts { Input = ".odt", Ouput = ".txt" },
               new FilesConverts { Input = ".ott", Ouput = ".docx" },
               new FilesConverts { Input = ".ott", Ouput = ".odt" },
               new FilesConverts { Input = ".ott", Ouput = ".pdf" },
               new FilesConverts { Input = ".ott", Ouput = ".rtf" },
               new FilesConverts { Input = ".ott", Ouput = ".txt" },
               new FilesConverts { Input = ".pot", Ouput = ".odp" },
               new FilesConverts { Input = ".pot", Ouput = ".pdf" },
               new FilesConverts { Input = ".pot", Ouput = ".pptx" },
               new FilesConverts { Input = ".potm", Ouput = ".odp" },
               new FilesConverts { Input = ".potm", Ouput = ".pdf" },
               new FilesConverts { Input = ".potm", Ouput = ".pptx" },
               new FilesConverts { Input = ".potx", Ouput = ".odp" },
               new FilesConverts { Input = ".potx", Ouput = ".pdf" },
               new FilesConverts { Input = ".potx", Ouput = ".pptx" },
               new FilesConverts { Input = ".pps", Ouput = ".odp" },
               new FilesConverts { Input = ".pps", Ouput = ".pdf" },
               new FilesConverts { Input = ".pps", Ouput = ".pptx" },
               new FilesConverts { Input = ".ppsm", Ouput = ".odp" },
               new FilesConverts { Input = ".ppsm", Ouput = ".pdf" },
               new FilesConverts { Input = ".ppsm", Ouput = ".pptx" },
               new FilesConverts { Input = ".ppsx", Ouput = ".odp" },
               new FilesConverts { Input = ".ppsx", Ouput = ".pdf" },
               new FilesConverts { Input = ".ppsx", Ouput = ".pptx" },
               new FilesConverts { Input = ".ppt", Ouput = ".odp" },
               new FilesConverts { Input = ".ppt", Ouput = ".pdf" },
               new FilesConverts { Input = ".ppt", Ouput = ".pptx" },
               new FilesConverts { Input = ".pptm", Ouput = ".odp" },
               new FilesConverts { Input = ".pptm", Ouput = ".pdf" },
               new FilesConverts { Input = ".pptm", Ouput = ".pptx" },
               new FilesConverts { Input = ".pptt", Ouput = ".odp" },
               new FilesConverts { Input = ".pptt", Ouput = ".pdf" },
               new FilesConverts { Input = ".pptt", Ouput = ".pptx" },
               new FilesConverts { Input = ".pptx", Ouput = ".odp" },
               new FilesConverts { Input = ".pptx", Ouput = ".pdf" },
               new FilesConverts { Input = ".rtf", Ouput = ".odp" },
               new FilesConverts { Input = ".rtf", Ouput = ".pdf" },
               new FilesConverts { Input = ".rtf", Ouput = ".docx" },
               new FilesConverts { Input = ".rtf", Ouput = ".txt" },
               new FilesConverts { Input = ".txt", Ouput = ".pdf" },
               new FilesConverts { Input = ".txt", Ouput = ".docx" },
               new FilesConverts { Input = ".txt", Ouput = ".odp" },
               new FilesConverts { Input = ".txt", Ouput = ".rtx" },
               new FilesConverts { Input = ".xls", Ouput = ".csv" },
               new FilesConverts { Input = ".xls", Ouput = ".ods" },
               new FilesConverts { Input = ".xls", Ouput = ".pdf" },
               new FilesConverts { Input = ".xls", Ouput = ".xlsx" },
               new FilesConverts { Input = ".xlsm", Ouput = ".csv" },
               new FilesConverts { Input = ".xlsm", Ouput = ".pdf" },
               new FilesConverts { Input = ".xlsm", Ouput = ".ods" },
               new FilesConverts { Input = ".xlsm", Ouput = ".xlsx" },
               new FilesConverts { Input = ".xlst", Ouput = ".pdf" },
               new FilesConverts { Input = ".xlst", Ouput = ".xlsx" },
               new FilesConverts { Input = ".xlst", Ouput = ".csv" },
               new FilesConverts { Input = ".xlst", Ouput = ".ods" },
               new FilesConverts { Input = ".xlt", Ouput = ".csv" },
               new FilesConverts { Input = ".xlt", Ouput = ".ods" },
               new FilesConverts { Input = ".xlt", Ouput = ".pdf" },
               new FilesConverts { Input = ".xlt", Ouput = ".xlsx" },
               new FilesConverts { Input = ".xltm", Ouput = ".csv" },
               new FilesConverts { Input = ".xltm", Ouput = ".ods" },
               new FilesConverts { Input = ".xltm", Ouput = ".pdf" },
               new FilesConverts { Input = ".xltm", Ouput = ".xlsx" },
               new FilesConverts { Input = ".xltx", Ouput = ".pdf" },
               new FilesConverts { Input = ".xltx", Ouput = ".csv" },
               new FilesConverts { Input = ".xltx", Ouput = ".ods" },
               new FilesConverts { Input = ".xltx", Ouput = ".xlsx" },
               new FilesConverts { Input = ".xps", Ouput = ".pdf" }
               );

            return modelBuilder;
        }

        public static void MySqlAddFilesConverts(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilesConverts>(entity =>
            {
                entity.HasKey(e => new { e.Input, e.Ouput })
                    .HasName("PRIMARY");

                entity.ToTable("files_converts");

                entity.Property(e => e.Input)
                    .HasColumnName("input")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");

                entity.Property(e => e.Ouput)
                    .HasColumnName("output")
                    .HasColumnType("varchar(50)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_general_ci");
            });
    }
        public static void PgSqlAddFilesConverts(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilesConverts>(entity =>
            {
                entity.HasKey(e => new { e.Input, e.Ouput })
                    .HasName("files_converts_pkey");

                entity.ToTable("files_converts", "onlyoffice");

                entity.Property(e => e.Input)
                    .HasColumnName("input")
                    .HasMaxLength(50);

                entity.Property(e => e.Ouput)
                    .HasColumnName("output")
                    .HasMaxLength(50);
            });
        }
    }
}
