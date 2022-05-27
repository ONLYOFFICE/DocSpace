﻿// (c) Copyright Ascensio System SIA 2010-2022
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

namespace ASC.Webhooks.Service.Services;

[Singletone]
public class WorkerService : BackgroundService
{
    private readonly ILogger<WorkerService> _logger;
    private readonly ConcurrentQueue<WebhookRequest> _queue;
    private readonly int? _threadCount = 10;
    private readonly WebhookSender _webhookSender;
    private readonly TimeSpan _waitingPeriod;

    public WorkerService(WebhookSender webhookSender,
        ILogger<WorkerService> logger,
        BuildQueueService buildQueueService,
        Settings settings)
    {
        _logger = logger;
        _webhookSender = webhookSender;
        _queue = buildQueueService.Queue;
        _threadCount = settings.ThreadCount;
        _waitingPeriod = TimeSpan.FromSeconds(5);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var queueSize = _queue.Count;

            if (queueSize == 0) // change to "<= threadCount"
            {
                _logger.TraceProcedure(_waitingPeriod);

                await Task.Delay(_waitingPeriod, stoppingToken);

                continue;
            }

            var tasks = new List<Task>();
            var counter = 0;

            for (var i = 0; i < queueSize; i++)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                if (!_queue.TryDequeue(out var entry))
                {
                    break;
                }

                tasks.Add(_webhookSender.Send(entry, stoppingToken));
                counter++;

                if (counter >= _threadCount)
                {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                    counter = 0;
                }
            }

            Task.WaitAll(tasks.ToArray());
            _logger.DebugProcedureFinish();
        }
    }
}