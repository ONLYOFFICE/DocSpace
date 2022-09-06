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

namespace ASC.Common;

public enum DIAttributeEnum
{
    Singletone,
    Scope,
    Transient
}

public class TransientAttribute : DIAttribute
{
    public override DIAttributeEnum DIAttributeEnum => DIAttributeEnum.Transient;

    public TransientAttribute() { }

    public TransientAttribute(Type service) : base(service) { }

    public TransientAttribute(Type service, Type implementation) : base(service, implementation) { }

    public override void TryAdd(IServiceCollection services, Type service, Type implementation = null)
    {
        if (implementation != null)
        {
            services.AddTransient(service, implementation);
        }
        else
        {
            services.AddTransient(service);
        }
    }
}

public class ScopeAttribute : DIAttribute
{
    public override DIAttributeEnum DIAttributeEnum => DIAttributeEnum.Scope;

    public ScopeAttribute() { }

    public ScopeAttribute(Type service) : base(service) { }

    public ScopeAttribute(Type service, Type implementation) : base(service, implementation) { }

    public override void TryAdd(IServiceCollection services, Type service, Type implementation = null)
    {
        if (implementation != null)
        {
            services.AddScoped(service, implementation);
        }
        else
        {
            services.AddScoped(service);
        }
    }
}

public class SingletoneAttribute : DIAttribute
{
    public override DIAttributeEnum DIAttributeEnum => DIAttributeEnum.Singletone;

    public SingletoneAttribute() { }

    public SingletoneAttribute(Type service) : base(service) { }

    public SingletoneAttribute(Type service, Type implementation) : base(service, implementation) { }

    public override void TryAdd(IServiceCollection services, Type service, Type implementation = null)
    {
        if (implementation != null)
        {
            services.AddSingleton(service, implementation);
        }
        else
        {
            services.AddSingleton(service);
        }
    }
}

public abstract class DIAttribute : Attribute
{
    public abstract DIAttributeEnum DIAttributeEnum { get; }
    protected internal Type Implementation { get; }
    protected internal Type Service { get; }
    public Type Additional { get; init; }

    protected DIAttribute() { }

    protected DIAttribute(Type service)
    {
        Service = service;
    }

    protected DIAttribute(Type service, Type implementation)
    {
        Implementation = implementation;
        Service = service;
    }

    public abstract void TryAdd(IServiceCollection services, Type service, Type implementation = null);
}

public class DIHelper
{
    public Dictionary<DIAttributeEnum, List<string>> Services { get; set; }
    public List<string> Added { get; set; }
    public List<string> Configured { get; set; }
    public IServiceCollection ServiceCollection { get; private set; }

    public DIHelper()
    {
        Services = new Dictionary<DIAttributeEnum, List<string>>()
            {
                { DIAttributeEnum.Singletone, new List<string>() },
                { DIAttributeEnum.Scope, new List<string>() },
                { DIAttributeEnum.Transient, new List<string>() }
            };

        Added = new List<string>();
        Configured = new List<string>();
    }

    public DIHelper(IServiceCollection serviceCollection) : this()
    {
        ServiceCollection = serviceCollection;
    }

    public void Configure(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }

    public void RegisterProducts(IConfiguration configuration, string path)
    {
        var types = AutofacExtension.FindAndLoad(configuration, path);

        foreach (var t in types.Select(Type.GetType).Where(r => r != null))
        {
            TryAdd(t);
        }
    }

    public void AddControllers()
    {
        foreach (var a in Assembly.GetEntryAssembly().GetTypes().Where(r => r.IsAssignableTo<ControllerBase>() && !r.IsAbstract))
        {
            _ = TryAdd(a);
        }
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
        if (service.IsInterface && service.IsGenericType && implementation == null &&
            (service.GetGenericTypeDefinition() == typeof(IOptionsSnapshot<>) ||
            service.GetGenericTypeDefinition() == typeof(IOptions<>) ||
            service.GetGenericTypeDefinition() == typeof(IOptionsMonitor<>)
            ))
        {
            service = service.GetGenericArguments().FirstOrDefault();

            if (service == null)
            {
                return false;
            }
        }

        var serviceName = $"{service}{implementation}";

        if (Added.Contains(serviceName))
        {
            return false;
        }

        Added.Add(serviceName);

        var di = service.IsGenericType && (
            service.GetGenericTypeDefinition() == typeof(IConfigureOptions<>) ||
            service.GetGenericTypeDefinition() == typeof(IPostConfigureOptions<>) ||
            service.GetGenericTypeDefinition() == typeof(IOptionsMonitor<>)
            ) && implementation != null ? implementation.GetCustomAttribute<DIAttribute>() : service.GetCustomAttribute<DIAttribute>();

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
                if (!isnew)
                {
                    return false;
                }
            }

            if (service.IsInterface && implementation == null || !service.IsInterface)
            {
                if (di.Service != null)
                {
                    var a = di.Service.GetInterfaces().FirstOrDefault(x => x.IsGenericType && (
                    x.GetGenericTypeDefinition() == typeof(IConfigureOptions<>) ||
                    x.GetGenericTypeDefinition() == typeof(IPostConfigureOptions<>) ||
                    x.GetGenericTypeDefinition() == typeof(IOptionsMonitor<>)
                    ));

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
                            Type c;
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
                        if (di.Implementation == null)
                        {
                            isnew = Register(service, di.Service);
                            TryAdd(di.Service);
                        }
                        else
                        {
                            Register(di.Service);
                        }
                    }
                }

                if (di.Implementation != null)
                {
                    var a = di.Implementation.GetInterfaces().FirstOrDefault(x => x.IsGenericType &&
                    (x.GetGenericTypeDefinition() == typeof(IConfigureOptions<>) ||
                    x.GetGenericTypeDefinition() == typeof(IPostConfigureOptions<>) ||
                    x.GetGenericTypeDefinition() == typeof(IOptionsMonitor<>))
                    );
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
                            Type c;
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

    public DIHelper TryAddSingleton<TService>(Func<IServiceProvider, TService> implementationFactory) where TService : class
    {
        var serviceName = $"{typeof(TService)}";

        if (!Services[DIAttributeEnum.Singletone].Contains(serviceName))
        {
            Services[DIAttributeEnum.Singletone].Add(serviceName);
            ServiceCollection.TryAddSingleton(implementationFactory);
        }

        return this;
    }

    public DIHelper TryAddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService
    {
        var serviceName = $"{typeof(TService)}{typeof(TImplementation)}";

        if (!Services[DIAttributeEnum.Singletone].Contains(serviceName))
        {
            Services[DIAttributeEnum.Singletone].Add(serviceName);
            ServiceCollection.TryAddSingleton<TService, TImplementation>();
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

    private bool Register(Type service, Type implementation = null)
    {
        if (service.IsSubclassOf(typeof(ControllerBase)) || service.GetInterfaces().Contains(typeof(IResourceFilter))
            || service.GetInterfaces().Contains(typeof(IDictionary<string, string>)))
        {
            return true;
        }

        var c = service.IsGenericType && (
            service.GetGenericTypeDefinition() == typeof(IConfigureOptions<>) ||
            service.GetGenericTypeDefinition() == typeof(IPostConfigureOptions<>) ||
            service.GetGenericTypeDefinition() == typeof(IOptionsMonitor<>)
            ) && implementation != null ? implementation.GetCustomAttribute<DIAttribute>() : service.GetCustomAttribute<DIAttribute>();

        var serviceName = $"{service}{implementation}";

        if (!Services[c.DIAttributeEnum].Contains(serviceName))
        {
            c.TryAdd(ServiceCollection, service, implementation);
            Services[c.DIAttributeEnum].Add(serviceName);

            return true;
        }

        return false;
    }
}