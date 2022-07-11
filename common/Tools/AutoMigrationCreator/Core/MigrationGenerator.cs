using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using ASC.Core.Common.EF;

using AutoMigrationCreator.Core;

using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMigrationCreator
{
    public class MigrationGenerator
    {
        private BaseDbContext _dbContext;
        private ProjectInfo _projectInfo;
        private string _providerName;
        private string _typeName;
        private string _contextFolderName;
        private Regex _pattern = new Regex(@"\d+$", RegexOptions.Compiled);
        private string ContextFolderName
        {
            get
            {
                if (_contextFolderName == null) _contextFolderName = _typeName[(_providerName.Length)..] + _providerName;

                return _contextFolderName;
            }
        }

        public MigrationGenerator(BaseDbContext context, ProjectInfo projectInfo)
        {
            _dbContext = context;
            _projectInfo = projectInfo;
            _typeName = _dbContext.GetType().Name;
            _providerName = GetProviderName();
        }

        public void Generate()
        {
            var scaffolder = EFCoreDesignTimeServices.GetServiceProvider(_dbContext)
                .GetService<IMigrationsScaffolder>();

            var name = GenerateMigrationName();

            var migration = scaffolder.ScaffoldMigration(name,
                $"{_projectInfo.AssemblyName}", $"Migrations.{_providerName}.{ContextFolderName}");

            SaveMigration(migration);
        }

        private void SaveMigration(ScaffoldedMigration migration)
        {
            var path = Path.Combine(_projectInfo.Path, "Migrations", _providerName, ContextFolderName);

            Directory.CreateDirectory(path);

            var migrationPath = Path.Combine(path, $"{migration.MigrationId}{migration.FileExtension}");
            var designerPath = Path.Combine(path, $"{migration.MigrationId}.Designer{migration.FileExtension}");
            var snapshotPath = Path.Combine(path, $"{migration.SnapshotName}{migration.FileExtension}");

            File.WriteAllText(migrationPath, migration.MigrationCode);
            File.WriteAllText(designerPath, migration.MetadataCode);
            File.WriteAllText(snapshotPath, migration.SnapshotCode);
        }

        private string GetLastMigrationName()
        {
            var scaffolderDependecies = EFCoreDesignTimeServices.GetServiceProvider(_dbContext)
                .GetService<MigrationsScaffolderDependencies>();

            var lastMigration = scaffolderDependecies.MigrationsAssembly.Migrations.LastOrDefault();

            return lastMigration.Key;
        }

        private string GenerateMigrationName()
        {
            var last = GetLastMigrationName();

            if (string.IsNullOrEmpty(last)) return ContextFolderName;

            var migrationNumber = _pattern.Match(last).Value;

            if (string.IsNullOrEmpty(migrationNumber))
                return ContextFolderName + "_Upgrade1";

            return ContextFolderName + "_Upgrade" + (int.Parse(migrationNumber) + 1);
        }

        private string GetProviderName()
        {
            var providers = Enum.GetNames(typeof(Provider));
            var lowerTypeName = _typeName.ToLower();
            var provider = providers.SingleOrDefault(p => lowerTypeName.Contains(p.ToLower()));

            if (provider == null) throw new Exception("Provider not support");

            return provider;
        }
    }
}
