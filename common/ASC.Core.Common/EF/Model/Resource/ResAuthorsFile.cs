using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model.Resource
{
    [Table("res_authorsfile")]
    public class ResAuthorsFile
    {
        public string AuthorLogin { get; set; }
        public int FileId { get; set; }
        public bool WriteAccess { get; set; }
    }

    public static class ResAuthorsFileExtension
    {
        public static ModelBuilder AddResAuthorsFile(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResAuthorsFile>()
                .HasKey(c => new { c.AuthorLogin, c.FileId });

            return modelBuilder;
        }
    }
}
