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
using System.Linq.Expressions;

using ASC.Common;
using ASC.Core.Common.EF;
using ASC.Core.Tenants;

using Microsoft.Extensions.Options;

namespace ASC.Core.Data
{
    [Scope]
    class ConfigureDbQuotaService : IConfigureNamedOptions<DbQuotaService>
    {
        private DbContextManager<CoreDbContext> DbContextManager { get; }
        public string DbId { get; set; }

        public ConfigureDbQuotaService(DbContextManager<CoreDbContext> dbContextManager)
        {
            DbContextManager = dbContextManager;
        }

        public void Configure(string name, DbQuotaService options)
        {
            options.LazyCoreDbContext = new Lazy<CoreDbContext>(() => DbContextManager.Get(name));
        }

        public void Configure(DbQuotaService options)
        {
            options.LazyCoreDbContext = new Lazy<CoreDbContext>(() => DbContextManager.Value);
        }
    }

    [Scope]
    class DbQuotaService : IQuotaService
    {
        private Expression<Func<DbQuota, TenantQuota>> FromDbQuotaToTenantQuota { get; set; }
        private Expression<Func<DbQuotaRow, TenantQuotaRow>> FromDbQuotaRowToTenantQuotaRow { get; set; }
        internal CoreDbContext CoreDbContext { get => LazyCoreDbContext.Value; }
        internal Lazy<CoreDbContext> LazyCoreDbContext { get; set; }

        public DbQuotaService()
        {
            FromDbQuotaToTenantQuota = r => new TenantQuota()
            {
                Id = r.Tenant,
                Name = r.Name,
                ActiveUsers = r.ActiveUsers != 0 ? r.ActiveUsers : int.MaxValue,
                AvangateId = r.AvangateId,
                Features = r.Features,
                MaxFileSize = GetInBytes(r.MaxFileSize),
                MaxTotalSize = GetInBytes(r.MaxTotalSize),
                Price = r.Price,
                Visible = r.Visible
            };

            FromDbQuotaRowToTenantQuotaRow = r => new TenantQuotaRow
            {
                Counter = r.Counter,
                Path = r.Path,
                Tag = r.Tag,
                Tenant = r.Tenant
            };
        }

        public DbQuotaService(DbContextManager<CoreDbContext> dbContextManager) : this()
        {
            LazyCoreDbContext = new Lazy<CoreDbContext>(() => dbContextManager.Value);
        }

        public IEnumerable<TenantQuota> GetTenantQuotas()
        {
            return
                CoreDbContext.Quotas
                .Select(FromDbQuotaToTenantQuota)
                .ToList();
        }

        public TenantQuota GetTenantQuota(int id)
        {
            return
                 CoreDbContext.Quotas
                 .Where(r => r.Tenant == id)
                .Select(FromDbQuotaToTenantQuota)
                .SingleOrDefault();
        }

        public TenantQuota SaveTenantQuota(TenantQuota quota)
        {
            if (quota == null) throw new ArgumentNullException("quota");

            var dbQuota = new DbQuota
            {
                Tenant = quota.Id,
                Name = quota.Name,
                MaxFileSize = GetInMBytes(quota.MaxFileSize),
                MaxTotalSize = GetInMBytes(quota.MaxTotalSize),
                ActiveUsers = quota.ActiveUsers,
                Features = quota.Features,
                Price = quota.Price,
                AvangateId = quota.AvangateId,
                Visible = quota.Visible
            };

            CoreDbContext.AddOrUpdate(r => r.Quotas, dbQuota);
            CoreDbContext.SaveChanges();

            return quota;
        }

        public void RemoveTenantQuota(int id)
        {
            using var tr = CoreDbContext.Database.BeginTransaction();
            var d = CoreDbContext.Quotas
                 .Where(r => r.Tenant == id)
                 .SingleOrDefault();

            if (d != null)
            {
                CoreDbContext.Quotas.Remove(d);
                CoreDbContext.SaveChanges();
            }

            tr.Commit();
        }


        public void SetTenantQuotaRow(TenantQuotaRow row, bool exchange)
        {
            if (row == null) throw new ArgumentNullException("row");

            using var tx = CoreDbContext.Database.BeginTransaction();

            var counter = CoreDbContext.QuotaRows
                .Where(r => r.Path == row.Path && r.Tenant == row.Tenant)
                .Select(r => r.Counter)
                .Take(1)
                .FirstOrDefault();

            var dbQuotaRow = new DbQuotaRow
            {
                Tenant = row.Tenant,
                Path = row.Path,
                Counter = exchange ? counter + row.Counter : row.Counter,
                Tag = row.Tag,
                LastModified = DateTime.UtcNow
            };

            CoreDbContext.AddOrUpdate(r => r.QuotaRows, dbQuotaRow);
            CoreDbContext.SaveChanges();

            tx.Commit();
        }

        public IEnumerable<TenantQuotaRow> FindTenantQuotaRows(int tenantId)
        {
            IQueryable<DbQuotaRow> q = CoreDbContext.QuotaRows;

            if (tenantId != Tenant.DEFAULT_TENANT)
            {
                q = q.Where(r => r.Tenant == tenantId);
            }

            return q.Select(FromDbQuotaRowToTenantQuotaRow).ToList();
        }


        private static long GetInBytes(long bytes)
        {
            const long MB = 1024 * 1024;
            return bytes < MB ? bytes * MB : bytes;
        }

        private static long GetInMBytes(long bytes)
        {
            const long MB = 1024 * 1024;
            return bytes < MB * MB ? bytes / MB : bytes;
        }
    }
}
