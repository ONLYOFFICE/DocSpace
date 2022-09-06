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

using IsolationLevel = System.Data.IsolationLevel;

namespace ASC.MessagingSystem.Data;

[Singletone(Additional = typeof(MessagesRepositoryExtension))]
public class MessagesRepository : IDisposable
{
    private DateTime _lastSave = DateTime.UtcNow;
    private bool _timerStarted;
    private readonly TimeSpan _cacheTime;
    private readonly IDictionary<string, EventMessage> _cache;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMapper _mapper;
    private readonly ILogger<MessagesRepository> _logger;
    private readonly Timer _timer;
    private readonly int _cacheLimit;
    private readonly HashSet<MessageAction> _forceSaveAuditActions = new HashSet<MessageAction>
        { MessageAction.RoomInviteLinkUsed, MessageAction.UserSentPasswordChangeInstructions };

    public MessagesRepository(IServiceScopeFactory serviceScopeFactory, ILogger<MessagesRepository> logger, IMapper mapper, IConfiguration configuration)
    {
        _cacheTime = TimeSpan.FromMinutes(1);
        _cache = new Dictionary<string, EventMessage>();
        _timerStarted = false;

        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        _timer = new Timer(FlushCache);

        _mapper = mapper;

        var minutes = configuration["messaging:CacheTimeFromMinutes"];
        var limit = configuration["messaging:CacheLimit"];

        _cacheTime = int.TryParse(minutes, out var cacheTime) ? TimeSpan.FromMinutes(cacheTime) : TimeSpan.FromMinutes(1);
        _cacheLimit = int.TryParse(limit, out var cacheLimit) ? cacheLimit : 100;
    }

    ~MessagesRepository()
    {
        FlushCache(true);
    }

    private bool ForseSave(EventMessage message)
    {
        // messages with action code < 2000 are related to login-history
        if ((int)message.Action < 2000)
        {
            return true;
        }

        return _forceSaveAuditActions.Contains(message.Action);
    }

    public int Add(EventMessage message)
    {
        if (ForseSave(message))
        {
            var id = 0;
            if (!string.IsNullOrEmpty(message.UAHeader))
            {
                try
                {
                    MessageSettings.AddInfoMessage(message);
                }
                catch (Exception e)
                {
                    _logger.ErrorWithException("Add " + message.Id, e);
                }
            }

            using var scope = _serviceScopeFactory.CreateScope();
            using var ef = scope.ServiceProvider.GetService<IDbContextFactory<MessagesContext>>().CreateDbContext();

            if ((int)message.Action < 2000)
            {
                id = AddLoginEvent(message, ef);
            }
            else
            {
                id = AddAuditEvent(message, ef);
            }
            return id;
        }

        var now = DateTime.UtcNow;
        var key = string.Format("{0}|{1}|{2}|{3}", message.TenantId, message.UserId, message.Id, now.Ticks);

        lock (_cache)
        {
            _cache[key] = message;

            if (!_timerStarted)
            {
                _timer.Change(0, 100);
                _timerStarted = true;
            }
        }
        return 0;
    }
    private void FlushCache(object state)
    {
        FlushCache(false);
    }

    private void FlushCache(bool isDisposed = false)
    {
        List<EventMessage> events = null;

        if (DateTime.UtcNow > _lastSave.Add(_cacheTime) || _cache.Count > _cacheLimit || isDisposed)
        {
            lock (_cache)
            {
                _timer.Change(-1, -1);
                _timerStarted = false;

                events = new List<EventMessage>(_cache.Values);
                _cache.Clear();
                _lastSave = DateTime.UtcNow;
            }
        }

        if (events == null)
        {
            return;
        }

        using var scope = _serviceScopeFactory.CreateScope();
        using var ef = scope.ServiceProvider.GetService<IDbContextFactory<MessagesContext>>().CreateDbContext();
        var strategy = ef.Database.CreateExecutionStrategy();

        strategy.Execute(() =>
        {
            using var tx = ef.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
            var dict = new Dictionary<string, ClientInfo>();

            foreach (var message in events)
            {
                if (!string.IsNullOrEmpty(message.UAHeader))
                {
                    try
                    {
                        MessageSettings.AddInfoMessage(message, dict);
                    }
                    catch (Exception e)
                    {
                        _logger.ErrorFlushCache(message.Id, e);
                    }
                }

                if (!ForseSave(message))
                {
                    // messages with action code < 2000 are related to login-history
                    if ((int)message.Action < 2000)
                    {
                        AddLoginEvent(message, ef);
                    }
                    else
                    {
                        AddAuditEvent(message, ef);
                    }
                }
            }

            tx.Commit();
        });
    }

    private int AddLoginEvent(EventMessage message, MessagesContext dbContext)
    {
        var loginEvent = _mapper.Map<EventMessage, LoginEvent>(message);

        dbContext.LoginEvents.Add(loginEvent);
        dbContext.SaveChanges();

        return loginEvent.Id;
    }

    private int AddAuditEvent(EventMessage message, MessagesContext dbContext)
    {
        var auditEvent = _mapper.Map<EventMessage, AuditEvent>(message);

        dbContext.AuditEvents.Add(auditEvent);
        dbContext.SaveChanges();

        return auditEvent.Id;
    }

    public void Dispose()
    {
        if (_timer != null)
        {
            _timer.Dispose();
        }
    }
}

public static class MessagesRepositoryExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<EventTypeConverter>();
    }
}
