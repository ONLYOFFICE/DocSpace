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

[Scope]
public class LoginEventsRepository
{
    private MessagesContext MessagesContext => _lazyMessagesContext.Value;

    private readonly Lazy<MessagesContext> _lazyMessagesContext;
    private readonly IMapper _mapper;

    public LoginEventsRepository(
        DbContextManager<MessagesContext> dbMessagesContext,
        IMapper mapper)
    {
        _lazyMessagesContext = new Lazy<MessagesContext>(() => dbMessagesContext.Value);
        _mapper = mapper;
    }

    public IEnumerable<LoginEventDto> GetLast(int tenant, int chunk)
    {
        var query =
            (from b in MessagesContext.LoginEvents
             from p in MessagesContext.Users.Where(p => b.UserId == p.Id).DefaultIfEmpty()
             where b.TenantId == tenant
             orderby b.Date descending
             select new LoginEventQuery
             {
                 Event = b,
                 UserName = p.UserName,
                 FirstName = p.FirstName,
                 LastName = p.LastName
             })
             .Take(chunk);

        return _mapper.Map<List<LoginEventQuery>, IEnumerable<LoginEventDto>>(query.ToList());
    }

    public IEnumerable<LoginEventDto> Get(int tenant, DateTime fromDate, DateTime to)
    {
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