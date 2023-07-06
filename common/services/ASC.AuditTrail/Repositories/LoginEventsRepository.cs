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

namespace ASC.AuditTrail.Repositories;

[Scope(Additional = typeof(LoginEventsRepositoryExtensions))]
public class LoginEventsRepository
{
    private readonly TenantManager _tenantManager;
    private readonly IDbContextFactory<MessagesContext> _dbContextFactory;
    private readonly IMapper _mapper;

    public LoginEventsRepository(
        TenantManager tenantManager,
        IDbContextFactory<MessagesContext> dbContextFactory,
        IMapper mapper)
    {
        _tenantManager = tenantManager;
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LoginEventDto>> GetByFilterAsync(
        Guid? login = null,
        MessageAction? action = null,
        DateTime? fromDate = null,
        DateTime? to = null,
        int startIndex = 0,
        int limit = 0)
    {
        var tenant = await _tenantManager.GetCurrentTenantIdAsync();
        await using var messagesContext = _dbContextFactory.CreateDbContext();

        var query =
            from q in messagesContext.LoginEvents
            from p in messagesContext.Users.Where(p => q.UserId == p.Id).DefaultIfEmpty()
            where q.TenantId == tenant
            orderby q.Date descending
            select new LoginEventQuery
            {
                Event = q,
                UserName = p.UserName,
                FirstName = p.FirstName,
                LastName = p.LastName
            };

        if (startIndex > 0)
        {
            query = query.Skip(startIndex);
        }
        if (limit > 0)
        {
            query = query.Take(limit);
        }

        if (login.HasValue && login.Value != Guid.Empty)
        {
            query = query.Where(r => r.Event.UserId == login.Value);
        }

        if (action.HasValue && action.Value != MessageAction.None)
        {
            query = query.Where(r => r.Event.Action == (int)action);
        }

        var hasFromFilter = (fromDate.HasValue && fromDate.Value != DateTime.MinValue);
        var hasToFilter = (to.HasValue && to.Value != DateTime.MinValue);

        if (hasFromFilter || hasToFilter)
        {
            if (hasFromFilter)
            {
                if (hasToFilter)
                {
                    query = query.Where(q => q.Event.Date >= fromDate.Value & q.Event.Date <= to.Value);
                }
                else
                {
                    query = query.Where(q => q.Event.Date >= fromDate.Value);
                }
            }
            else if (hasToFilter)
            {
                query = query.Where(q => q.Event.Date <= to.Value);
            }
        }

        return _mapper.Map<List<LoginEventQuery>, IEnumerable<LoginEventDto>>(await query.ToListAsync());
    }
}

public static class LoginEventsRepositoryExtensions
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<EventTypeConverter>();
    }
}