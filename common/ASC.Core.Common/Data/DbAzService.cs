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

namespace ASC.Core.Data;

[Scope]
class DbAzService : IAzService
{
    private UserDbContext UserDbContext => _lazyUserDbContext.Value;
    private readonly Lazy<UserDbContext> _lazyUserDbContext;
    private readonly IMapper _mapper;

    public DbAzService(DbContextManager<UserDbContext> dbContextManager, IMapper mapper)
    {
        _lazyUserDbContext = new Lazy<UserDbContext>(() => dbContextManager.Value);
        _mapper = mapper;
    }

    public IEnumerable<AzRecord> GetAces(int tenant, DateTime from)
    {
        // row with tenant = -1 - common for all tenants, but equal row with tenant != -1 escape common row for the portal
        var commonAces =
            UserDbContext.Acl
            .Where(r => r.Tenant == Tenant.DefaultTenant)
            .ProjectTo<AzRecord>(_mapper.ConfigurationProvider)
            .ToDictionary(a => string.Concat(a.Tenant.ToString(), a.Subject.ToString(), a.Action.ToString(), a.Object));

        var tenantAces =
            UserDbContext.Acl
            .Where(r => r.Tenant == tenant)
            .ProjectTo<AzRecord>(_mapper.ConfigurationProvider)
            .ToList();

        // remove excaped rows
        foreach (var a in tenantAces)
        {
            var key = string.Concat(a.Tenant.ToString(), a.Subject.ToString(), a.Action.ToString(), a.Object);
            if (commonAces.TryGetValue(key, out var common))
            {
                commonAces.Remove(key);
                if (common.AceType == a.AceType)
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

        var strategy = UserDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tx = UserDbContext.Database.BeginTransaction();

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
        });

        return r;
    }

    public void RemoveAce(int tenant, AzRecord r)
    {
        r.Tenant = tenant;

        var strategy = UserDbContext.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tx = UserDbContext.Database.BeginTransaction();

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
        });

    }


    private bool ExistEscapeRecord(AzRecord r)
    {
        return UserDbContext.Acl
            .Where(a => a.Tenant == Tenant.DefaultTenant)
            .Where(a => a.Subject == r.Subject)
            .Where(a => a.Action == r.Action)
            .Where(a => a.Object == (r.Object ?? string.Empty))
            .Where(a => a.AceType == r.AceType)
            .Any();
    }

    private void DeleteRecord(AzRecord r)
    {
        var record = UserDbContext.Acl
            .Where(a => a.Tenant == r.Tenant)
            .Where(a => a.Subject == r.Subject)
            .Where(a => a.Action == r.Action)
            .Where(a => a.Object == (r.Object ?? string.Empty))
            .Where(a => a.AceType == r.AceType)
            .FirstOrDefault();

        if (record != null)
        {
            UserDbContext.Acl.Remove(record);
            UserDbContext.SaveChanges();
        }
    }

    private void InsertRecord(AzRecord r)
    {
        UserDbContext.AddOrUpdate(r => r.Acl, _mapper.Map<AzRecord, Acl>(r));
        UserDbContext.SaveChanges();
    }
}
