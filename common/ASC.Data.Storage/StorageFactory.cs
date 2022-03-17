/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/

namespace ASC.Data.Storage;

[Singletone(Additional = typeof(StorageConfigExtension))]
public class StorageFactoryConfig
{
    public Configuration.Storage Section { get; }

    public StorageFactoryConfig(IServiceProvider serviceProvider)
    {
        Section = serviceProvider.GetService<Configuration.Storage>();
    }

    public IEnumerable<string> GetModuleList(string configpath, bool exceptDisabledMigration = false)
    {
        return Section.Module
                .Where(x => x.Visible && (!exceptDisabledMigration || !x.DisableMigrate))
            .Select(x => x.Name);
    }

    public IEnumerable<string> GetDomainList(string configpath, string modulename)
    {
        if (Section == null)
        {
            throw new ArgumentException("config section not found");
        }

        return Section.Module
            .Single(x => x.Name.Equals(modulename, StringComparison.OrdinalIgnoreCase))
            .Domain
            .Where(x => x.Visible)
            .Select(x => x.Name);
    }
}

public static class StorageFactoryExtenstion
{
    public static void InitializeHttpHandlers(this IEndpointRouteBuilder builder, string config = null)
    {
        //TODO:
        //if (!HostingEnvironment.IsHosted)
        //{
        //    throw new InvalidOperationException("Application not hosted.");
        //}

        var section = builder.ServiceProvider.GetService<Configuration.Storage>();
        var pathUtils = builder.ServiceProvider.GetService<PathUtils>();
        if (section != null)
        {
            //old scheme
            var discHandler = section.GetHandler("disc");
            if (discHandler != null && section.Module != null)
            {
                var props = discHandler.Property != null ? discHandler.Property.ToDictionary(r => r.Name, r => r.Value) : new Dictionary<string, string>();
                foreach (var m in section.Module.Where(m => m.Type == "disc"))
                {
                    if (m.Path.Contains(Constants.StorageRootParam))
                    {
                        builder.RegisterDiscDataHandler(
                            pathUtils.ResolveVirtualPath(m.VirtualPath),
                            pathUtils.ResolvePhysicalPath(m.Path, props),
                            m.Public);
                    }

                    if (m.Domain != null)
                    {
                        foreach (var d in m.Domain.Where(d => (d.Type == "disc" || string.IsNullOrEmpty(d.Type)) && d.Path.Contains(Constants.StorageRootParam)))
                        {
                            builder.RegisterDiscDataHandler(
                                pathUtils.ResolveVirtualPath(d.VirtualPath),
                                pathUtils.ResolvePhysicalPath(d.Path, props));
                        }
                    }
                }
            }

            //new scheme
            if (section.Module != null)
            {
                foreach (var m in section.Module)
                {
                    //todo: add path criterion
                    if (m.Type == "disc" || !m.Public || m.Path.Contains(Constants.StorageRootParam))
                    {
                        builder.RegisterStorageHandler(
                            m.Name,
                            string.Empty,
                            m.Public);
                    }

                    //todo: add path criterion
                    if (m.Domain != null)
                    {
                        foreach (var d in m.Domain.Where(d => d.Path.Contains(Constants.StorageRootParam)))
                        {
                            builder.RegisterStorageHandler(
                                m.Name,
                                d.Name,
                                d.Public);
                        }
                    }
                }
            }
        }
    }
}

[Scope(Additional = typeof(StorageFactoryExtension))]
public class StorageFactory
{
    private const string DefaultTenantName = "default";

    private readonly StorageFactoryConfig _storageFactoryConfig;
    private readonly SettingsManager _settingsManager;
    private readonly StorageSettingsHelper _storageSettingsHelper;
    private readonly TenantManager _tenantManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IServiceProvider _serviceProvider;

    public StorageFactory(
        IServiceProvider serviceProvider,
        StorageFactoryConfig storageFactoryConfig,
        SettingsManager settingsManager,
        StorageSettingsHelper storageSettingsHelper,
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings)
    {
        _serviceProvider = serviceProvider;
        _storageFactoryConfig = storageFactoryConfig;
        _settingsManager = settingsManager;
        _storageSettingsHelper = storageSettingsHelper;
        _tenantManager = tenantManager;
        _coreBaseSettings = coreBaseSettings;
    }

    public IDataStore GetStorage(string tenant, string module)
    {
        return GetStorage(string.Empty, tenant, module);
    }

    public IDataStore GetStorage(string configpath, string tenant, string module)
    {
        int.TryParse(tenant, out var tenantId);

        return GetStorage(configpath, tenant, module, new TenantQuotaController(tenantId, _tenantManager));
    }

    public IDataStore GetStorage(string configpath, string tenant, string module, IQuotaController controller)
    {
        var tenantId = -2;
        if (string.IsNullOrEmpty(tenant))
        {
            tenant = DefaultTenantName;
        }
        else
        {
            tenantId = Convert.ToInt32(tenant);
        }

        //Make tennant path
        tenant = TenantPath.CreatePath(tenant);

        var section = _storageFactoryConfig.Section;
        if (section == null)
        {
            throw new InvalidOperationException("config section not found");
        }

        var settings = _settingsManager.LoadForTenant<StorageSettings>(tenantId);
        //TODO:GetStoreAndCache
        return GetDataStore(tenant, module, _storageSettingsHelper.DataStoreConsumer(settings), controller);
    }

    public IDataStore GetStorageFromConsumer(string configpath, string tenant, string module, DataStoreConsumer consumer)
    {
        if (tenant == null)
        {
            tenant = DefaultTenantName;
        }

        //Make tennant path
        tenant = TenantPath.CreatePath(tenant);

        var section = _storageFactoryConfig.Section;
        if (section == null)
        {
            throw new InvalidOperationException("config section not found");
        }

        int.TryParse(tenant, out var tenantId);

        return GetDataStore(tenant, module, consumer, new TenantQuotaController(tenantId, _tenantManager));
    }

    private IDataStore GetDataStore(string tenant, string module, DataStoreConsumer consumer, IQuotaController controller)
    {
        var storage = _storageFactoryConfig.Section;
        var moduleElement = storage.GetModuleElement(module);
        if (moduleElement == null)
        {
            throw new ArgumentException("no such module", module);
        }

        var handler = storage.GetHandler(moduleElement.Type);
        Type instanceType;
        IDictionary<string, string> props;

        if (_coreBaseSettings.Standalone &&
            !moduleElement.DisableMigrate &&
            consumer.IsSet)
        {
            instanceType = consumer.HandlerType;
            props = consumer;
        }
        else
        {
            instanceType = Type.GetType(handler.Type, true);
            props = handler.Property.ToDictionary(r => r.Name, r => r.Value);
        }

            
        return ((IDataStore)ActivatorUtilities.CreateInstance(_serviceProvider, instanceType))
            .Configure(tenant, handler, moduleElement, props)
            .SetQuotaController(moduleElement.Count ? controller : null
            /*don't count quota if specified on module*/);
    }
}

public static class StorageFactoryExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<DiscDataStore>();
        services.TryAdd<GoogleCloudStorage>();
        services.TryAdd<RackspaceCloudStorage>();
        services.TryAdd<S3Storage>();
    }
}
