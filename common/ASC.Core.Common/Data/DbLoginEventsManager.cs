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
public class DbLoginEventsManager
{
    private const string GuidLoginEvent = "F4D8BBF6-EB63-4781-B55E-5885EAB3D759";
    private static readonly TimeSpan _expirationTimeout = TimeSpan.FromMinutes(5);
    private static readonly List<int> _loginActions = new List<int>
    {
        (int)MessageAction.LoginSuccess,
        (int)MessageAction.LoginSuccessViaSocialAccount,
        (int)MessageAction.LoginSuccessViaSms,
        (int)MessageAction.LoginSuccessViaApi,
        (int)MessageAction.LoginSuccessViaSocialApp,
        (int)MessageAction.LoginSuccessViaApiSms,
        (int)MessageAction.LoginSuccessViaSSO,
        (int)MessageAction.LoginSuccessViaApiSocialAccount,
        (int)MessageAction.LoginSuccesViaTfaApp,
        (int)MessageAction.LoginSuccessViaApiTfa
    };

    private readonly ICache _cache;
    private readonly TenantManager _tenantManager;
    private readonly SecurityContext _securityContext;
    private readonly TenantUtil _tenantUtil;
    private readonly IMapper _mapper;

    private MessagesContext LoginEventContext => _lazyLoginEventContext.Value;
    private readonly Lazy<MessagesContext> _lazyLoginEventContext;

    public DbLoginEventsManager(
        ICache cache,
        TenantManager tenantManager,
        SecurityContext securityContext,
        DbContextManager<MessagesContext> dbContextManager,
        TenantUtil tenantUtil,
        IMapper mapper)
    {
        _cache = cache;
        _tenantManager = tenantManager;
        _securityContext = securityContext;
        _tenantUtil = tenantUtil;
        _mapper = mapper;
        _lazyLoginEventContext = new Lazy<MessagesContext>(() => dbContextManager.Value);
    }

    public async Task<List<int>> GetLoginEventIds(int tenantId, Guid userId)
    {
        var commonKey = GetCacheKey(tenantId, userId);
        var cacheKeys = _cache.Get<List<int>>(commonKey);
        if (cacheKeys != null)
        {
            return cacheKeys;
        }

        var date = DateTime.UtcNow.AddYears(-1);
        var resultList = await LoginEventContext.LoginEvents
            .Where(r => r.TenantId == tenantId && r.UserId == userId && _loginActions.Contains(r.Action) && r.Date >= date && r.Active)
            .Select(r => r.Id)
            .ToListAsync();

        if (resultList != null)
        {
            _cache.Insert(commonKey, resultList, _expirationTimeout);
        }

        return resultList;
    }

    public async Task<List<BaseEvent>> GetLoginEvents(int tenantId, Guid userId)
    {
        var date = DateTime.UtcNow.AddYears(-1);
        var loginInfo = await LoginEventContext.LoginEvents
            .Where(r => r.TenantId == tenantId && r.UserId == userId && _loginActions.Contains(r.Action) && r.Date >= date && r.Active)
            .OrderByDescending(r => r.Id)
            .ToListAsync();

        return _mapper.Map<List<LoginEvent>, List<BaseEvent>>(loginInfo);
    }

    public async Task LogOutEvent(int loginEventId)
    {
        var events = await LoginEventContext.LoginEvents
           .Where(r => r.Id == loginEventId)
           .ToListAsync();

        foreach (var e in events)
        {
            e.Active = false;
        }

        await LoginEventContext.SaveChangesAsync();

        ResetCache();
    }

    public async Task LogOutAllActiveConnections(int tenantId, Guid userId)
    {
        var events = await LoginEventContext.LoginEvents
              .Where(r => r.TenantId == tenantId && r.UserId == userId && r.Active)
              .ToListAsync();

        foreach (var e in events)
        {
            e.Active = false;
        }

        await LoginEventContext.SaveChangesAsync();

        ResetCache(tenantId, userId);
    }

    public async Task LogOutAllActiveConnectionsForTenant(int tenantId)
    {
        var events = await LoginEventContext.LoginEvents
              .Where(r => r.TenantId == tenantId && r.Active)
              .ToListAsync();

        foreach (var e in events)
        {
            e.Active = false;
        }

        await LoginEventContext.SaveChangesAsync();
    }

    public async Task LogOutAllActiveConnectionsExceptThis(int loginEventId, int tenantId, Guid userId)
    {
        var events = await LoginEventContext.LoginEvents
            .Where(r => r.TenantId == tenantId && r.UserId == userId && r.Id != loginEventId && r.Active)
            .ToListAsync();

        foreach (var e in events)
        {
            e.Active = false;
        }

        await LoginEventContext.SaveChangesAsync();

        ResetCache(tenantId, userId);
    }

    private void ResetCache()
    {
        var tenantId = _tenantManager.GetCurrentTenant().Id;
        var userId = _securityContext.CurrentAccount.ID;
        ResetCache(tenantId, userId);
    }

    private void ResetCache(int tenantId, Guid userId)
    {
        var key = GetCacheKey(tenantId, userId);
        _cache.Remove(key);
    }

    private string GetCacheKey(int tenantId, Guid userId)
    {
        return string.Join("", GuidLoginEvent, tenantId, userId);
    }
}
