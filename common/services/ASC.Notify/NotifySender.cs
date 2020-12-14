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
using System.Threading;
using System.Threading.Tasks;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Notify.Config;
using ASC.Notify.Messages;

using Microsoft.Extensions.Options;

namespace ASC.Notify
{
    [Singletone]
    public class NotifySender : IDisposable
    {
        private readonly ILog log;

        private readonly DbWorker db;
        private CancellationTokenSource cancellationToken;

        public NotifyServiceCfg NotifyServiceCfg { get; }

        public NotifySender(IOptions<NotifyServiceCfg> notifyServiceCfg, DbWorker dbWorker, IOptionsMonitor<ILog> options)
        {
            log = options.CurrentValue;
            NotifyServiceCfg = notifyServiceCfg.Value;
            db = dbWorker;
        }

        public void StartSending()
        {
            db.ResetStates();
            cancellationToken = new CancellationTokenSource();
            var task = new Task(async () => await ThreadManagerWork(), cancellationToken.Token, TaskCreationOptions.LongRunning);
            task.Start();
        }

        public void StopSending()
        {
            cancellationToken.Cancel();
        }

        private async Task ThreadManagerWork()
        {
            var tasks = new List<Task>(NotifyServiceCfg.Process.MaxThreads);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (tasks.Count < NotifyServiceCfg.Process.MaxThreads)
                    {
                        var messages = db.GetMessages(NotifyServiceCfg.Process.BufferSize);
                        if (messages.Count > 0)
                        {
                            var t = new Task(() => SendMessages(messages), cancellationToken.Token, TaskCreationOptions.LongRunning);
                            tasks.Add(t);
                            t.Start(TaskScheduler.Default);
                        }
                        else
                        {
                            await Task.Delay(5000);
                        }
                    }
                    else
                    {
                        await Task.WhenAny(tasks.ToArray()).ContinueWith(r => tasks.RemoveAll(a => a.IsCompleted));
                    }
                }
                catch (ThreadAbortException)
                {
                    return;
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }
        }

        private void SendMessages(object messages)
        {
            try
            {
                foreach (var m in (IDictionary<int, NotifyMessage>)messages)
                {
                    if (cancellationToken.IsCancellationRequested) return;

                    var result = MailSendingState.Sended;
                    try
                    {
                        var sender = NotifyServiceCfg.Senders.FirstOrDefault(r => r.Name == m.Value.Sender);
                        if (sender != null)
                        {
                            sender.NotifySender.Send(m.Value);
                        }
                        else
                        {
                            result = MailSendingState.FatalError;
                        }

                        log.DebugFormat("Notify #{0} has been sent.", m.Key);
                    }
                    catch (Exception e)
                    {
                        result = MailSendingState.FatalError;
                        log.Error(e);
                    }
                    db.SetState(m.Key, result);
                }
            }
            catch (ThreadAbortException)
            {
                return;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        public void Dispose()
        {
            if (cancellationToken != null)
            {
                cancellationToken.Dispose();
            }
        }
    }
}
