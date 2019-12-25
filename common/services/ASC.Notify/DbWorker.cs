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
using System.Data;
using System.Linq;

using ASC.Core.Common.EF;
using ASC.Core.Common.EF.Context;
using ASC.Core.Common.EF.Model;
using ASC.Notify.Config;
using ASC.Notify.Messages;

using Google.Protobuf.Collections;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace ASC.Notify
{
    public class DbWorker
    {
        private readonly string dbid;
        private readonly object syncRoot = new object();

        public IServiceProvider ServiceProvider { get; }
        public NotifyServiceCfg NotifyServiceCfg { get; }

        public DbWorker(IServiceProvider serviceProvider, IOptions<NotifyServiceCfg> notifyServiceCfg)
        {
            ServiceProvider = serviceProvider;
            NotifyServiceCfg = notifyServiceCfg.Value;
            dbid = NotifyServiceCfg.ConnectionStringName;
        }

        public int SaveMessage(NotifyMessage m)
        {
            using var scope = ServiceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetService<DbContextManager<NotifyDbContext>>().Get(dbid);
            using var tx = dbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted);

            var notifyQueue = new NotifyQueue
            {
                NotifyId = 0,
                TenantId = m.Tenant,
                Sender = m.From,
                Reciever = m.To,
                Subject = m.Subject,
                ContentType = m.ContentType,
                Content = m.Content,
                SenderType = m.Sender,
                CreationDate = new DateTime(m.CreationDate),
                ReplyTo = m.ReplyTo,
                Attachments = m.EmbeddedAttachments.ToString()
            };

            notifyQueue = dbContext.NotifyQueue.Add(notifyQueue).Entity;
            dbContext.SaveChanges();

            var id = notifyQueue.NotifyId;

            var info = new NotifyInfo
            {
                NotifyId = id,
                State = 0,
                Attempts = 0,
                ModifyDate = DateTime.UtcNow,
                Priority = m.Priority
            };

            dbContext.NotifyInfo.Add(info);
            dbContext.SaveChanges();

            tx.Commit();

            return 1;
        }

        public IDictionary<int, NotifyMessage> GetMessages(int count)
        {
            lock (syncRoot)
            {
                using var scope = ServiceProvider.CreateScope();
                using var dbContext = scope.ServiceProvider.GetService<DbContextManager<NotifyDbContext>>().Get(dbid);
                using var tx = dbContext.Database.BeginTransaction();

                var q = dbContext.NotifyQueue
                    .Join(dbContext.NotifyInfo, r => r.NotifyId, r => r.NotifyId, (queue, info) => new { queue, info })
                    .Where(r => r.info.State == (int)MailSendingState.NotSended || r.info.State == (int)MailSendingState.Error && r.info.ModifyDate < DateTime.UtcNow - TimeSpan.Parse(NotifyServiceCfg.Process.AttemptsInterval))
                    .OrderBy(i => i.info.Priority)
                    .ThenBy(i => i.info.NotifyId)
                    .Take(count);


                var messages = q
                    .ToDictionary(
                        r => r.queue.NotifyId,
                        r =>
                        {
                            var res = new NotifyMessage
                            {
                                Tenant = r.queue.TenantId,
                                From = r.queue.Sender,
                                To = r.queue.Reciever,
                                Subject = r.queue.Subject,
                                ContentType = r.queue.ContentType,
                                Content = r.queue.Content,
                                Sender = r.queue.SenderType,
                                CreationDate = r.queue.CreationDate.Ticks,
                                ReplyTo = r.queue.ReplyTo
                            };
                            try
                            {
                                res.EmbeddedAttachments.AddRange(JsonConvert.DeserializeObject<RepeatedField<NotifyMessageAttachment>>(r.queue.Attachments));
                            }
                            catch (Exception)
                            {

                            }
                            return res;
                        });

                var info = dbContext.NotifyInfo.Where(r => messages.Keys.Any(a => a == r.NotifyId)).ToList();

                foreach (var i in info)
                {
                    i.State = (int)MailSendingState.Sending;
                }

                dbContext.SaveChanges();

                return messages;
            }
        }


        public void ResetStates()
        {
            using var scope = ServiceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetService<DbContextManager<NotifyDbContext>>().Get(dbid);

            var tr = dbContext.Database.BeginTransaction();
            var info = dbContext.NotifyInfo.Where(r => r.State == 1).ToList();

            foreach (var i in info)
            {
                i.State = 0;
            }

            dbContext.SaveChanges();
            tr.Commit();
        }

        public void SetState(int id, MailSendingState result)
        {
            using var scope = ServiceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetService<DbContextManager<NotifyDbContext>>().Get(dbid);
            using var tx = dbContext.Database.BeginTransaction();

            if (result == MailSendingState.Sended)
            {
                var d = dbContext.NotifyInfo.Where(r => r.NotifyId == id).FirstOrDefault();
                dbContext.NotifyInfo.Remove(d);
                dbContext.SaveChanges();
            }
            else
            {
                if (result == MailSendingState.Error)
                {
                    var attempts = dbContext.NotifyInfo.Where(r => r.NotifyId == id).Select(r => r.Attempts).FirstOrDefault();
                    if (NotifyServiceCfg.Process.MaxAttempts <= attempts + 1)
                    {
                        result = MailSendingState.FatalError;
                    }
                }

                var info = dbContext.NotifyInfo
                    .Where(r => r.NotifyId == id)
                    .ToList();

                foreach (var i in info)
                {
                    i.State = (int)result;
                    i.Attempts += 1;
                    i.ModifyDate = DateTime.UtcNow;
                }

                dbContext.SaveChanges();
            }

            tx.Commit();
        }
    }

    public static class DbWorkerExtension
    {
        public static IServiceCollection AddDbWorker(this IServiceCollection services)
        {
            services.TryAddSingleton<DbWorker>();

            return services
                .AddNotifyDbContext();
        }
    }
}
