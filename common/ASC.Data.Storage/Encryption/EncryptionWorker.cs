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
using System.Linq;

using ASC.Common;
using ASC.Common.Threading;
using ASC.Core.Encryption;

using Microsoft.Extensions.DependencyInjection;

namespace ASC.Data.Storage.Encryption
{
    [Singletone]
    public class EncryptionWorker
    {
        private object Locker { get; }
        private FactoryOperation FactoryOperation { get; }
        private DistributedTaskQueue Queue { get; }

        public EncryptionWorker(FactoryOperation factoryOperation, DistributedTaskQueueOptionsManager options)
        {
            Locker = new object();
            FactoryOperation = factoryOperation;
            Queue = options.Get<EncryptionOperation>();
        }

        public void Start(EncryptionSettingsProto encryptionSettings)
        {
            EncryptionOperation encryptionOperation;
            lock (Locker)
            {
                if (Queue.GetTask<EncryptionOperation>(GetCacheId()) != null) return;
                encryptionOperation = FactoryOperation.CreateOperation(encryptionSettings, GetCacheId());
                Queue.QueueTask(encryptionOperation);
            }
        }

        public void Stop()
        {
            Queue.CancelTask(GetCacheId());
        }

        public string GetCacheId()
        {
            return typeof(EncryptionOperation).FullName;
        }

        public double? GetEncryptionProgress()
        {
            var progress = Queue.GetTasks<EncryptionOperation>().FirstOrDefault();
            return progress.Percentage;
    }
    }

    [Singletone(Additional = typeof(FactoryOperationExtension))]
    public class FactoryOperation
    {
        private IServiceProvider ServiceProvider { get; set; }

        public FactoryOperation(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public EncryptionOperation CreateOperation(EncryptionSettingsProto encryptionSettings, string id)
        {
            var item = ServiceProvider.GetService<EncryptionOperation>();
            item.Init(encryptionSettings, id);
            return item;
        }
    }

    public static class FactoryOperationExtension
    {
        public static void Register(DIHelper dIHelper)
        {
            dIHelper.TryAdd<EncryptionOperation>();
            dIHelper.AddDistributedTaskQueueService<EncryptionOperation>(1);
        }
    }
}
