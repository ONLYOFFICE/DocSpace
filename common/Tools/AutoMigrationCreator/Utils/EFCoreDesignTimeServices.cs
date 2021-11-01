using ASC.Core.Common.EF;

using Microsoft.EntityFrameworkCore.Design;
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

            var designTimeServices = serviceCollection.BuildServiceProvider();

            return designTimeServices;
        }
    }
}
