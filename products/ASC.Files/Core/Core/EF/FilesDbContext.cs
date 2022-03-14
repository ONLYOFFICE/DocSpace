using System;
using System.Collections.Generic;

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;
namespace ASC.Files.Core.EF
{
    public class MySqlFilesDbContext : FilesDbContext { }
    public class PostgreSqlFilesDbContext : FilesDbContext { }
    public class FilesDbContext : BaseDbContext
    {
        public DbSet<DbFile> Files { get; set; }
        public DbSet<DbFolder> Folders { get; set; }
        public DbSet<DbFolderTree> Tree { get; set; }
        public DbSet<DbFilesBunchObjects> BunchObjects { get; set; }
        public DbSet<DbFilesSecurity> Security { get; set; }
        public DbSet<DbFilesThirdpartyIdMapping> ThirdpartyIdMapping { get; set; }
        public DbSet<DbFilesThirdpartyAccount> ThirdpartyAccount { get; set; }
        public DbSet<DbFilesTagLink> TagLink { get; set; }
        public DbSet<DbFilesTag> Tag { get; set; }
        public DbSet<DbFilesThirdpartyApp> ThirdpartyApp { get; set; }
        public DbSet<DbFilesLink> FilesLink { get; set; }
        public DbSet<DbTariff> Tariffs { get; set; }
        public DbSet<DbQuota> Quotas { get; set; }
        public DbSet<DbTenant> Tenants { get; set; }

        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
        {
            get
            {
                return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlFilesDbContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlFilesDbContext() } ,
                };
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
                .From(modelBuilder, Provider)
                .AddDbFiles()
                .AddDbFolder()
                .AddDbFolderTree()
                .AddDbFilesThirdpartyAccount()
                .AddDbFilesBunchObjects()
                .AddDbFilesSecurity()
                .AddDbFilesThirdpartyIdMapping()
                .AddDbFilesTagLink()
                .AddDbFilesTag()
                .AddDbDbFilesThirdpartyApp()                
                .AddDbFilesLink()
                .AddDbTariff()
                .AddDbQuota()                
                .AddDbTenant();
        }
    }

    public static class FilesDbExtension
    {
        public static DIHelper AddFilesDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<FilesDbContext>();
        }
    }
}
