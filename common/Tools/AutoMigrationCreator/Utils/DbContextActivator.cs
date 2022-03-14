using System;
using System.Configuration;

using ASC.Core.Common.EF;

namespace AutoMigrationCreator
{
    public class DbContextActivator
    {
        private const string FAKE_CONNECTION_STRING = "Server=localhost;User ID=root;Password=root";
        public static BaseDbContext CreateInstance(Type contextType)
        {
            var context = (BaseDbContext)Activator.CreateInstance(contextType);
            context.ConnectionStringSettings = new ConnectionStringSettings
            {
                ConnectionString = FAKE_CONNECTION_STRING
            };

            return context;
        }
    }
}
