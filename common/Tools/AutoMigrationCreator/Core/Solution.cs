using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.Build.Construction;

namespace AutoMigrationCreator.Core
{
    public class Solution
    {
        private const string SOLUTION_NAME = "ASC.Tools.sln";

        public IEnumerable<ProjectInfo> GetProjects()
        {
            var solutionPath = Path.GetFullPath(Path.Combine("..", "..", "..", SOLUTION_NAME));
            var source = SolutionFile.Parse(solutionPath);
            var currentAssembly = Assembly.GetExecutingAssembly().GetName().Name;

            return source.ProjectsInOrder
                    .Where(p => p.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat
                        && p.ProjectName != currentAssembly)
                    .Select(p => new ProjectInfo
                    {
                        AssemblyName = p.ProjectName,
                        Path = p.AbsolutePath.Replace($"{p.ProjectName}.csproj", string.Empty)
                    });
        }
    }
}
