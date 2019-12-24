using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    [Table("files_converts")]
    public class FilesConverts
    {
        public string Input { get; set; }
        public string Ouput { get; set; }
    }

    public static class FilesConvertsExtension
    {
        public static ModelBuilder AddFilesConverts(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FilesConverts>()
                .HasKey(c => new { c.Input, c.Ouput });

            return modelBuilder;
        }
    }
}
