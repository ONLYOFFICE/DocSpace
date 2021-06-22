using System.Threading.Tasks;

using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.EF;

using Microsoft.AspNetCore.Http;

namespace ASC.CRM.HttpHandlers
{
    public class TenantConfigureMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantConfigureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,
                                      DaoFactory daoFactory,
                                      SettingsManager settingsManager,
                                      CoreConfiguration coreConfiguration)
        {
            CrmDbContextSeed.SeedInitPortalData(settingsManager, daoFactory, coreConfiguration);

            await _next.Invoke(context);
        }
    }
}
