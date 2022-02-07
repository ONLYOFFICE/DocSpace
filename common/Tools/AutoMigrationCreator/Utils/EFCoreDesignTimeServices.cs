using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations.Design;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace AutoMigrationCreator
{
    public static class EFCoreDesignTimeServices
    {
        public static ServiceProvider GetServiceProvider(BaseDbContext context)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddEntityFrameworkDesignTimeServices();
            serviceCollection.AddDbContextDesignTimeServices(context);
            serviceCollection.AddSingleton<MigrationsCodeGeneratorDependencies>();
            serviceCollection.AddSingleton<AnnotationCodeGeneratorDependencies>();
            serviceCollection.AddSingleton<IAnnotationCodeGenerator, AnnotationCodeGenerator>();
            serviceCollection.AddSingleton(context.GetService<ITypeMappingSource>());

            var designTimeServices = serviceCollection.BuildServiceProvider();

            return designTimeServices;
        }
    }
}
