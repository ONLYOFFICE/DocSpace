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

namespace ASC.Core;

[Scope]
public class AuthorizationManager
{
    private readonly IAzService _service;
    private readonly TenantManager _tenantManager;

    public AuthorizationManager(IAzService service, TenantManager tenantManager)
    {
        _service = service;
        _tenantManager = tenantManager;
    }


    public IEnumerable<AzRecord> GetAces(Guid subjectId, Guid actionId)
    {
        var aces = _service.GetAces(_tenantManager.GetCurrentTenant().Id, default);

        return aces
            .Where(a => a.Action == actionId && (a.Subject == subjectId || subjectId == Guid.Empty))
            .ToList();
    }

    public IEnumerable<AzRecord> GetAces(Guid subjectId, Guid actionId, ISecurityObjectId objectId)
    {
        var aces = _service.GetAces(_tenantManager.GetCurrentTenant().Id, default);

        return FilterAces(aces, subjectId, actionId, objectId)
            .ToList();
    }

    public IEnumerable<AzRecord> GetAcesWithInherits(Guid subjectId, Guid actionId, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider)
    {
        if (objectId == null)
        {
            return GetAces(subjectId, actionId, null);
        }

        var result = new List<AzRecord>();
        var aces = _service.GetAces(_tenantManager.GetCurrentTenant().Id, default);
        result.AddRange(FilterAces(aces, subjectId, actionId, objectId));

        var inherits = new List<AzRecord>();
        var secObjProviderHelper = new AzObjectSecurityProviderHelper(objectId, secObjProvider);
        while (secObjProviderHelper.NextInherit())
        {
            inherits.AddRange(FilterAces(aces, subjectId, actionId, secObjProviderHelper.CurrentObjectId));
        }

        inherits.AddRange(FilterAces(aces, subjectId, actionId, null));

        result.AddRange(DistinctAces(inherits));

        return result;
    }

    public void AddAce(AzRecord r)
    {
        _service.SaveAce(_tenantManager.GetCurrentTenant().Id, r);
    }

    public void RemoveAce(AzRecord r)
    {
        _service.RemoveAce(_tenantManager.GetCurrentTenant().Id, r);
    }

    public void RemoveAllAces(ISecurityObjectId id)
    {
        foreach (var r in GetAces(Guid.Empty, Guid.Empty, id))
        {
            RemoveAce(r);
        }
    }

    private IEnumerable<AzRecord> GetAcesInternal()
    {
        return _service.GetAces(_tenantManager.GetCurrentTenant().Id, default);
    }

    private IEnumerable<AzRecord> DistinctAces(IEnumerable<AzRecord> inheritAces)
    {
        var aces = new Dictionary<string, AzRecord>();
        foreach (var a in inheritAces)
        {
            aces[string.Format("{0}{1}{2:D}", a.Subject, a.Action, a.AceType)] = a;
        }

        return aces.Values;
    }

    private IEnumerable<AzRecord> FilterAces(IEnumerable<AzRecord> aces, Guid subjectId, Guid actionId, ISecurityObjectId objectId)
    {
        var objId = AzObjectIdHelper.GetFullObjectId(objectId);

        return aces is AzRecordStore store ?
            store.Get(objId).Where(a => (a.Subject == subjectId || subjectId == Guid.Empty) && (a.Action == actionId || actionId == Guid.Empty)) :
            aces.Where(a => (a.Subject == subjectId || subjectId == Guid.Empty) && (a.Action == actionId || actionId == Guid.Empty) && a.Object == objId);
    }
}
