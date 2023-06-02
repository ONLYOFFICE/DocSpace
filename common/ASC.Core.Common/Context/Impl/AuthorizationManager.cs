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


    public async Task<IEnumerable<AzRecord>> GetAcesAsync(Guid subjectId, Guid actionId)
    {
        var aces = await _service.GetAcesAsync(await _tenantManager.GetCurrentTenantIdAsync(), default);

        return aces
            .Where(a => a.Action == actionId && (a.Subject == subjectId || subjectId == Guid.Empty))
            .ToList();
    }

    public async Task<IEnumerable<AzRecord>> GetAcesAsync(Guid subjectId, Guid actionId, ISecurityObjectId objectId)
    {
        var aces = await _service.GetAcesAsync(await _tenantManager.GetCurrentTenantIdAsync(), default);

        return FilterAces(aces, subjectId, actionId, objectId)
            .ToList();
    }

    public async Task<IEnumerable<AzRecord>> GetAcesWithInheritsAsync(Guid subjectId, Guid actionId, ISecurityObjectId objectId, ISecurityObjectProvider secObjProvider)
    {
        if (objectId == null)
        {
            return await GetAcesAsync(subjectId, actionId, null);
        }

        var result = new List<AzRecord>();
        var aces = await _service.GetAcesAsync(await _tenantManager.GetCurrentTenantIdAsync(), default);
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

    public async Task AddAceAsync(AzRecord r)
    {
        await _service.SaveAceAsync(await _tenantManager.GetCurrentTenantIdAsync(), r);
    }

    public async Task RemoveAceAsync(AzRecord r)
    {
        await _service.RemoveAceAsync(await _tenantManager.GetCurrentTenantIdAsync(), r);
    }

    public async Task RemoveAllAcesAsync(ISecurityObjectId id)
    {
        foreach (var r in await GetAcesAsync(Guid.Empty, Guid.Empty, id))
        {
            await RemoveAceAsync(r);
        }
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
