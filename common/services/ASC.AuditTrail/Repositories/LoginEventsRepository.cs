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
    private MessagesContext MessagesContext => _lazyMessagesContext.Value;

    private readonly Lazy<MessagesContext> _lazyMessagesContext;
    private readonly TenantManager _tenantManager;
    private readonly IMapper _mapper;

    public LoginEventsRepository(
        TenantManager tenantManager,
        DbContextManager<MessagesContext> dbMessagesContext,
        IMapper mapper)
    {
        _lazyMessagesContext = new Lazy<MessagesContext>(() => dbMessagesContext.Value);
        _tenantManager = tenantManager;
        _mapper = mapper;
    }

    public IEnumerable<LoginEventDto> GetByFilter(
        Guid? login = null,
        MessageAction? action = null,
        DateTime? fromDate = null,
        DateTime? to = null,
        int startIndex = 0,
        int limit = 0)
    {
        var tenant = _tenantManager.GetCurrentTenant().Id;
        var query =
            from q in MessagesContext.LoginEvents
            from p in MessagesContext.Users.Where(p => q.UserId == p.Id).DefaultIfEmpty()
            where q.TenantId == tenant
            where q.Date >= fromDate
            where q.Date <= to
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

        return _mapper.Map<List<LoginEventQuery>, IEnumerable<LoginEventDto>>(query.ToList());
    }

    public int GetCount(int tenant, DateTime? from = null, DateTime? to = null)
    {
        var query = MessagesContext.LoginEvents
            .Where(l => l.TenantId == tenant);

        if (from.HasValue && to.HasValue)
        {
            query = query.Where(l => l.Date >= from & l.Date <= to);
        }

        return query.Count();
    }
}

public static class LoginEventsRepositoryExtensions
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<EventTypeConverter>();
    }
}