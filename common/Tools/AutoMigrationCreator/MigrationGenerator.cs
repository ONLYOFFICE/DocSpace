using System;
using System.IO;

using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMigrationCreator
{
    public class MigrationGenerator
    {
        private BaseDbContext _dbContext;
        private string _providerName;
        private string _assemblyName;
        private string _typeName;
        private string _contextFolderName;
        private string ContextFolderName
        {
            get
            {
                if (_contextFolderName == null) _contextFolderName = _typeName[(_providerName.Length)..] + _providerName;

                return _contextFolderName;
            }
        }

        public MigrationGenerator(BaseDbContext context)
        {
            _dbContext = context;
            _typeName = _dbContext.GetType().Name;
            _providerName = _dbContext.GetProviderByConnectionString().ToString();
            _assemblyName = _dbContext.GetType().Assembly.GetName().Name;
        }

        public void Generate()
        {
            var scaffolder = GetScaffolder();

            var migration = scaffolder.ScaffoldMigration($"{ContextFolderName + DateTime.Now.ToString("MM-dd-yyyy-HH-mm-ss")}",
                $"{_assemblyName}", $"Migrations.{_providerName}.{ContextFolderName}");

            SaveMigration(migration);
        }

        private IMigrationsScaffolder GetScaffolder()
        {
            return EFCoreDesignTimeServices.GetServiceProvider(_dbContext).GetService<IMigrationsScaffolder>();
        }

        private void SaveMigration(ScaffoldedMigration migration)
        {
            var path = Path.GetFullPath(Path.Combine("..", "..", _assemblyName,
                "Migrations", _providerName, ContextFolderName));

            Directory.CreateDirectory(path);

            var migrationPath = Path.Combine(path, $"{migration.MigrationId}{migration.FileExtension}");
            var designerPath = Path.Combine(path, $"{migration.MigrationId}.Designer{migration.FileExtension}");
            var snapshotPath = Path.Combine(path, $"{migration.SnapshotName}{migration.FileExtension}");

            File.WriteAllText(migrationPath, migration.MigrationCode);
            File.WriteAllText(designerPath, migration.MetadataCode);
            File.WriteAllText(snapshotPath, migration.SnapshotCode);
        }
    }
}
