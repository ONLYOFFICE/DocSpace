using System.Collections.Generic;

using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMigrationCreator
{
    public class ModelDifferenceChecker
    {
        private BaseDbContext _dbContext;
        public ModelDifferenceChecker(BaseDbContext context)
        {
            _dbContext = context;
        }

        public bool IsDifferent()
        {
            var scaffolderDependecies = EFCoreDesignTimeServices.GetServiceProvider(_dbContext)
                .GetService<MigrationsScaffolderDependencies>();

            var modelSnapshot = scaffolderDependecies.MigrationsAssembly.ModelSnapshot;

            if (modelSnapshot == null) return true;

            var lastModel = scaffolderDependecies.SnapshotModelProcessor.Process(modelSnapshot.Model)
                .GetRelationalModel();

            if (modelSnapshot == null) return true;

            var upMethodOperations = scaffolderDependecies.MigrationsModelDiffer.GetDifferences(
                lastModel, scaffolderDependecies.Model.GetRelationalModel());

            var downMethodOperations = upMethodOperations.Count != 0 ?
                scaffolderDependecies.MigrationsModelDiffer.GetDifferences(
                    scaffolderDependecies.Model.GetRelationalModel(), lastModel)
                : new List<MigrationOperation>();

            if (upMethodOperations.Count > 0 || downMethodOperations.Count > 0) return true;

            return false;
        }
    }
}
