using System;
using System.Collections.Generic;

using ASC.Common;
using ASC.Core.Common.EF.Model;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Context
{
    public class MySqlWebstudioDbContext : WebstudioDbContext { }
    public class PostgreSqlWebstudioDbContext : WebstudioDbContext { }
    public class WebstudioDbContext : BaseDbContext
    {
        public DbSet<DbWebstudioSettings> WebstudioSettings { get; set; }
        public DbSet<DbWebstudioUserVisit> WebstudioUserVisit { get; set; }
        public DbSet<DbWebstudioIndex> WebstudioIndex { get; set; }
        protected override Dictionary<Provider, Func<BaseDbContext>> ProviderContext
        {
            get
            {
                return new Dictionary<Provider, Func<BaseDbContext>>()
                {
                    { Provider.MySql, () => new MySqlWebstudioDbContext() } ,
                    { Provider.PostgreSql, () => new PostgreSqlWebstudioDbContext() } ,
                };
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ModelBuilderWrapper
                .From(modelBuilder, Provider)
                .AddWebstudioSettings()
                .AddWebstudioUserVisit()
                .AddDbWebstudioIndex();
        }
    }

    public static class WebstudioDbExtension
    {
        public static DIHelper AddWebstudioDbContextService(this DIHelper services)
        {
            return services.AddDbContextManagerService<WebstudioDbContext>();
        }
    }
}
