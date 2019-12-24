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

using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Feed.Data
{
    public class FeedReadedDataProvider
    {
        private const string dbId = Constants.FeedDbId;

        public AuthContext AuthContext { get; }
        public TenantManager TenantManager { get; }
        public FeedDbContext FeedDbContext { get; }

        public FeedReadedDataProvider(AuthContext authContext, TenantManager tenantManager, DbContextManager<FeedDbContext> dbContextManager)
        {
            AuthContext = authContext;
            TenantManager = tenantManager;
            FeedDbContext = dbContextManager.Get(dbId);
        }

        public DateTime GetTimeReaded()
        {
            return GetTimeReaded(GetUser(), "all", GetTenant());
        }

        public DateTime GetTimeReaded(string module)
        {
            return GetTimeReaded(GetUser(), module, GetTenant());
        }

        public DateTime GetTimeReaded(Guid user, string module, int tenant)
        {
            return FeedDbContext.FeedReaded
                .Where(r => r.Tenant == tenant)
                .Where(r => r.UserId == user)
                .Where(r => r.Module == module)
                .Max(r => r.TimeStamp);
        }

        public void SetTimeReaded()
        {
            SetTimeReaded(GetUser(), DateTime.UtcNow, "all", GetTenant());
        }

        public void SetTimeReaded(string module)
        {
            SetTimeReaded(GetUser(), DateTime.UtcNow, module, GetTenant());
        }

        public void SetTimeReaded(Guid user)
        {
            SetTimeReaded(user, DateTime.UtcNow, "all", GetTenant());
        }

        public void SetTimeReaded(Guid user, DateTime time, string module, int tenant)
        {
            if (string.IsNullOrEmpty(module)) return;

            var feedReaded = new FeedReaded
            {
                UserId = user,
                TimeStamp = time,
                Module = module,
                Tenant = tenant
            };

            FeedDbContext.AddOrUpdate(r => r.FeedReaded, feedReaded);
            FeedDbContext.SaveChanges();
        }

        public IEnumerable<string> GetReadedModules(DateTime fromTime)
        {
            return GetReadedModules(GetUser(), GetTenant(), fromTime);
        }

        public IEnumerable<string> GetReadedModules(Guid user, int tenant, DateTime fromTime)
        {
            return FeedDbContext.FeedReaded
                .Where(r => r.Tenant == tenant)
                .Where(r => r.UserId == user)
                .Where(r => r.TimeStamp >= fromTime)
                .Select(r => r.Module)
                .ToList();
        }

        private int GetTenant()
        {
            return TenantManager.GetCurrentTenant().TenantId;
        }

        private Guid GetUser()
        {
            return AuthContext.CurrentAccount.ID;
        }
    }

    public static class FeedReadedDataProviderExtension
    {
        public static IServiceCollection AddFeedReadedDataProvider(this IServiceCollection services)
        {
            return services
                .AddAuthContextService()
                .AddTenantManagerService()
                .AddFeedDbService();
        }
    }
}