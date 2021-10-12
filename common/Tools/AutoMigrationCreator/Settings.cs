using System.Configuration;

using Microsoft.Extensions.Configuration;

namespace AutoMigrationCreator
{
    public class Settings
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string ProviderName { get; set; }

        private ConnectionStringSettings _connectionStringSettings;
        public ConnectionStringSettings ConnectionStringSettings
        {
            get
            {
                if (_connectionStringSettings == null)
                    _connectionStringSettings = new ConnectionStringSettings(Name, ConnectionString, ProviderName);

                return _connectionStringSettings;
            }
        }

        private Settings() { }

        public static Settings GetSettings(IConfiguration configuration)
        {
            var settings = new Settings();
            configuration.Bind("ConnectionStrings:default", settings);

            return settings;
        }
    }
}
