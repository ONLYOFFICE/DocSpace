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
using System.Data.Common;
using System.Linq;
using System.Security;

using ASC.Common.Logging;
using ASC.Common.Utils;
using ASC.Core.Billing;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Security.Authentication;
using ASC.Core.Tenants;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using DbContext = ASC.Core.Common.EF.Context.DbContext;

namespace ASC.Core
{
    public class MultiRegionHostedSolution
    {
        private readonly Dictionary<string, HostedSolution> regions = new Dictionary<string, HostedSolution>();
        private readonly string dbid;

        public IConfiguration Configuraion { get; }
        public CookieStorage CookieStorage { get; }
        public EFLoggerFactory LoggerFactory { get; }
        public TariffServiceStorage TariffServiceStorage { get; }
        public IOptionsMonitor<ILog> Options { get; }
        public TenantUtil TenantUtil { get; }
        public TenantDomainValidator TenantDomainValidator { get; }

        public MultiRegionHostedSolution(string dbid,
            IConfiguration configuraion,
            CookieStorage cookieStorage,
            EFLoggerFactory loggerFactory,
            TariffServiceStorage tariffServiceStorage,
            IOptionsMonitor<ILog> options,
            TenantUtil tenantUtil,
            TenantDomainValidator tenantDomainValidator)
        {
            this.dbid = dbid;
            Configuraion = configuraion;
            CookieStorage = cookieStorage;
            LoggerFactory = loggerFactory;
            TariffServiceStorage = tariffServiceStorage;
            Options = options;
            TenantUtil = tenantUtil;
            TenantDomainValidator = tenantDomainValidator;
            Initialize();
        }

        public List<Tenant> GetTenants(DateTime from)
        {
            return GetRegionServices()
                .SelectMany(r => r.GetTenants(from))
                .ToList();
        }

        public List<Tenant> FindTenants(string login)
        {
            return FindTenants(login, null);
        }

        public List<Tenant> FindTenants(string login, string password)
        {
            var result = new List<Tenant>();
            Exception error = null;

            foreach (var service in GetRegionServices())
            {
                try
                {
                    result.AddRange(service.FindTenants(login, password));
                }
                catch (SecurityException exception)
                {
                    error = exception;
                }
            }
            if (!result.Any() && error != null)
            {
                throw error;
            }
            return result;
        }

        public void RegisterTenant(string region, TenantRegistrationInfo ri, out Tenant tenant)
        {
            ri.HostedRegion = region;
            GetRegionService(region).RegisterTenant(ri, out tenant);
        }

        public Tenant GetTenant(string domain)
        {
            foreach (var service in GetRegionServices())
            {
                var tenant = service.GetTenant(domain);
                if (tenant != null)
                {
                    return tenant;
                }
            }
            return null;
        }

        public Tenant GetTenant(string region, int tenantId)
        {
            return GetRegionService(region).GetTenant(tenantId);
        }

        public Tenant SaveTenant(string region, Tenant tenant)
        {
            return GetRegionService(region).SaveTenant(tenant);
        }


        public string CreateAuthenticationCookie(string region, int tenantId, string login, string password)
        {
            return GetRegionService(region).CreateAuthenticationCookie(CookieStorage, tenantId, login, password);
        }

        public string CreateAuthenticationCookie(string region, int tenantId, Guid userId)
        {
            return GetRegionService(region).CreateAuthenticationCookie(CookieStorage, tenantId, userId);
        }


        public Tariff GetTariff(string region, int tenantId, bool withRequestToPaymentSystem = true)
        {
            return GetRegionService(region).GetTariff(tenantId, withRequestToPaymentSystem);
        }

        public void SetTariff(string region, int tenant, bool paid)
        {
            GetRegionService(region).SetTariff(tenant, paid);
        }

        public void SetTariff(string region, int tenant, Tariff tariff)
        {
            GetRegionService(region).SetTariff(tenant, tariff);
        }

        public void SaveButton(string region, int tariffId, string partnerId, string buttonUrl)
        {
            GetRegionService(region).SaveButton(tariffId, partnerId, buttonUrl);
        }

        public TenantQuota GetTenantQuota(string region, int tenant)
        {
            return GetRegionService(region).GetTenantQuota(tenant);
        }

        public void CheckTenantAddress(string address)
        {
            foreach (var service in GetRegionServices())
            {
                service.CheckTenantAddress(address);
            }
        }

        public IEnumerable<string> GetRegions()
        {
            return GetRegionServices().Select(s => s.Region).ToList();
        }



        private IEnumerable<HostedSolution> GetRegionServices()
        {
            return regions.Where(x => !string.IsNullOrEmpty(x.Key))
                   .Select(x => x.Value);
        }

        private HostedSolution GetRegionService(string region)
        {
            return regions[region];
        }

        private void Initialize()
        {
            var connectionStrings = Configuraion.GetConnectionStrings();
            var dbConnectionStrings = Configuraion.GetConnectionStrings(dbid);

            if (Convert.ToBoolean(Configuraion["core.multi-hosted.config-only"] ?? "false"))
            {
                foreach (var cs in Configuraion.GetConnectionStrings())
                {
                    if (cs.Name.StartsWith(dbid + "."))
                    {
                        var name = cs.Name.Substring(dbid.Length + 1);
                        regions[name] = new HostedSolution(Configuraion, cs, TariffServiceStorage, Options, TenantUtil, TenantDomainValidator, new TenantDbContext(GetOptions<TenantDbContext>(cs.ConnectionString)), new UserDbContext(GetOptions<UserDbContext>(cs.ConnectionString)), new CoreDbContext(GetOptions<CoreDbContext>(cs.ConnectionString)), name);
                    }
                }

                regions[dbid] = new HostedSolution(Configuraion, dbConnectionStrings, TariffServiceStorage, Options, TenantUtil, TenantDomainValidator, new TenantDbContext(GetOptions<TenantDbContext>(dbConnectionStrings.ConnectionString)), new UserDbContext(GetOptions<UserDbContext>(dbConnectionStrings.ConnectionString)), new CoreDbContext(GetOptions<CoreDbContext>(dbConnectionStrings.ConnectionString)));
                if (!regions.ContainsKey(string.Empty))
                {
                    regions[string.Empty] = new HostedSolution(Configuraion, dbConnectionStrings, TariffServiceStorage, Options, TenantUtil, TenantDomainValidator, new TenantDbContext(GetOptions<TenantDbContext>(dbConnectionStrings.ConnectionString)), new UserDbContext(GetOptions<UserDbContext>(dbConnectionStrings.ConnectionString)), new CoreDbContext(GetOptions<CoreDbContext>(dbConnectionStrings.ConnectionString)));
                }
            }
            else
            {

                var find = false;
                foreach (var cs in connectionStrings)
                {
                    var dbContextOptionsBuilder = new DbContextOptionsBuilder<DbContext>();
                    var options = dbContextOptionsBuilder
                        .UseMySql(cs.ConnectionString)
                        .UseLoggerFactory(LoggerFactory)
                        .Options;

                    using var dbContext = new DbContext(options);

                    if (cs.Name.StartsWith(dbid + "."))
                    {
                        var name = cs.Name.Substring(dbid.Length + 1);
                        regions[name] = new HostedSolution(Configuraion, cs, TariffServiceStorage, Options, TenantUtil, TenantDomainValidator, new TenantDbContext(GetOptions<TenantDbContext>(dbConnectionStrings.ConnectionString)), new UserDbContext(GetOptions<UserDbContext>(dbConnectionStrings.ConnectionString)), new CoreDbContext(GetOptions<CoreDbContext>(dbConnectionStrings.ConnectionString)), name);
                        find = true;
                    }
                }
                if (find)
                {
                    regions[dbid] = new HostedSolution(Configuraion, dbConnectionStrings, TariffServiceStorage, Options, TenantUtil, TenantDomainValidator, new TenantDbContext(GetOptions<TenantDbContext>(dbConnectionStrings.ConnectionString)), new UserDbContext(GetOptions<UserDbContext>(dbConnectionStrings.ConnectionString)), new CoreDbContext(GetOptions<CoreDbContext>(dbConnectionStrings.ConnectionString)));
                    if (!regions.ContainsKey(string.Empty))
                    {
                        regions[string.Empty] = new HostedSolution(Configuraion, dbConnectionStrings, TariffServiceStorage, Options, TenantUtil, TenantDomainValidator, new TenantDbContext(GetOptions<TenantDbContext>(dbConnectionStrings.ConnectionString)), new UserDbContext(GetOptions<UserDbContext>(dbConnectionStrings.ConnectionString)), new CoreDbContext(GetOptions<CoreDbContext>(dbConnectionStrings.ConnectionString)));
                    }
                }
                else
                {
                    foreach (var connectionString in connectionStrings)
                    {
                        try
                        {
                            var dbContextOptionsBuilder = new DbContextOptionsBuilder<DbContext>();
                            var options = dbContextOptionsBuilder
                                .UseMySql(connectionString.ConnectionString)
                                .UseLoggerFactory(LoggerFactory)
                                .Options;

                            using var dbContext = new DbContext(options);

                            var q = dbContext.Regions.ToList();

                            foreach (var r in q)
                            {
                                var cs = new System.Configuration.ConnectionStringSettings(r.Region, r.ConnectionString, r.Provider);

                                if (!regions.ContainsKey(string.Empty))
                                {
                                    regions[string.Empty] = new HostedSolution(Configuraion, cs, TariffServiceStorage, Options, TenantUtil, TenantDomainValidator, new TenantDbContext(GetOptions<TenantDbContext>(cs.ConnectionString)), new UserDbContext(GetOptions<UserDbContext>(cs.ConnectionString)), new CoreDbContext(GetOptions<CoreDbContext>(cs.ConnectionString)), cs.Name);
                                }

                                regions[cs.Name] = new HostedSolution(Configuraion, cs, TariffServiceStorage, Options, TenantUtil, TenantDomainValidator, new TenantDbContext(GetOptions<TenantDbContext>(cs.ConnectionString)), new UserDbContext(GetOptions<UserDbContext>(cs.ConnectionString)), new CoreDbContext(GetOptions<CoreDbContext>(cs.ConnectionString)), cs.Name);
                            }
                        }
                        catch (DbException) { }
                    }
                }
            }
        }

        private DbContextOptions<T> GetOptions<T>(string cs) where T : BaseDbContext
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<T>();
            return dbContextOptionsBuilder
                .UseMySql(cs)
                .UseLoggerFactory(LoggerFactory)
                .Options;
        }
    }
}