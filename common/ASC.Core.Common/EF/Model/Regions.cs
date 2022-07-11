
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model
{
    public class Regions
    {
        public string Region { get; set; }
        public string Provider { get; set; }
        public string ConnectionString { get; set; }
    }

    public static class RegionsExtension
    {
        public static ModelBuilderWrapper AddRegions(this ModelBuilderWrapper modelBuilder)
        {
            modelBuilder
                .Add(MySqlAddRegions, Provider.MySql)
                .Add(PgSqlAddRegions, Provider.PostgreSql);

            return modelBuilder;
        }

        public static void MySqlAddRegions(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Regions>(entity =>
            {
                entity.HasKey(e => e.Region);
            });
        }

        public static void PgSqlAddRegions(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Regions>(entity =>
            {
                entity.HasKey(e => e.Region);
            });
        }
    }
}
