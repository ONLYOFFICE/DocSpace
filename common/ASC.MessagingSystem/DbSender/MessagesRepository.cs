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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

using ASC.Common.Logging;
using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;
using ASC.Core.Tenants;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using UAParser;

using IsolationLevel = System.Data.IsolationLevel;

namespace ASC.MessagingSystem.DbSender
{
    public class MessagesRepository
    {
        private static DateTime lastSave = DateTime.UtcNow;
        private readonly TimeSpan CacheTime;
        private readonly IDictionary<string, EventMessage> Cache;
        private static Parser Parser { get; set; }

        private readonly Timer Timer;
        private bool timerStarted;

        private readonly Timer ClearTimer;

        public ILog Log { get; set; }
        public IServiceProvider ServiceProvider { get; }

        public MessagesRepository(IServiceProvider serviceProvider, IOptionsMonitor<ILog> options)
        {
            CacheTime = TimeSpan.FromMinutes(1);
            Cache = new Dictionary<string, EventMessage>();
            Parser = Parser.GetDefault();
            Timer = new Timer(FlushCache);
            timerStarted = false;

            ClearTimer = new Timer(DeleteOldEvents);
            ClearTimer.Change(new TimeSpan(0), TimeSpan.FromDays(1));
            Log = options.CurrentValue;
            ServiceProvider = serviceProvider;
        }

        public void Add(EventMessage message)
        {
            // messages with action code < 2000 are related to login-history
            if ((int)message.Action < 2000)
            {
                using var scope = ServiceProvider.CreateScope();
                using var ef = scope.ServiceProvider.GetService<DbContextManager<MessagesContext>>().Get("messages");
                AddLoginEvent(message, ef);
                return;
            }

            var now = DateTime.UtcNow;
            var key = string.Format("{0}|{1}|{2}|{3}", message.TenantId, message.UserId, message.Id, now.Ticks);

            lock (Cache)
            {
                Cache[key] = message;

                if (!timerStarted)
                {
                    Timer.Change(0, 100);
                    timerStarted = true;
                }
            }

        }

        private void FlushCache(object state)
        {
            List<EventMessage> events = null;

            if (CacheTime < DateTime.UtcNow - lastSave || Cache.Count > 100)
            {
                lock (Cache)
                {
                    Timer.Change(-1, -1);
                    timerStarted = false;

                    events = new List<EventMessage>(Cache.Values);
                    Cache.Clear();
                    lastSave = DateTime.UtcNow;
                }
            }

            if (events == null) return;

            using var scope = ServiceProvider.CreateScope();
            using var ef = scope.ServiceProvider.GetService<DbContextManager<MessagesContext>>().Get("messages");
            using var tx = ef.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
            var dict = new Dictionary<string, ClientInfo>();

            foreach (var message in events)
            {
                if (!string.IsNullOrEmpty(message.UAHeader))
                {
                    try
                    {

                        ClientInfo clientInfo;

                        if (dict.ContainsKey(message.UAHeader))
                        {
                            clientInfo = dict[message.UAHeader];
                        }
                        else
                        {
                            clientInfo = Parser.Parse(message.UAHeader);
                            dict.Add(message.UAHeader, clientInfo);
                        }

                        if (clientInfo != null)
                        {
                            message.Browser = GetBrowser(clientInfo);
                            message.Platform = GetPlatform(clientInfo);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("FlushCache " + message.Id, e);
                    }
                }

                // messages with action code < 2000 are related to login-history
                if ((int)message.Action >= 2000)
                {
                    AddAuditEvent(message, ef);
                }
            }

            tx.Commit();
        }

        private static void AddLoginEvent(EventMessage message, MessagesContext dbContext)
        {
            var le = new LoginEvents
            {
                Ip = message.IP,
                Login = message.Initiator,
                Browser = message.Browser,
                Platform = message.Platform,
                Date = message.Date,
                TenantId = message.TenantId,
                UserId = message.UserId,
                Page = message.Page,
                Action = (int)message.Action
            };

            if (message.Description != null && message.Description.Any())
            {
                le.Description =
                    JsonConvert.SerializeObject(message.Description, new JsonSerializerSettings
                    {
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc
                    });
            }

            dbContext.LoginEvents.Add(le);
            dbContext.SaveChanges();
        }

        private static void AddAuditEvent(EventMessage message, MessagesContext dbContext)
        {
            var ae = new AuditEvent
            {
                Ip = message.IP,
                Initiator = message.Initiator,
                Browser = message.Browser,
                Platform = message.Platform,
                Date = message.Date,
                TenantId = message.TenantId,
                UserId = message.UserId,
                Page = message.Page,
                Action = (int)message.Action,
                Target = message.Target?.ToString()
            };

            if (message.Description != null && message.Description.Any())
            {
                ae.Description =
                    JsonConvert.SerializeObject(GetSafeDescription(message.Description), new JsonSerializerSettings
                    {
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc
                    });
            }

            dbContext.AuditEvents.Add(ae);
            dbContext.SaveChanges();
        }

        private static IList<string> GetSafeDescription(IEnumerable<string> description)
        {
            const int maxLength = 15000;

            var currentLength = 0;
            var safe = new List<string>();

            foreach (var d in description.Where(r => r != null))
            {
                if (currentLength + d.Length <= maxLength)
                {
                    currentLength += d.Length;
                    safe.Add(d);
                }
                else
                {
                    safe.Add(d.Substring(0, maxLength - currentLength - 3) + "...");
                    break;
                }
            }

            return safe;
        }

        private static string GetBrowser(ClientInfo clientInfo)
        {
            return clientInfo == null
                       ? null
                       : string.Format("{0} {1}", clientInfo.UA.Family, clientInfo.UA.Major);
        }

        private static string GetPlatform(ClientInfo clientInfo)
        {
            return clientInfo == null
                       ? null
                       : string.Format("{0} {1}", clientInfo.OS.Family, clientInfo.OS.Major);
        }

        //TODO: move to external service and fix
        private void DeleteOldEvents(object state)
        {
            try
            {
                GetOldEvents(r => r.LoginEvents, "LoginHistoryLifeTime");
                GetOldEvents(r => r.AuditEvents, "AuditTrailLifeTime");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
            }
        }

        private void GetOldEvents<T>(Expression<Func<MessagesContext, DbSet<T>>> func, string settings) where T : MessageEvent
        {
            List<T> ids;
            var compile = func.Compile();
            do
            {
                using var scope = ServiceProvider.CreateScope();
                using var ef = scope.ServiceProvider.GetService<DbContextManager<MessagesContext>>().Get("messages");
                var table = compile.Invoke(ef);

                var ae = table
                    .Join(ef.Tenants, r => r.TenantId, r => r.Id, (audit, tenant) => audit)
                    .Select(r => new
                    {
                        r.Id,
                        r.Date,
                        r.TenantId,
                        ef = r
                    })
                    .Where(r => r.Date < DateTime.UtcNow.AddDays(
                        ef.WebstudioSettings
                        .Where(a => a.TenantId == r.TenantId && a.Id == TenantAuditSettings.Guid)
                        .Select(r => Convert.ToDouble(JsonExtensions.JsonValue(nameof(r.Data).ToLower(), settings) ?? TenantAuditSettings.MaxLifeTime.ToString()))
                        .FirstOrDefault()))
                    .Take(1000);

                ids = ae.Select(r => r.ef).ToList();

                if (!ids.Any()) return;

                table.RemoveRange(ids);
                ef.SaveChanges();

            } while (ids.Any());
        }
    }
}