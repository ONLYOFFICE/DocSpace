using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;

namespace ASC.Core.Common.EF.Context
{
    public class Migrator
    {
        internal ConnectionStringSettings ConnectionStringSettings { get; }
        
        //Method 1
   
        public void UpdateDb()
        {
            var configuration = new DbMigrationsConfiguration
            {
                TargetDatabase = new DbConnectionInfo(ConnectionStringSettings.ConnectionString, ConnectionStringSettings.ProviderName)
            };

            var migrator = new DbMigrator(configuration);
            var local = migrator.GetLocalMigrations().ToArray();
            migrator.Update();
        }

        //Method 2
        public static IEnumerable<Migration> GetLocalMigrations()
        {
            var migrations = new List<Migration>();
            string Migration_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Migrations");
            var Migration_Files = Directory.GetFiles(Migration_Path);
            foreach (var MigrationsFiles in Migration_Files)
            {
                var FileName = Path.GetFileName(MigrationsFiles);
                var file = File.ReadAllText(MigrationsFiles);
                var name = FileName.Split('-');
                var success = int.TryParse(name[0], out var migrationNumber);
                if (success)
                {
                    if (migrations.Any(m => m.Number == migrationNumber))
                    {
                        throw new ArgumentException(
                            $"There are two migrations with this {migrationNumber}.");
                    }

                    migrations.Add(new Migration(FileName, file, migrationNumber));
                }
                else
                {
                    throw new FormatException($"The migration file must start with a number.");
                }
                
            }
            return migrations;
        }
    }
}

