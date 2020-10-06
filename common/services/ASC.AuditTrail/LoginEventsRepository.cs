/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/


using System;
using System.Collections.Generic;
using ASC.AuditTrail.Mappers;
using System.Linq;
using ASC.Core.Tenants;
using ASC.Core.Users;
using Newtonsoft.Json;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;
using ASC.Core.Common.EF;
using Microsoft.EntityFrameworkCore;
using ASC.Common;

namespace ASC.AuditTrail.Data
{
    public class LoginEventsRepository
    {
        private UserFormatter UserFormatter { get; }
        private AuditTrailContext AuditTrailContext { get; }

        public LoginEventsRepository(UserFormatter userFormatter, DbContextManager<AuditTrailContext> dbContextManager)
        {
            UserFormatter = userFormatter;
            AuditTrailContext = dbContextManager.Value;
        }

        private class Query
        {
            public LoginEvents LoginEvents { get; set; }
            public User User { get; set; }
        }

        public IEnumerable<LoginEvent> GetLast(int tenant, int chunk)
        {
            /*
               var query = BackupContext.Schedules.Join(BackupContext.Tenants,
                s => s.TenantId,
                t => t.Id,
                (s, t) => new { schedule = s, tenant = t })
                .Where(q => q.tenant.Status == TenantStatus.Active)
                .Select(q => q.schedule);
*/
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
            /* var q = new SqlQuery("login_events au")
                 .Select(auditColumns.Select(x => "au." + x).ToArray())
                 .LeftOuterJoin("core_user u", Exp.EqColumns("au.user_id", "u.id"))
                 .Select("u.firstname", "u.lastname")
                 .Where("au.tenant_id", tenant)
                 .Where(Exp.Between("au.date", from, to))
                 .OrderBy("au.date", false);

             using (var db = new DbManager(auditDbId))
             {
                 return db.ExecuteList(q).Select(ToLoginEvent).Where(x => x != null);
             }*/
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
            /*
            var q = new SqlQuery("login_events")
                .SelectCount()
                .Where("tenant_id", tenant);

            if (from.HasValue && to.HasValue)
            {
                q.Where(Exp.Between("date", from.Value, to.Value));
            }

            using (var db = new DbManager(auditDbId))
            {
                return db.ExecuteScalar<int>(q);
            }*/
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
                    //log.Error("Error while forming event from db: " + ex);
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