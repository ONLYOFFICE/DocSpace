using ASC.Common;
using ASC.Core.Common.EF.Model.Resource;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public partial class ResourceDbContext : BaseDbContext
    {
        public DbSet<ResAuthors> Authors { get; set; }
        public DbSet<ResAuthorsFile> ResAuthorsFiles { get; set; }
        public DbSet<ResAuthorsLang> ResAuthorsLang { get; set; }
        public DbSet<ResCultures> ResCultures { get; set; }
        public DbSet<ResData> ResData { get; set; }
        public DbSet<ResFiles> ResFiles { get; set; }
        public DbSet<ResReserve> ResReserve { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .MySqlAddResAuthorsLang()
                .MySqlAddResAuthorsFile()
                .MySqlAddResAuthors()
                .MySqlAddResCultures()
                .MySqlAddResData()
                .MySqlAddResFiles();

            modelBuilder
               .PgSqlAddResAuthorsLang()
               .PgSqlAddResAuthorsFile()
               .PgSqlAddResAuthors()
               .PgSqlAddResCultures()
               .PgSqlAddResData()
               .PgSqlAddResFiles();

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

    public static class ResourceDbExtension
    {
        public static DIHelper AddResourceDbService(this DIHelper services)
        {
            return services.AddDbContextManagerService<ResourceDbContext>();
        }
    }
}
