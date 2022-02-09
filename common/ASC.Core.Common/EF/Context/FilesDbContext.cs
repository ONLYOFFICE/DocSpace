﻿namespace ASC.Core.Common.EF.Context
{
    public class MySqlFilesDbContext : FilesDbContext { }
    public class PostgreSqlFilesDbContext : FilesDbContext { }
    public class FilesDbContext : BaseDbContext
    {
        public DbSet<FilesConverts> FilesConverts { get; set; }

        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext =>
            new Dictionary<Provider, Func<BaseDbContext>>()
            {
                { Provider.MySql, () => new MySqlFilesDbContext() } ,
                { Provider.PostgreSql, () => new PostgreSqlFilesDbContext() } ,
            };

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
                .From(modelBuilder, Provider)
                .AddFilesConverts();
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