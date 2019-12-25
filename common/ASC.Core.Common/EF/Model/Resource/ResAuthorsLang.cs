using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model.Resource
{
    [Table("res_authorslang")]
    public class ResAuthorsLang
    {
        public string AuthorLogin { get; set; }
        public string CultureTitle { get; set; }
    }

    public static class ResAuthorsLangExtension
    {
        public static ModelBuilder AddResAuthorsLang(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResAuthorsLang>()
                .HasKey(c => new { c.AuthorLogin, c.CultureTitle });

            return modelBuilder;
        }
    }
}
