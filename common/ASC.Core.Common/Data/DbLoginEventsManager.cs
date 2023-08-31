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

    private readonly TenantManager _tenantManager;
    private readonly AuthContext _authContext;
    private readonly IDbContextFactory<MessagesContext> _dbContextFactory;
    private readonly IMapper _mapper;

    public DbLoginEventsManager(
        TenantManager tenantManager,
        AuthContext authContext,
        IDbContextFactory<MessagesContext> dbContextFactory,
        IMapper mapper)
    {
        _tenantManager = tenantManager;
        _authContext = authContext;
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<DbLoginEvent> GetByIdAsync(int id)
    {
        if (id < 0) return null;

        await using var loginEventContext = _dbContextFactory.CreateDbContext();
        
        return await loginEventContext.LoginEvents.FindAsync(id);
    }

    public async Task<List<BaseEvent>> GetLoginEventsAsync(int tenantId, Guid userId)
    {
        var date = DateTime.UtcNow.AddYears(-1);

        await using var loginEventContext = _dbContextFactory.CreateDbContext();

        var loginInfo = await Queries.LoginEventsAsync(loginEventContext, tenantId, userId, _loginActions, date).ToListAsync();

        return _mapper.Map<List<DbLoginEvent>, List<BaseEvent>>(loginInfo);
    }

    public async Task LogOutEventAsync(int loginEventId)
    {
        await using var loginEventContext = _dbContextFactory.CreateDbContext();

        await Queries.DeleteLoginEventsAsync(loginEventContext, loginEventId);

        await ResetCacheAsync();
    }

    public async Task LogOutAllActiveConnectionsAsync(int tenantId, Guid userId)
    {
        await using var loginEventContext = _dbContextFactory.CreateDbContext();

        await Queries.DeleteLoginEventsByUserIdAsync(loginEventContext, tenantId, userId);

        ResetCache(tenantId, userId);
    }

    public async Task LogOutAllActiveConnectionsForTenantAsync(int tenantId)
    {
        await using var loginEventContext = _dbContextFactory.CreateDbContext();

        await Queries.UpdateLoginEventsByTenantIdAsync(loginEventContext, tenantId);
    }

    public async Task LogOutAllActiveConnectionsExceptThisAsync(int loginEventId, int tenantId, Guid userId)
    {
        await using var loginEventContext = _dbContextFactory.CreateDbContext();

        await Queries.UpdateLoginEventsAsync(loginEventContext, tenantId, userId, loginEventId);

        ResetCache(tenantId, userId);
    }

    public async Task ResetCacheAsync()
    {
        var tenantId = await _tenantManager.GetCurrentTenantIdAsync();
        var userId = _authContext.CurrentAccount.ID;
        ResetCache(tenantId, userId);
    }

    public void ResetCache(int tenantId, Guid userId)
    {
        var key = GetCacheKey(tenantId, userId);
    }

    private string GetCacheKey(int tenantId, Guid userId)
    {
        return string.Join("", GuidLoginEvent, tenantId, userId);
    }
}

static file class Queries
{
    public static readonly Func<MessagesContext, int, Guid, IEnumerable<int>, DateTime, IAsyncEnumerable<DbLoginEvent>>
        LoginEventsAsync = EF.CompileAsyncQuery(
            (MessagesContext ctx, int tenantId, Guid userId, IEnumerable<int> loginActions, DateTime date) =>
                ctx.LoginEvents
                    .Where(r => r.TenantId == tenantId
                                && r.UserId == userId
                                && loginActions.Contains(r.Action ?? 0)
                                && r.Date >= date
                                && r.Active)
                    .OrderByDescending(r => r.Id)
                    .AsQueryable());

    public static readonly Func<MessagesContext, int, Task<int>> DeleteLoginEventsAsync =
        EF.CompileAsyncQuery(
            (MessagesContext ctx, int loginEventId) =>
                ctx.LoginEvents
                    .Where(r => r.Id == loginEventId)
                    .ExecuteUpdate(r => r.SetProperty(p => p.Active, false)));

    public static readonly Func<MessagesContext, int, Guid, Task<int>> DeleteLoginEventsByUserIdAsync =
        EF.CompileAsyncQuery(
            (MessagesContext ctx, int tenantId, Guid userId) =>
                ctx.LoginEvents
                    .Where(r => r.TenantId == tenantId
                                && r.UserId == userId
                                && r.Active)
                    .ExecuteUpdate(r => r.SetProperty(p => p.Active, false)));

    public static readonly Func<MessagesContext, int, Task<int>> UpdateLoginEventsByTenantIdAsync =
        EF.CompileAsyncQuery(
            (MessagesContext ctx, int tenantId) =>
                ctx.LoginEvents
                    .Where(r => r.TenantId == tenantId
                                && r.Active)
                    .ExecuteUpdate(r => r.SetProperty(p => p.Active, false)));

    public static readonly Func<MessagesContext, int, Guid, int, Task<int>> UpdateLoginEventsAsync =
        EF.CompileAsyncQuery(
            (MessagesContext ctx, int tenantId, Guid userId, int loginEventId) =>
                ctx.LoginEvents
                    .Where(r => r.TenantId == tenantId
                                && r.UserId == userId
                                && r.Id != loginEventId
                                && r.Active)
                    .ExecuteUpdate(r => r.SetProperty(p => p.Active, false)));
}