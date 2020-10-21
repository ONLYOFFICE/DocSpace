using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using ASC.Common.Threading.Progress;
using ASC.Common.Threading.Workers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ASC.Common
{
    public class TransientAttribute : DIAttribute
    {
        public TransientAttribute()
        {
        }

        public TransientAttribute(Type service) : base(service)
        {
        }

        public TransientAttribute(Type service, Type implementation) : base(service, implementation)
        {

        }
    }

    public class ScopeAttribute : DIAttribute
    {
        public ScopeAttribute()
        {
        }

        public ScopeAttribute(Type service) : base(service)
        {
        }

        public ScopeAttribute(Type service, Type implementation) : base(service, implementation)
        {
        }
    }

    public class SingletoneAttribute : DIAttribute
    {
        public SingletoneAttribute()
        {
        }

        public SingletoneAttribute(Type service) : base(service)
        {
        }

        public SingletoneAttribute(Type service, Type implementation) : base(service, implementation)
        {
        }
    }

    public class DIAttribute : Attribute
    {
        public Type Implementation { get; }
        public Type Service { get; }
        public Type Additional { get; set; }

        public DIAttribute()
        {

        }

        public DIAttribute(Type service)
        {
            Service = service;
        }

        public DIAttribute(Type service, Type implementation)
        {
            Implementation = implementation;
            Service = service;
        }
    }

    public class DIHelper
    {
        public List<string> Added { get; set; }
        public List<string> Singleton { get; set; }
        public List<string> Scoped { get; set; }
        public List<string> Transient { get; set; }
        public List<string> Configured { get; set; }
        public IServiceCollection ServiceCollection { get; private set; }

        public DIHelper()
        {
            Added = new List<string>();
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

        public bool TryAdd<TService, TImplementation>() where TService : class
        {
            return TryAdd(typeof(TService), typeof(TImplementation));
        }

        public bool TryAdd(Type service, Type implementation = null)
        {
            var serviceName = $"{service}{implementation}";
            if (Added.Contains(serviceName)) return false;
            Added.Add(serviceName);

            if (serviceName == "ASC.Core.IAzService")
            {
                var qweasd = 0;
            }

            if (serviceName == "ASC.Core.Caching.CachedAzService")
            {
                var qweasd = 0;
            }

            var di = service.IsGenericType && service.GetGenericTypeDefinition() == typeof(IConfigureOptions<>) && implementation != null ? implementation.GetCustomAttribute<DIAttribute>() : service.GetCustomAttribute<DIAttribute>();
            var isnew = false;

            if (di != null)
            {
                if (di.Additional != null)
                {
                    var m = di.Additional.GetMethod("Register", BindingFlags.Public | BindingFlags.Static);
                    m.Invoke(null, new[] { this });
                }

                if (!service.IsInterface || implementation != null)
                {
                    isnew = implementation != null ? Register(service, implementation) : Register(service);
                    if (!isnew) return false;
                }

                if (service.IsInterface && implementation == null || !service.IsInterface)
                {
                    if (di.Service != null)
                    {
                        var a = di.Service.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConfigureOptions<>));
                        if (a != null)
                        {
                            if (!a.ContainsGenericParameters)
                            {
                                var b = a.GetGenericArguments();

                                foreach (var g in b)
                                {
                                    if (g != service)
                                    {
                                        TryAdd(g);
                                        if (service.IsInterface && di.Implementation == null)
                                        {
                                            TryAdd(service, g);
                                        }
                                    }
                                }

                                TryAdd(a, di.Service);
                            }
                            else
                            {
                                Type c = null;
                                var a1 = a.GetGenericTypeDefinition();
                                var b = a.GetGenericArguments().FirstOrDefault();

                                if (b != null && b.IsGenericType)
                                {
                                    var b1 = b.GetGenericTypeDefinition().MakeGenericType(service.GetGenericArguments());

                                    TryAdd(b1);
                                    c = a1.MakeGenericType(b1);
                                }
                                else
                                {
                                    c = a1.MakeGenericType(service.GetGenericArguments());
                                }

                                TryAdd(c, di.Service.MakeGenericType(service.GetGenericArguments()));
                                //a, di.Service
                            }
                        }
                        else
                        {
                            isnew = di.Implementation == null ? Register(service, di.Service) : Register(di.Service);
                        }
                    }

                    if (di.Implementation != null)
                    {
                        var a = di.Implementation.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConfigureOptions<>));
                        if (a != null)
                        {
                            if (!a.ContainsGenericParameters)
                            {
                                var b = a.GetGenericArguments();

                                foreach (var g in b)
                                {
                                    if (g != service)
                                    {
                                        //TryAdd(g);
                                        if (service.IsInterface && implementation == null)
                                        {
                                            TryAdd(service, g);
                                        }
                                    }
                                }

                                TryAdd(a, di.Implementation);
                            }
                            else
                            {
                                Type c = null;
                                var a1 = a.GetGenericTypeDefinition();
                                var b = a.GetGenericArguments().FirstOrDefault();

                                if (b != null && b.IsGenericType)
                                {
                                    var b1 = b.GetGenericTypeDefinition().MakeGenericType(service.GetGenericArguments());

                                    TryAdd(b1);
                                    c = a1.MakeGenericType(b1);
                                }
                                else
                                {
                                    c = a1.MakeGenericType(service.GetGenericArguments());
                                }

                                TryAdd(c, di.Implementation.MakeGenericType(service.GetGenericArguments()));
                                //a, di.Service
                            }
                        }
                        else
                        {
                            isnew = TryAdd(service, di.Implementation);
                        }
                    }
                }
            }

            if (isnew)
            {
                ConstructorInfo[] props = null;

                if (!service.IsInterface)
                {
                    props = service.GetConstructors();
                }
                else if (implementation != null)
                {
                    props = implementation.GetConstructors();
                }
                else if (di.Service != null)
                {
                    props = di.Service.GetConstructors();
                }

                if (props != null)
                {
                    var par = props.SelectMany(r => r.GetParameters()).Distinct();
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
            if (service.IsSubclassOf(typeof(ControllerBase)) || service.GetInterfaces().Contains(typeof(IResourceFilter))) return true;
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
            else if (c is TransientAttribute)
            {
                if (!Transient.Contains(serviceName))
                {
                    Transient.Add(serviceName);
                    ServiceCollection.TryAddTransient(service);
                    return true;
                }
            }

            return false;
        }

        private bool Register(Type service, Type implementation)
        {
            if (service.IsSubclassOf(typeof(ControllerBase)) || service.GetInterfaces().Contains(typeof(IResourceFilter))) return true;
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
                    ServiceCollection.AddSingleton(service, implementation);
                    return true;
                }
            }
            else if (c is TransientAttribute)
            {
                if (!Transient.Contains(serviceName))
                {
                    Transient.Add(serviceName);
                    ServiceCollection.TryAddTransient(service, implementation);
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
