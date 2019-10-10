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


using ASC.Common.Data;
using ASC.Common.DependencyInjection;
using ASC.Common.Utils;
using ASC.Core.Billing;
using ASC.Core.Caching;
using ASC.Core.Data;
using ASC.Core.Tenants;

using Microsoft.Extensions.Configuration;

namespace ASC.Core
{
    public static class CoreContext
    {
        static CoreContext()
        {
            ConfigureCoreContextByDefault();
        }


        public static CoreConfiguration Configuration { get; private set; }

        public static TenantManager TenantManager { get; private set; }

        private static bool QuotaCacheEnabled(IConfiguration configuration)
        {
            if (configuration["core:enable-quota-cache"] == null)
                return true;

            return !bool.TryParse(configuration["core:enable-quota-cache"], out var enabled) || enabled;
        }

        private static void ConfigureCoreContextByDefault()
        {
            var DbRegistry = CommonServiceProvider.GetService<DbRegistry>();
            var cs = DbRegistry.GetConnectionString("core");
            if (cs == null)
            {
                throw new System.Configuration.ConfigurationErrorsException("Can not configure CoreContext: connection string with name core not found.");
            }

            var configuration = CommonServiceProvider.GetService<IConfiguration>();
            var tenantDomainValidator = CommonServiceProvider.GetService<TenantDomainValidator>();
            var timeZoneConverter = CommonServiceProvider.GetService<TimeZoneConverter>();
            var coreBaseSettings = new CoreBaseSettings(configuration);
            var tenantService = new CachedTenantService(new DbTenantService(cs, DbRegistry, tenantDomainValidator, timeZoneConverter), coreBaseSettings);
            var coreSettings = new CoreSettings(tenantService, coreBaseSettings, configuration);
            var quotaService = QuotaCacheEnabled(configuration) ? (IQuotaService)new CachedQuotaService(new DbQuotaService(cs, DbRegistry)) : new DbQuotaService(cs, DbRegistry);
            var tariffService = new TariffService(cs, quotaService, tenantService, coreBaseSettings, coreSettings, configuration, DbRegistry);

            TenantManager = new TenantManager(tenantService, quotaService, tariffService, null, coreBaseSettings, coreSettings);
            Configuration = new CoreConfiguration(coreBaseSettings, coreSettings, TenantManager, configuration);
        }
    }
}