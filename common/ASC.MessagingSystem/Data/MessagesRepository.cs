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

    public MessagesRepository(IServiceScopeFactory serviceScopeFactory, IOptionsMonitor<ILog> options, IMapper mapper)
    {
        _cacheTime = TimeSpan.FromMinutes(1);
        _cache = new Dictionary<string, EventMessage>();
        _timerStarted = false;

        _logger = options.CurrentValue;
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
    }
}
