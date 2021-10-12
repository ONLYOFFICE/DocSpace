using System;

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
