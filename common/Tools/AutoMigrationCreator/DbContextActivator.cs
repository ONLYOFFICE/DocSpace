using System;
using System.Configuration;

using ASC.Core.Common.EF;

namespace AutoMigrationCreator
{
    public class DbContextActivator
    {
        public static BaseDbContext CreateInstance(Type contextType, ConnectionStringSettings connectionSettings)
        {
            var context = (BaseDbContext)Activator.CreateInstance(contextType);
            context.ConnectionStringSettings = connectionSettings;

            return context;
        }
    }
}
