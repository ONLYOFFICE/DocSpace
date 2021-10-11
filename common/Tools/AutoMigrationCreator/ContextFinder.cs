using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ASC.Core.Common.EF;

namespace AutoMigrationCreator
{
    public class ContextFinder
    {
        public Settings Settings { get; }
        private readonly Type _baseType = typeof(BaseDbContext);
        private readonly string _assemblyName = "ASC.Core.Common";

        public ContextFinder(Settings settings)
        {
            Settings = settings;
        }

        public IEnumerable<Type> GetContextsTypes()
        {
            var coreContextAssembly = Assembly.Load(_assemblyName);
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
            var provider = GetProviderName();

            foreach (var assemblyType in assemblyTypes)
            {
                foreach (var independtType in indepentedTypes)
                {
                    if (IsNeededContext(assemblyType, independtType, provider)) yield return assemblyType;
                }
            }
        }

        private string GetProviderName()
        {
            using var templateContext = new CoreDbContext();
            templateContext.ConnectionStringSettings = Settings.ConnectionStringSettings;

            return templateContext.GetProviderByConnectionString().ToString().ToLower();
        }

        private bool IsNeededContext(Type inheritedСontext, Type baseContext, string providerName)
        {
            if (inheritedСontext.BaseType == baseContext
                && inheritedСontext.Name.ToLower().Contains(providerName)) return true;

            return false;
        }
    }
}
