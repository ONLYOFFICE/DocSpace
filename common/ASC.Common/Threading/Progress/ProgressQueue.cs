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
using System.Linq;

using ASC.Common.Logging;
using ASC.Common.Threading.Workers;

using Microsoft.Extensions.Options;

namespace ASC.Common.Threading.Progress
{
    [Singletone(typeof(ConfigureProgressQueue<>))]
    public class ProgressQueueOptionsManager<T> : OptionsManager<ProgressQueue<T>> where T : class, IProgressItem
    {
        public ProgressQueueOptionsManager(IOptionsFactory<ProgressQueue<T>> factory) : base(factory)
        {
        }
    }

    [Singletone]
    public class ConfigureProgressQueue<T> : IPostConfigureOptions<ProgressQueue<T>> where T : class, IProgressItem
    {
        public ConfigureProgressQueue(IOptionsMonitor<ILog> log)
        {
            Log = log;
        }

        private IOptionsMonitor<ILog> Log { get; }

        public void PostConfigure(string name, ProgressQueue<T> queue)
        {
            queue.log = Log.Get("ASC.WorkerQueue");
            queue.Start(x => x.RunJob());
        }
    }

    [Singletone]
    public class ProgressQueue<T> : WorkerQueue<T> where T : class, IProgressItem
    {
        public bool removeAfterCompleted;

        public ProgressQueue()
        {

        }

        public override void Add(T item)
        {
            if (GetStatus(item.Id) == null)
            {
                base.Add(item);
            }
        }

        public T GetStatus(object id)
        {
            T item;
            lock (SynchRoot)
            {
                item = GetItems().Where(x => Equals(x.Id, id)).SingleOrDefault();
                if (item != null)
                {
                    if (removeAfterCompleted && item.IsCompleted)
                    {
                        Remove(item);
                    }
                    return (T)item.Clone();
                }
            }
            return item;
        }

        public void PostComplete(object id)
        {
            lock (SynchRoot)
            {
                var item = GetItems().Where(x => Equals(x.Id, id)).SingleOrDefault();

                if (item != null)
                {
                    item.IsCompleted = true;

                    if (removeAfterCompleted)
                    {
                        Remove(item);
                    }
                }
            }
        }

        protected override WorkItem<T> Selector()
        {
            return Items
                .Where(x => !x.IsProcessed && !x.IsCompleted)
                .OrderBy(x => x.Added)
                .FirstOrDefault();
        }

        protected override void PostComplete(WorkItem<T> item)
        {
            item.IsCompleted = true;
        }

        protected override void ErrorLimit(WorkItem<T> item)
        {
            PostComplete(item);
        }

        protected override void Error(WorkItem<T> workItem, Exception exception)
        {
            workItem.Item.Error = exception;
            workItem.Item.IsCompleted = true;

            base.Error(workItem, exception);
        }
    }
}