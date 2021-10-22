using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ASC.Core.Common.EF;

using AutoMigrationCreator.Core;

namespace AutoMigrationCreator
{
    public class ContextFinder
    {
        private readonly Type _baseType = typeof(BaseDbContext);
        private ProjectInfo _projectInfo;

        public ContextFinder(ProjectInfo projectInfo)
        {
            _projectInfo = projectInfo;
        }

        public IEnumerable<Type> GetContextsTypes()
        {
            var coreContextAssembly = Assembly.Load(_projectInfo.AssemblyName);
            var assemblyTypes = coreContextAssembly.GetTypes();

            var independetProviderTypes = GetProviderIndependentContextTypes(assemblyTypes);
            var dependetProviderTypes = GetProviderDependetContextTypes(assemblyTypes, independetProviderTypes);

            foreach (var contextType in dependetProviderTypes)
            {
                yield return contextType;
            }
        }

        private IEnumerable<Type> GetProviderIndependentContextTypes(IEnumerable<Type> assemblyTypes)
        {
            return assemblyTypes.Where(b => b.BaseType == _baseType);
        }

        private IEnumerable<Type> GetProviderDependetContextTypes(IEnumerable<Type> assemblyTypes,
            IEnumerable<Type> indepentedTypes)
        {
            foreach (var assemblyType in assemblyTypes)
            {
                foreach (var independtType in indepentedTypes)
                {
                    if (assemblyType.BaseType == independtType) yield return assemblyType;
                }
            }
        }
    }
}
