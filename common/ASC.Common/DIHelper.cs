using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ASC.Common.Threading.Progress;
using ASC.Common.Threading.Workers;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ASC.Common
{
    public class ScopeAttribute : DIAttribute
    {


        public ScopeAttribute()
        {

        }

        public ScopeAttribute(Type type) : base(type)
        {
        }

        public ScopeAttribute(Type type, Type cachedType) : base(type, cachedType)
        {

        }
    }

    public class SingletoneAttribute : DIAttribute
    {
        public SingletoneAttribute()
        {

        }

        public SingletoneAttribute(Type type) : base(type)
        {
        }

        public SingletoneAttribute(Type type, Type cachedType) : base(type, cachedType)
        {

        }
    }

    public class DIAttribute : Attribute
    {
        public Type CachedType { get; }
        public Type Type { get; }

        public DIAttribute()
        {

        }

        public DIAttribute(Type type)
        {
            Type = type;
        }

        public DIAttribute(Type type, Type cachedType)
        {
            CachedType = cachedType;
            Type = type;
        }
    }

    public class DIHelper
    {
        public List<string> Singleton { get; set; }
        public List<string> Scoped { get; set; }
        public List<string> Transient { get; set; }
        public List<string> Configured { get; set; }
        public IServiceCollection ServiceCollection { get; private set; }

        public DIHelper()
        {
            Singleton = new List<string>();
            Scoped = new List<string>();
            Transient = new List<string>();
            Configured = new List<string>();
        }

        public DIHelper(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        public void Configure(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        public bool TryAddScoped<TService>() where TService : class
        {
            var serviceName = $"{typeof(TService)}";
            if (!Scoped.Contains(serviceName))
            {
                Scoped.Add(serviceName);
                ServiceCollection.TryAddScoped<TService>();
                return true;
            }
            return false;
        }

        public bool TryAdd<TService>() where TService : class
        {
            return TryAdd(typeof(TService));
        }

        public bool TryAdd(Type service, Type implementation = null)
        {
            var di = service.IsGenericType && service.GetGenericTypeDefinition() == typeof(IConfigureOptions<>) && implementation != null ? implementation.GetCustomAttribute<DIAttribute>() : service.GetCustomAttribute<DIAttribute>();
            var isnew = false;

            if (di != null)
            {
                if (!service.IsInterface || implementation != null)
                {
                    isnew = implementation != null ? Register(service, implementation) : Register(service);
                }

                if (service.IsInterface && implementation == null || !service.IsInterface)
                {
                    if (di.Type != null)
                    {
                        var a = di.Type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConfigureOptions<>));
                        if (a != null)
                        {
                            var b = a.GetGenericArguments();

                            foreach (var g in b)
                            {
                                TryAdd(g);
                            }

                            TryAdd(a, di.Type);
                        }
                        else
                        {
                            Register(service, di.Type);
                        }
                    }

                    if (di.CachedType != null)
                    {
                        var a = di.CachedType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConfigureOptions<>));
                        if (a != null)
                        {
                            var b = a.GetGenericArguments();

                            foreach (var g in b)
                            {
                                TryAdd(service, g);
                            }

                            TryAdd(a, di.CachedType);
                        }
                        else
                        {
                            Register(service, di.CachedType);
                        }
                    }

                }
            }

            if (isnew)
            {
                var props = service.GetConstructors();

                foreach (var p in props)
                {
                    var par = p.GetParameters();

                    foreach (var p1 in par)
                    {
                        TryAdd(p1.ParameterType);
                    }
                }
            }

            return isnew;
        }

        private bool Register(Type service)
        {
            var c = service.GetCustomAttribute<DIAttribute>();
            var serviceName = $"{service}";
            if (c is ScopeAttribute)
            {
                if (!Scoped.Contains(serviceName))
                {
                    Scoped.Add(serviceName);
                    ServiceCollection.TryAddScoped(service);
                    return true;
                }
            }
            else if (c is SingletoneAttribute)
            {
                if (!Singleton.Contains(serviceName))
                {
                    Singleton.Add(serviceName);
                    ServiceCollection.TryAddSingleton(service);
                    return true;
                }
            }

            return false;
        }

        private bool Register(Type service, Type implementation)
        {
            var c = service.IsGenericType && service.GetGenericTypeDefinition() == typeof(IConfigureOptions<>) && implementation != null ? implementation.GetCustomAttribute<DIAttribute>() : service.GetCustomAttribute<DIAttribute>();
            var serviceName = $"{service}{implementation}";
            if (c is ScopeAttribute)
            {
                if (!Scoped.Contains(serviceName))
                {
                    Scoped.Add(serviceName);
                    ServiceCollection.TryAddScoped(service, implementation);
                    return true;
                }
            }
            else if (c is SingletoneAttribute)
            {
                if (!Singleton.Contains(serviceName))
                {
                    Singleton.Add(serviceName);
                    ServiceCollection.TryAddSingleton(service, implementation);
                    return true;
                }
            }

            return false;
        }

        public bool TryAddScoped<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            var serviceName = $"{typeof(TService)}{typeof(TImplementation)}";
            if (!Scoped.Contains(serviceName))
            {
                Scoped.Add(serviceName);
                ServiceCollection.TryAddScoped<TService, TImplementation>();
                return true;
            }

            return false;
        }

        public bool TryAddScoped<TService, TImplementation>(TService tservice, TImplementation tImplementation) where TService : Type where TImplementation : Type
        {
            var serviceName = $"{tservice}{tImplementation}";
            if (!Scoped.Contains(serviceName))
            {
                Scoped.Add(serviceName);
                ServiceCollection.TryAddScoped(tservice, tImplementation);
                return true;
            }

            return false;
        }


        public DIHelper TryAddSingleton<TService>() where TService : class
        {
            var serviceName = $"{typeof(TService)}";
            if (!Singleton.Contains(serviceName))
            {
                Singleton.Add(serviceName);
                ServiceCollection.TryAddSingleton<TService>();
            }

            return this;
        }

        public DIHelper TryAddSingleton<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class
        {
            var serviceName = $"{typeof(TService)}";
            if (!Singleton.Contains(serviceName))
            {
                Singleton.Add(serviceName);
                ServiceCollection.TryAddSingleton(implementationFactory);
            }

            return this;
        }

        public DIHelper TryAddSingleton<TService>(TService t) where TService : class
        {
            var serviceName = $"{typeof(TService)}";
            if (!Singleton.Contains(serviceName))
            {
                Singleton.Add(serviceName);
                ServiceCollection.TryAddSingleton(t);
            }

            return this;
        }

        public DIHelper TryAddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            var serviceName = $"{typeof(TService)}{typeof(TImplementation)}";
            if (!Singleton.Contains(serviceName))
            {
                Singleton.Add(serviceName);
                ServiceCollection.TryAddSingleton<TService, TImplementation>();
            }

            return this;
        }

        public DIHelper AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            var serviceName = $"{typeof(TService)}{typeof(TImplementation)}";
            if (!Singleton.Contains(serviceName))
            {
                Singleton.Add(serviceName);
                ServiceCollection.AddSingleton<TService, TImplementation>();
            }

            return this;
        }

        public DIHelper TryAddSingleton<TService, TImplementation>(TService tservice, TImplementation tImplementation) where TService : Type where TImplementation : Type
        {
            var serviceName = $"{tservice}{tImplementation}";
            if (!Singleton.Contains(serviceName))
            {
                Singleton.Add(serviceName);
                ServiceCollection.TryAddSingleton(tservice, tImplementation);
            }
            return this;
        }


        public DIHelper TryAddTransient<TService>() where TService : class
        {
            var serviceName = $"{typeof(TService)}";
            if (!Transient.Contains(serviceName))
            {
                Transient.Add(serviceName);
                ServiceCollection.TryAddTransient<TService>();
            }

            return this;
        }

        public DIHelper Configure<TOptions>(Action<TOptions> configureOptions) where TOptions : class
        {
            var serviceName = $"{typeof(TOptions)}";
            if (!Configured.Contains(serviceName))
            {
                Configured.Add(serviceName);
                ServiceCollection.Configure(configureOptions);
            }

            return this;
        }

        private void AddToConfigured<TOptions>(string type, Action<TOptions> action) where TOptions : class
        {
            if (!Configured.Contains(type))
            {
                Configured.Add(type);
                ServiceCollection.Configure(action);
            }
        }

        public DIHelper AddWorkerQueue<T1>(int workerCount, int waitInterval, bool stopAfterFinsih, int errorCount)
        {
            void action(WorkerQueue<T1> a)
            {
                a.workerCount = workerCount;
                a.waitInterval = waitInterval;
                a.stopAfterFinsih = stopAfterFinsih;
                a.errorCount = errorCount;
            }
            AddToConfigured($"{typeof(WorkerQueue<T1>)}", (Action<WorkerQueue<T1>>)action);
            return this;
        }
        public DIHelper AddProgressQueue<T1>(int workerCount, int waitInterval, bool removeAfterCompleted, bool stopAfterFinsih, int errorCount) where T1 : class, IProgressItem
        {
            void action(ProgressQueue<T1> a)
            {
                a.workerCount = workerCount;
                a.waitInterval = waitInterval;
                a.stopAfterFinsih = stopAfterFinsih;
                a.errorCount = errorCount;
                a.removeAfterCompleted = removeAfterCompleted;
            }
            AddToConfigured($"{typeof(ProgressQueue<T1>)}", (Action<ProgressQueue<T1>>)action);
            return this;
        }
        public DIHelper Configure<TOptions>(string name, Action<TOptions> configureOptions) where TOptions : class
        {
            var serviceName = $"{typeof(TOptions)}{name}";
            if (!Configured.Contains(serviceName))
            {
                Configured.Add(serviceName);
                ServiceCollection.Configure(name, configureOptions);
            }

            return this;
        }
    }
}
