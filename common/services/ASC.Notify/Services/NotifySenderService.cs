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

namespace ASC.Notify.Services;

[Singletone]
public class NotifySenderService : BackgroundService
{
    private readonly DbWorker _db;
    private readonly ILogger _logger;
    private readonly NotifyServiceCfg _notifyServiceCfg;
    private readonly NotifyConfiguration _notifyConfiguration;
    private readonly IServiceScopeFactory _scopeFactory;

    public NotifySenderService(
        IOptions<NotifyServiceCfg> notifyServiceCfg,
        NotifyConfiguration notifyConfiguration,
        DbWorker dbWorker,
        IServiceScopeFactory scopeFactory,
        ILoggerProvider options)
    {
        _logger = options.CreateLogger("ASC.NotifySender");
        _notifyServiceCfg = notifyServiceCfg.Value;
        _notifyConfiguration = notifyConfiguration;
        _db = dbWorker;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.InformationNotifySenderRunning();

        stoppingToken.Register(() => _logger.Debug("NotifySenderService background task is stopping."));

        if (_notifyServiceCfg.Schedulers != null && _notifyServiceCfg.Schedulers.Any())
        {
            InitializeNotifySchedulers();
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await using var serviceScope = _scopeFactory.CreateAsyncScope();

            var registerInstanceService = serviceScope.ServiceProvider.GetService<IRegisterInstanceManager<NotifySenderService>>();

            if (!await registerInstanceService.IsActive(RegisterInstanceWorkerService<NotifySenderService>.InstanceId))
            {
                _logger.Debug($"Notify Sender Service background task with instance id {RegisterInstanceWorkerService<NotifySenderService>.InstanceId} is't active.");

                await Task.Delay(1000, stoppingToken);

                continue;
            }

            await ThreadManagerWorkAsync(stoppingToken);
        }

        _logger.InformationNotifySenderStopping();
    }

    private void InitializeNotifySchedulers()
    {
        _notifyConfiguration.Configure();

        foreach (var pair in _notifyServiceCfg.Schedulers.Where(r => r.MethodInfo != null))
        {
            _logger.DebugStartScheduler(pair.Name, pair.MethodInfo);
            pair.MethodInfo.Invoke(null, null);
        }
    }

    private async Task ThreadManagerWorkAsync(CancellationToken stoppingToken)
    {
        var tasks = new List<Task>(_notifyServiceCfg.Process.MaxThreads);

        try
        {
            if (tasks.Count < _notifyServiceCfg.Process.MaxThreads)
            {
                var messages = await _db.GetMessagesAsync(_notifyServiceCfg.Process.BufferSize);
                if (messages.Count > 0)
                {
                    var t = new Task(async () => await SendMessagesAsync(messages, stoppingToken), stoppingToken, TaskCreationOptions.LongRunning);
                    tasks.Add(t);
                    t.Start(TaskScheduler.Default);
                }
                else
                {
                    await Task.Delay(5000, stoppingToken);
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
            _logger.ErrorThreadManagerWork(e);
        }
    }

    private async Task SendMessagesAsync(object messages, CancellationToken stoppingToken)
    {
        try
        {
            foreach (var m in (IDictionary<int, NotifyMessage>)messages)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                var result = MailSendingState.Sended;
                try
                {
                    var sender = _notifyServiceCfg.Senders.FirstOrDefault(r => r.Name == m.Value.SenderType);
                    if (sender != null)
                    {
                        await sender.NotifySender.SendAsync(m.Value);
                    }
                    else
                    {
                        result = MailSendingState.FatalError;
                    }

                    _logger.DebugNotify(m.Key);
                }
                catch (Exception e)
                {
                    result = MailSendingState.FatalError;
                    _logger.ErrorWithException(e);
                }

                await _db.SetStateAsync(m.Key, result);
            }
        }
        catch (ThreadAbortException)
        {
            return;
        }
        catch (Exception e)
        {
            _logger.ErrorSendMessages(e);
        }
    }
}