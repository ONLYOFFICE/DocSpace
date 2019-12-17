using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ASC.Core.Common.EF
{
    public class BaseDbContextManager<T> : OptionsManager<T>, IDisposable where T : class, IDisposable, IAsyncDisposable, new()
    {
        private Dictionary<string, T> Pairs { get; set; }
        private List<T> AsyncList { get; set; }

        public IOptionsFactory<T> Factory { get; }

        public BaseDbContextManager(IOptionsFactory<T> factory) : base(factory)
        {
            Pairs = new Dictionary<string, T>();
            AsyncList = new List<T>();
            Factory = factory;
        }

        public override T Get(string name)
        {
            if (!Pairs.ContainsKey(name))
            {
                Pairs.Add(name, base.Get(name));
            }

            return Pairs[name];
        }

        public T GetNew(string name = "default")
        {
            var result = Factory.Create(name);

            AsyncList.Add(result);

            return result;
        }

        public void Dispose()
        {
            foreach (var v in Pairs)
            {
                v.Value.Dispose();
            }

            foreach (var v in AsyncList)
            {
                v.Dispose();
            }
        }
    }

    public class DbContextManager<T> : BaseDbContextManager<T> where T : BaseDbContext, new()
    {
        public DbContextManager(IOptionsFactory<T> factory) : base(factory)
        {
        }
    }

    public class MultiRegionalDbContextManager<T> : BaseDbContextManager<MultiRegionalDbContext<T>> where T : BaseDbContext, new()
    {
        public MultiRegionalDbContextManager(IOptionsFactory<MultiRegionalDbContext<T>> factory) : base(factory)
        {
        }
    }

    public static class DbContextManagerExtension
    {
        public static IServiceCollection AddDbContextManagerService<T>(this IServiceCollection services) where T : BaseDbContext, new()
        {
            services.TryAddScoped<DbContextManager<T>>();
            services.TryAddScoped<MultiRegionalDbContextManager<T>>();
            services.TryAddScoped<IConfigureOptions<T>, ConfigureDbContext>();
            services.TryAddScoped<IConfigureOptions<MultiRegionalDbContext<T>>, ConfigureMultiRegionalDbContext<T>>();
            //services.TryAddScoped<T>();

            return services;
        }
    }
}
