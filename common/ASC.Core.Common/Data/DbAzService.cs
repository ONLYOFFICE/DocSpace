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

namespace ASC.Core.Data;

[Scope]
class DbAzService : IAzService
{
    private UserDbContext UserDbContext => _lazyUserDbContext.Value;

    private readonly Lazy<UserDbContext> _lazyUserDbContext;
    private IMapper _mapper;

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
            .ToDictionary(a => string.Concat(a.Tenant.ToString(), a.SubjectId.ToString(), a.ActionId.ToString(), a.ObjectId));

        var tenantAces =
            UserDbContext.Acl
            .Where(r => r.Tenant == tenant)
            .ProjectTo<AzRecord>(_mapper.ConfigurationProvider)
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
        using var tx = UserDbContext.Database.BeginTransaction();

        if (!ExistEscapeRecord(r))
        {
            InsertRecord(r);
        }
        else
        {
            DeleteRecord(r); // unescape
        }

        tx.Commit();

        return r;
    }

    public void RemoveAce(int tenant, AzRecord r)
    {
        r.Tenant = tenant;
        using var tx = UserDbContext.Database.BeginTransaction();

        if (ExistEscapeRecord(r))
        {
            InsertRecord(r); // escape
        }
        else
        {
            DeleteRecord(r);
        }

        tx.Commit();
    }

    private bool ExistEscapeRecord(AzRecord r)
    {
        var count = UserDbContext.Acl
            .Where(a => a.Tenant == Tenant.DefaultTenant)
            .Where(a => a.SubjectId == r.SubjectId)
            .Where(a => a.ActionId == r.ActionId)
            .Where(a => a.ObjectId == (r.ObjectId ?? string.Empty))
            .Where(a => a.Reaction == r.Reaction)
            .Count();

        return count != 0;
    }

    private void DeleteRecord(AzRecord r)
    {
        var record = UserDbContext.Acl
            .Where(a => a.Tenant == r.Tenant)
            .Where(a => a.SubjectId == r.SubjectId)
            .Where(a => a.ActionId == r.ActionId)
            .Where(a => a.ObjectId == (r.ObjectId ?? string.Empty))
            .Where(a => a.Reaction == r.Reaction)
            .FirstOrDefault();

        if (record != null)
        {
            UserDbContext.Acl.Remove(record);
            UserDbContext.SaveChanges();
        }
    }

    private void InsertRecord(AzRecord r)
    {
        UserDbContext.AddOrUpdate(r => r.Acl, _mapper.Map<Acl>(r));
        UserDbContext.SaveChanges();
    }
}
