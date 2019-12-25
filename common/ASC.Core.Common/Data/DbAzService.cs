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

using ASC.Core.Common.EF;
using ASC.Core.Tenants;

namespace ASC.Core.Data
{
    class DbAzService : IAzService
    {
        public Expression<Func<Acl, AzRecord>> FromAclToAzRecord { get; set; }

        private CoreDbContext CoreDbContext { get; set; }

        public DbAzService(DbContextManager<CoreDbContext> dbContextManager)
        {
            CoreDbContext = dbContextManager.Value;
            FromAclToAzRecord = r => new AzRecord()
            {
                ActionId = r.Action,
                ObjectId = r.Object,
                Reaction = r.AceType,
                SubjectId = r.Subject,
                Tenant = r.Tenant
            };
        }

        public IEnumerable<AzRecord> GetAces(int tenant, DateTime from)
        {
            // row with tenant = -1 - common for all tenants, but equal row with tenant != -1 escape common row for the portal
            var commonAces =
                CoreDbContext.Acl
                .Where(r => r.Tenant == Tenant.DEFAULT_TENANT)
                .Select(FromAclToAzRecord)
                .ToDictionary(a => string.Concat(a.Tenant.ToString(), a.SubjectId.ToString(), a.ActionId.ToString(), a.ObjectId));

            var tenantAces =
                CoreDbContext.Acl
                .Where(r => r.Tenant == tenant)
                .Select(FromAclToAzRecord)
                .ToList();

            // remove excaped rows
            foreach (var a in tenantAces)
            {
                var key = string.Concat(a.Tenant.ToString(), a.SubjectId.ToString(), a.ActionId.ToString(), a.ObjectId);
                if (commonAces.ContainsKey(key))
                {
                    var common = commonAces[key];
                    commonAces.Remove(key);
                    if (common.Reaction == a.Reaction)
                    {
                        tenantAces.Remove(a);
                    }
                }
            }

            return commonAces.Values.Concat(tenantAces);
        }

        public AzRecord SaveAce(int tenant, AzRecord r)
        {
            r.Tenant = tenant;
            using var tx = CoreDbContext.Database.BeginTransaction();
            if (!ExistEscapeRecord(r))
            {
                InsertRecord(r);
            }
            else
            {
                // unescape
                DeleteRecord(r);
            }
            tx.Commit();

            return r;
        }

        public void RemoveAce(int tenant, AzRecord r)
        {
            r.Tenant = tenant;
            using var tx = CoreDbContext.Database.BeginTransaction();
            if (ExistEscapeRecord(r))
            {
                // escape
                InsertRecord(r);
            }
            else
            {
                DeleteRecord(r);
            }
            tx.Commit();
        }


        private bool ExistEscapeRecord(AzRecord r)
        {
            var count = CoreDbContext.Acl
                .Where(a => a.Tenant == Tenant.DEFAULT_TENANT)
                .Where(a => a.Subject == r.SubjectId)
                .Where(a => a.Action == r.ActionId)
                .Where(a => a.Object == (r.ObjectId ?? string.Empty))
                .Where(a => a.AceType == r.Reaction)
                .Count();

            return count != 0;
        }

        private void DeleteRecord(AzRecord r)
        {
            using var tr = CoreDbContext.Database.BeginTransaction();
            var record = CoreDbContext.Acl
                .Where(a => a.Tenant == r.Tenant)
                .Where(a => a.Subject == r.SubjectId)
                .Where(a => a.Action == r.ActionId)
                .Where(a => a.Object == (r.ObjectId ?? string.Empty))
                .Where(a => a.AceType == r.Reaction)
                .FirstOrDefault();

            CoreDbContext.Acl.Remove(record);
            CoreDbContext.SaveChanges();
            tr.Commit();
        }

        private void InsertRecord(AzRecord r)
        {
            var record = new Acl
            {
                AceType = r.Reaction,
                Action = r.ActionId,
                Object = r.ObjectId,
                Subject = r.SubjectId,
                Tenant = r.Tenant
            };

            CoreDbContext.AddOrUpdate(r => r.Acl, record);
            CoreDbContext.SaveChanges();
        }
    }
}
