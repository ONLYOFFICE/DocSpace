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
    private readonly ILog _logger;
    private readonly Timer _timer;
    private Parser _parser;

    public MessagesRepository(IServiceScopeFactory serviceScopeFactory, ILog logger, IMapper mapper)
    {
        _cacheTime = TimeSpan.FromMinutes(1);
        _cache = new Dictionary<string, EventMessage>();
        _timerStarted = false;

        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        _timer = new Timer(FlushCache);

        _mapper = mapper;
    }

    public void Add(EventMessage message)
    {
        // messages with action code < 2000 are related to login-history
        if ((int)message.Action < 2000)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            using var ef = scope.ServiceProvider.GetService<DbContextManager<MessagesContext>>().Get("messages");

            AddLoginEvent(message, ef);

            return;
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

    }

    private void FlushCache(object state)
    {
        List<EventMessage> events = null;

        if (_cacheTime < DateTime.UtcNow - _lastSave || _cache.Count > 100)
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

                    if (dict.TryGetValue(message.UAHeader, out clientInfo))
                    {

                    }
                    else
                    {
                        _parser = _parser ?? Parser.GetDefault();
                        clientInfo = _parser.Parse(message.UAHeader);
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
                    _logger.Error("FlushCache " + message.Id, e);
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

    private void AddLoginEvent(EventMessage message, MessagesContext dbContext)
    {
        var loginEvent = _mapper.Map<EventMessage, LoginEvent>(message);

        dbContext.LoginEvents.Add(loginEvent);
        dbContext.SaveChanges();
    }

    private void AddAuditEvent(EventMessage message, MessagesContext dbContext)
    {
        var auditEvent = _mapper.Map<EventMessage, AuditEvent>(message);

        dbContext.AuditEvents.Add(auditEvent);
        dbContext.SaveChanges();
    }

    private static string GetBrowser(ClientInfo clientInfo)
    {
        return clientInfo == null
                   ? null
                   : $"{clientInfo.UA.Family} {clientInfo.UA.Major}";
    }

    private static string GetPlatform(ClientInfo clientInfo)
    {
        return clientInfo == null
                   ? null
                   : $"{clientInfo.OS.Family} {clientInfo.OS.Major}";
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
        services.TryAdd<DbContextManager<MessagesContext>>();
        services.TryAdd<EventTypeConverter>();
    }
}
