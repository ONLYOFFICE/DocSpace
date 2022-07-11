using System;

using AutoMigrationCreator.Core;

namespace AutoMigrationCreator
{
    public class MigrationCreator
    {
        public static void Run()
        {
            var counter = 0;
            var solution = new Solution();

            foreach (var projectInfo in solution.GetProjects())
            {
                var ctxTypesFinder = new ContextFinder(projectInfo);

                foreach (var contextType in ctxTypesFinder.GetContextsTypes())
                {
                    var context = DbContextActivator.CreateInstance(contextType);

                    var modelDiffChecker = new ModelDifferenceChecker(context);

                    if (!modelDiffChecker.IsDifferent()) continue;

                    context = DbContextActivator.CreateInstance(contextType); //Hack: refresh context

                    var migrationGenerator = new MigrationGenerator(context, projectInfo);
                    migrationGenerator.Generate();

                    counter++;
                }
            }

            Console.WriteLine($"Created {counter} migrations");
        }
    }
}
