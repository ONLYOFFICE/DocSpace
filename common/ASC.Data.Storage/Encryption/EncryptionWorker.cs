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

namespace ASC.Data.Storage.Encryption;

[Singletone]
public class EncryptionWorker
{
    private readonly object _locker;
    private readonly FactoryOperation _factoryOperation;
    private readonly DistributedTaskQueue _queue;
    public const string CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME = "encryption";

    public EncryptionWorker(FactoryOperation factoryOperation,
                            IDistributedTaskQueueFactory queueFactory)
    {
        _locker = new object();
        _factoryOperation = factoryOperation;
        _queue = queueFactory.CreateQueue(CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME);
    }

    public void Start(EncryptionSettingsProto encryptionSettings)
    {
        EncryptionOperation encryptionOperation;
        lock (_locker)
        {
            if (_queue.GetAllTasks().Any(x => x.Id == GetCacheId()))
            {
                return;
            }

            encryptionOperation = _factoryOperation.CreateOperation(encryptionSettings, GetCacheId());
            _queue.EnqueueTask(encryptionOperation);
        }
    }

    public void Stop()
    {
        _queue.DequeueTask(GetCacheId());
    }

    public string GetCacheId()
    {
        return typeof(EncryptionOperation).FullName;
    }

    public double? GetEncryptionProgress()
    {
        var progress = _queue.GetAllTasks<EncryptionOperation>().FirstOrDefault();

        return progress.Percentage;
    }
}

[Singletone(Additional = typeof(FactoryOperationExtension))]
public class FactoryOperation
{
    private readonly IServiceProvider _serviceProvider;

    public FactoryOperation(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public EncryptionOperation CreateOperation(EncryptionSettingsProto encryptionSettings, string id)
    {
        var item = _serviceProvider.GetService<EncryptionOperation>();
        item.Init(encryptionSettings, id);

        return item;
    }
}

public static class FactoryOperationExtension
{
    public static void Register(DIHelper dIHelper)
    {
        dIHelper.TryAdd<EncryptionOperation>();
    }
}
