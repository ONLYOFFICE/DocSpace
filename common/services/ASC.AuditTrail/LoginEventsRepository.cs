/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using ASC.AuditTrail.Mappers;
using System.Linq;
using ASC.Core.Users;
using Newtonsoft.Json;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;
using ASC.Core.Common.EF;
using ASC.Common;
using ASC.Common.Logging;
using Microsoft.Extensions.Options;

namespace ASC.AuditTrail.Data
{
    public class LoginEventsRepository
    {
        private UserFormatter UserFormatter { get; }
        private AuditTrailContext AuditTrailContext { get; }
        private ILog Log { get; }

        public LoginEventsRepository(UserFormatter userFormatter, DbContextManager<AuditTrailContext> dbContextManager, IOptionsMonitor<ILog> options)
        {
            UserFormatter = userFormatter;
            AuditTrailContext = dbContextManager.Value;
            Log = options.CurrentValue;
        }

        private class Query
        {
            public LoginEvents LoginEvents { get; set; }
            public User User { get; set; }
        }

        public IEnumerable<LoginEvent> GetLast(int tenant, int chunk)
        {
            var query = AuditTrailContext.LoginEvents.Join(AuditTrailContext.User,
                l => l.UserId,
                u => u.Id,
                (l, u) => new Query { LoginEvents = l, User = u })
                .Where(q => q.LoginEvents.TenantId == tenant)
                .OrderBy(q => q.LoginEvents.Date)
                .Take(chunk);
           
            return ToLoginEvent(query.ToList());
        }

        public IEnumerable<LoginEvent> Get(int tenant, DateTime from, DateTime to)
        {
            var query = AuditTrailContext.LoginEvents.Join(AuditTrailContext.User,
                l => l.UserId,
                u => u.Id,
                 (l, u) => new Query { LoginEvents = l, User = u })
               .Where(q => q.LoginEvents.TenantId == tenant)
               .Where(q => q.LoginEvents.Date >= from & q.LoginEvents.Date <= to)
               .OrderBy(q => q.LoginEvents.Date);

            return ToLoginEvent(query.ToList());
        }

        public int GetCount(int tenant, DateTime? from = null, DateTime? to = null)
        {
            var query = AuditTrailContext.LoginEvents
                .Where(l => l.TenantId == tenant);

            if (from.HasValue && to.HasValue)
            {
               query =  query.Where(l => l.Date >= from & l.Date <= to);
            }

            return query.Count();
        }

        private IEnumerable<LoginEvent> ToLoginEvent(List<Query> list)
        {
            var loginsEvents = new List<LoginEvent>();
            list.ForEach(query =>
            {
                try
                {
                    var evt = new LoginEvent
                        {
                            Id = query.LoginEvents.Id,
                            IP = query.LoginEvents.Ip,
                            Login = query.LoginEvents.Login,
                            Browser = query.LoginEvents.Browser,
                            Platform = query.LoginEvents.Platform,
                            Date = query.LoginEvents.Date,
                            TenantId = query.LoginEvents.TenantId,
                            UserId = query.LoginEvents.UserId,
                            Page = query.LoginEvents.Page,
                            Action = query.LoginEvents.Action
                        };

                    if (query.LoginEvents.Description != null)
                    {
                        evt.Description = JsonConvert.DeserializeObject<IList<string>>(
                            query.LoginEvents.Description,
                            new JsonSerializerSettings
                                {
                                    DateTimeZoneHandling = DateTimeZoneHandling.Utc
                                });
                    }
                    evt.UserName = (query.User.UserName != null && query.User.LastName != null)
                                        ? UserFormatter.GetUserName(query.User.UserName, query.User.LastName)
                                        : !string.IsNullOrWhiteSpace(evt.Login)
                                                ? evt.Login
                                                : evt.UserId == Core.Configuration.Constants.Guest.ID
                                                    ? AuditReportResource.GuestAccount
                                                    : AuditReportResource.UnknownAccount;

                    evt.ActionText = AuditActionMapper.GetActionText(evt);

                    loginsEvents.Add(evt);
                }
                catch(Exception)
                {
                    Log.Error("Error while forming event from db: " + ex);
                }
            });
            return loginsEvents;
        }
    }

    public static class LoginEventsRepositoryExtension
    {
        public static DIHelper AddLoginEventsRepositoryService(this DIHelper services)
        {
            _ = services.TryAddScoped<LoginEventsRepository>();
            return services
                .AddUserFormatter()
                .AddAuditTrailContextService();
        }
    }
}