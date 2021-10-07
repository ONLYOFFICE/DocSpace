using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMigrationCreator
{
    public class MigrationCreator
    {
        public static void Run(Settings settings)
        {
            var counter = 0;
            var ctxTypesFinder = new ContextFinder(settings);
            
            foreach (var contextType in ctxTypesFinder.GetContextsTypes())
            {
                var context = DbContextActivator.CreateInstance(contextType, settings.ConnectionStringSettings);

                var modelDiffChecker = new ModelDifferenceChecker(context);

                if (!modelDiffChecker.IsDifferent()) continue;

                context = DbContextActivator.CreateInstance(contextType, settings.ConnectionStringSettings); //Hack: refresh context

                var migrationGenerator = new MigrationGenerator(context);

                migrationGenerator.Generate();

                counter++;
            }

            Console.WriteLine($"Created {counter} migrations");
        }
    }
}
