using System;
using System.IO;

using Microsoft.Extensions.Configuration;

namespace AutoMigrationCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var configPath = Path.GetFullPath(
               Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
               "..", "..", "..", "..", "..", "..", "config", "appsettings.json"));

            var config = new ConfigurationBuilder().AddJsonFile(configPath).Build();
            var settings = Settings.GetSettings(config);

            MigrationCreator.Run(settings);
        }
    }
}
