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

namespace ASC.Web.Api.Core;

[Singletone(Additional = typeof(SmtpOperationExtension))]
public class SmtpOperation
{
    public const string CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME = "smtp";
    private readonly DistributedTaskQueue _progressQueue;
    private readonly IServiceProvider _serviceProvider;

    public SmtpOperation(IServiceProvider serviceProvider, IDistributedTaskQueueFactory queueFactory)
    {
        _serviceProvider = serviceProvider;
        _progressQueue = queueFactory.CreateQueue(CUSTOM_DISTRIBUTED_TASK_QUEUE_NAME);
    }

    public void StartSmtpJob(SmtpSettingsDto smtpSettings, Tenant tenant, Guid user)
    {
        var item = _progressQueue.GetAllTasks<SmtpJob>().FirstOrDefault(t => t.TenantId == tenant.Id);

        if (item != null && item.IsCompleted)
        {
            _progressQueue.DequeueTask(item.Id);
            item = null;
        }

        if (item == null)
        {
            item = _serviceProvider.GetRequiredService<SmtpJob>();
            item.Init(smtpSettings, tenant.Id, user);
            _progressQueue.EnqueueTask(item);
        }

        item.PublishChanges();
    }

    public SmtpOperationStatusRequestsDto GetStatus(Tenant tenant)
    {
        var item = _progressQueue.GetAllTasks<SmtpJob>().FirstOrDefault(t => t.TenantId == tenant.Id);

        if (item == null)
        {
            return null;
        }

        if (item.IsCompleted == true)
        {
            _progressQueue.DequeueTask(item.Id);
        }

        var result = new SmtpOperationStatusRequestsDto
        {
            Id = item.Id,
            Completed = item.IsCompleted,
            Percents = (int)item.Percentage,
            Error = item.Exception != null ? item.Exception.Message : "",
            Status = item.CurrentOperation
        };

        return result;
    }
}

public static class SmtpOperationExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<SmtpJob>();
    }
}
