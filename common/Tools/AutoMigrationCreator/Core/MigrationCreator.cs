// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

namespace AutoMigrationCreator;

public static class MigrationCreator
{
    public static void RunCreateMigrations()
    {
        var counter = 0;

        foreach (var projectInfo in Solution.GetProjects())
        {
            var ctxTypesFinder = new ProjectInfoContextFinder(projectInfo);

            foreach (var contextType in ctxTypesFinder.GetDependetContextsTypes())
            {
                var context = DbContextActivator.CreateInstance(contextType);

                var modelDiffChecker = new ModelDifferenceChecker(context);

                if (!modelDiffChecker.IsDifferent())
                {
                    continue;
                }

                context = DbContextActivator.CreateInstance(contextType); //Hack: refresh context

                var migrationGenerator = new MigrationGenerator(context, projectInfo);
                migrationGenerator.Generate();

                counter++;
            }
        }

        Console.WriteLine($"Created {counter} migrations");
    }

    public static void RunApplyMigrations(string path)
    {
        var counter = 0;

        foreach (var assembly in GetAssemblies(path))
        {
            var ctxTypesFinder = new AssemblyContextFinder(assembly);

            foreach (var contextType in ctxTypesFinder.GetIndependentContextsTypes())
            {
                var context = DbContextActivator.CreateInstance(contextType);

                context.Migrate();

                counter++;
            }
        }

        Console.WriteLine($"Applied {counter} migrations");
    }

    private static IEnumerable<Assembly> GetAssemblies(string path)
    {
        var assemblyPaths = Directory.GetFiles(path, "ASC.*.dll");

        foreach (var assembly in assemblyPaths)
        {
            yield return Assembly.LoadFrom(assembly);
        }
    }
}
