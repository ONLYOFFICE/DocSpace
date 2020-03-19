using ASC.Api.Core;
using ASC.Common;
using ASC.Common.Caching;
using ASC.Core;
using ASC.Web.Studio.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Calendar.Core
{
    public class ExportDataCache
    {
        public static readonly ICache Cache = AscCache.Memory;

        private TenantManager TenantManager { get; }
        public ExportDataCache(
            TenantManager tenantManager)
        {
            TenantManager = tenantManager;
        }
        public String GetCacheKey(string calendarId)
        {
            return String.Format("{0}_ExportCalendar_{1}", TenantManager.GetCurrentTenant().TenantId, calendarId);
        }

        public string Get(string calendarId)
        {
            return Cache.Get<string>(GetCacheKey(calendarId));
        }

        public void Insert(string calendarId, string data)
        {
            if (string.IsNullOrEmpty(data))
                Reset(calendarId);
            else
                Cache.Insert(GetCacheKey(calendarId), data, TimeSpan.FromMinutes(5));
        }

        public void Reset(string calendarId)
        {
            Cache.Remove(GetCacheKey(calendarId));
        }
    }

    public static class ExportDataCacheExtention
    {
        public static DIHelper AddExportDataCache(this DIHelper services)
        {
            services.TryAddScoped<ExportDataCache>();
            
            return services
                .AddApiContextService()
                .AddSecurityContextService()
                .AddPermissionContextService()
                .AddTenantManagerService();
        }
    }
}
