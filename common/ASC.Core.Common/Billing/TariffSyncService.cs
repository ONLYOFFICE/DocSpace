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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using ASC.Common.Logging;
using ASC.Common.Module;
using ASC.Common.Utils;
using ASC.Core.Data;
using ASC.Core.Tenants;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ASC.Core.Billing
{
    class TariffSyncService : ITariffSyncService, IServiceController
    {
        private readonly ILog log;
        private readonly TariffSyncServiceSection config;
        private readonly IDictionary<int, IEnumerable<TenantQuota>> quotaServices = new Dictionary<int, IEnumerable<TenantQuota>>();
        private Timer timer;


        public TariffSyncService(
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            DbQuotaService dbQuotaService,
            IOptionsMonitor<ILog> options)
        {
            config = TariffSyncServiceSection.GetSection();
            ServiceProvider = serviceProvider;
            Configuration = configuration;
            DbQuotaService = dbQuotaService;
            log = options.CurrentValue;
        }


        // server part of service
        public IEnumerable<TenantQuota> GetTariffs(int version, string key)
        {
            lock (quotaServices)
            {
                if (!quotaServices.ContainsKey(version))
                {
                    var cs = Configuration.GetConnectionStrings(config.ConnectionStringName + version) ??
                             Configuration.GetConnectionStrings(config.ConnectionStringName);
                    quotaServices[version] = DbQuotaService.GetTenantQuotas();
                }
                return quotaServices[version];
            }
        }


        // client part of service
        public string ServiceName
        {
            get { return "Tariffs synchronizer"; }
        }

        public IServiceProvider ServiceProvider { get; }
        public IConfiguration Configuration { get; }
        public DbQuotaService DbQuotaService { get; }

        public void Start()
        {
            if (timer == null)
            {
                timer = new Timer(Sync, null, TimeSpan.Zero, config.Period);
            }
        }

        public void Stop()
        {
            if (timer != null)
            {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                timer.Dispose();
                timer = null;
            }
        }

        private void Sync(object _)
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();
                var tariffSync = scope.ServiceProvider.GetService<TariffSync>();
                tariffSync.Sync();

            }
            catch (Exception error)
            {
                log.Error(error);
            }
        }
    }

    class TariffSync
    {
        public TariffSync(TenantManager tenantManager, CoreSettings coreSettings, DbQuotaService dbQuotaService)
        {
            TenantManager = tenantManager;
            CoreSettings = coreSettings;
            DbQuotaService = dbQuotaService;
        }

        public TenantManager TenantManager { get; }
        public CoreSettings CoreSettings { get; }
        public DbQuotaService DbQuotaService { get; }

        public void Sync()
        {
            var tenant = TenantManager.GetTenants(false).OrderByDescending(t => t.Version).FirstOrDefault();
            if (tenant != null)
            {
                using var wcfClient = new TariffSyncClient();
                var quotaService = DbQuotaService;

                var oldtariffs = quotaService.GetTenantQuotas().ToDictionary(t => t.Id);
                // save new
                foreach (var tariff in wcfClient.GetTariffs(tenant.Version, CoreSettings.GetKey(tenant.TenantId)))
                {
                    quotaService.SaveTenantQuota(tariff);
                    oldtariffs.Remove(tariff.Id);
                }

                // remove old
                foreach (var tariff in oldtariffs.Values)
                {
                    tariff.Visible = false;
                    quotaService.SaveTenantQuota(tariff);
                }
            }
        }
    }
}
