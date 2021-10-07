using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace AutoMigrationCreator
{
    public class Settings
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public string MigrationPath { get; set; } = "D:\\AppServer\\common\\ASC.Core.Common\\Migrations\\MySql\\";
        public string AssemblyName { get; set; } = "ASC.Core.Common";

        private ConnectionStringSettings _connectionStringSettings;
        public ConnectionStringSettings ConnectionStringSettings
        {
            get
            {
                if (_connectionStringSettings == null)
                    _connectionStringSettings = new ConnectionStringSettings(Name, ConnectionString);

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
