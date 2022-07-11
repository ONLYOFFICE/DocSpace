
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model.Mail
{
    [Keyless]
    public class GreyListingWhiteList
    {
        public string Comment { get; set; }
        public string Source { get; set; }
    }

    public static class GreyListingWhiteListExtension
    {
        public static ModelBuilderWrapper AddGreyListingWhiteList(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddGreyListingWhiteList, Provider.MySql)
                .Add(PgSqlAddGreyListingWhiteList, Provider.PostgreSql);

            return modelBuilder;
        }

        public static void MySqlAddGreyListingWhiteList(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GreyListingWhiteList>()
                .HasKey(e => e.Comment);
        }

        public static void PgSqlAddGreyListingWhiteList(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GreyListingWhiteList>()
                .HasKey(e => e.Comment);
        }
    }
}
