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

namespace ASC.Core.Notify.Signalr;

[Scope]
public class ConfigureSignalrServiceClient : IConfigureNamedOptions<SignalrServiceClient>
{
    internal readonly TenantManager TenantManager;
    internal readonly CoreSettings CoreSettings;
    internal readonly MachinePseudoKeys MachinePseudoKeys;
    internal readonly IConfiguration Configuration;
    internal readonly IOptionsMonitor<ILog> Options;
    internal readonly IHttpClientFactory ClientFactory;

    public ConfigureSignalrServiceClient(
        TenantManager tenantManager,
        CoreSettings coreSettings,
        MachinePseudoKeys machinePseudoKeys,
        IConfiguration configuration,
        IOptionsMonitor<ILog> options,
        IHttpClientFactory clientFactory)
    {
        TenantManager = tenantManager;
        CoreSettings = coreSettings;
        MachinePseudoKeys = machinePseudoKeys;
        Configuration = configuration;
        Options = options;
        ClientFactory = clientFactory;
    }

    public void Configure(string name, SignalrServiceClient options)
    {
        options.Logger = Options.CurrentValue;
        options.Hub = name.Trim('/');
        options.TenantManager = TenantManager;
        options.CoreSettings = CoreSettings;
        options.ClientFactory = ClientFactory;
        options.SKey = MachinePseudoKeys.GetMachineConstant();
        options.Url = Configuration["web:hub:internal"];
        options.EnableSignalr = !string.IsNullOrEmpty(options.Url);

        try
        {
            var replaceSetting = Configuration["jabber:replace-domain"];
            if (!string.IsNullOrEmpty(replaceSetting))
            {
                options.JabberReplaceDomain = true;
                var q =
                    replaceSetting.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim().ToLowerInvariant())
                        .ToList();
                options.JabberReplaceFromDomain = q.ElementAt(0);
                options.JabberReplaceToDomain = q.ElementAt(1);
            }
        }
        catch (Exception) { }
    }

    public void Configure(SignalrServiceClient options)
    {
        Configure("default", options);
    }
}

[Scope(typeof(ConfigureSignalrServiceClient))]
public class SignalrServiceClient
{
    private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(1);
    internal ILog Logger;
    private static DateTime _lastErrorTime;
    public bool EnableSignalr { get; set; }
    internal byte[] SKey;
    internal string Url;
    internal bool JabberReplaceDomain;
    internal string JabberReplaceFromDomain;
    internal string JabberReplaceToDomain;

    internal string Hub;

    internal TenantManager TenantManager;
    internal CoreSettings CoreSettings;
    internal IHttpClientFactory ClientFactory;

    public SignalrServiceClient() { }

    public void SendMessage(string callerUserName, string calleeUserName, string messageText, int tenantId,
        string domain)
    {
        try
        {
            domain = ReplaceDomain(domain);
            var tenant = tenantId == -1
                ? TenantManager.GetTenant(domain)
                : TenantManager.GetTenant(tenantId);
            var isTenantUser = callerUserName.Length == 0;
            var message = new MessageClass
            {
                UserName = isTenantUser ? tenant.GetTenantDomain(CoreSettings) : callerUserName,
                Text = messageText
            };

            MakeRequest("send", new { tenantId = tenant.Id, callerUserName, calleeUserName, message, isTenantUser });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void SendInvite(string chatRoomName, string calleeUserName, string domain)
    {
        try
        {
            domain = ReplaceDomain(domain);

            var tenant = TenantManager.GetTenant(domain);

            var message = new MessageClass
            {
                UserName = tenant.GetTenantDomain(CoreSettings),
                Text = chatRoomName
            };

            MakeRequest("sendInvite", new { tenantId = tenant.Id, calleeUserName, message });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void SendState(string from, byte state, int tenantId, string domain)
    {
        try
        {
            domain = ReplaceDomain(domain);

            if (tenantId == -1)
            {
                tenantId = TenantManager.GetTenant(domain).Id;
            }

            MakeRequest("setState", new { tenantId, from, state });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void SendOfflineMessages(string callerUserName, List<string> users, int tenantId)
    {
        try
        {
            MakeRequest("sendOfflineMessages", new { tenantId, callerUserName, users });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void SendUnreadCounts(Dictionary<string, int> unreadCounts, string domain)
    {
        try
        {
            domain = ReplaceDomain(domain);

            var tenant = TenantManager.GetTenant(domain);

            MakeRequest("sendUnreadCounts", new { tenantId = tenant.Id, unreadCounts });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void SendUnreadUsers(Dictionary<int, Dictionary<Guid, int>> unreadUsers)
    {
        try
        {
            MakeRequest("sendUnreadUsers", unreadUsers);
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void SendUnreadUser(int tenant, string userId, int count)
    {
        try
        {
            MakeRequest("updateFolders", new { tenant, userId, count });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void SendMailNotification(int tenant, string userId, int state)
    {
        try
        {
            MakeRequest("sendMailNotification", new { tenant, userId, state });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void EnqueueCall(string numberId, string callId, string agent)
    {
        try
        {
            MakeRequest("enqueue", new { numberId, callId, agent });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void IncomingCall(string callId, string agent)
    {
        try
        {
            MakeRequest("incoming", new { callId, agent });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void MissCall(string numberId, string callId, string agent)
    {
        try
        {
            MakeRequest("miss", new { numberId, callId, agent });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void Reload(string numberId, string agentId = null)
    {
        try
        {
            var numberRoom = TenantManager.GetCurrentTenant().Id + numberId;
            MakeRequest("reload", new { numberRoom, agentId });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void StartEdit<T>(T fileId, string room)
    {
        try
        {
            MakeRequest("start-edit", new { room, fileId });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
     }

    public void StopEdit<T>(T fileId, string room, string data)
    {
        try
        {
            MakeRequest("stop-edit", new { room, fileId, data });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void CreateFile<T>(T fileId, string room, string data)
    {
        try
        {
            MakeRequest("create-file", new { room, fileId, data });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public void DeleteFile<T>(T fileId, string room)
    {
        try
        {
            MakeRequest("delete-file", new { room, fileId });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }
    }

    public T GetAgent<T>(string numberId, List<Guid> contactsResponsibles)
    {
        try
        {
            return MakeRequest<T>("GetAgent", new { numberId, contactsResponsibles });
        }
        catch (Exception error)
        {
            ProcessError(error);
        }

        return default;
    }

    private string ReplaceDomain(string domain)
    {
        if (JabberReplaceDomain && domain.EndsWith(JabberReplaceFromDomain))
        {
            var place = domain.LastIndexOf(JabberReplaceFromDomain);
            if (place >= 0)
            {
                return domain.Remove(place, JabberReplaceFromDomain.Length).Insert(place, JabberReplaceToDomain);
            }
        }

        return domain;
    }

        private void ProcessError(Exception e)
        {
            Logger.ErrorFormat("Service Error: {0}, {1}, {2}", e.Message, e.StackTrace,
                (e.InnerException != null) ? e.InnerException.Message : string.Empty);

            if (e is HttpRequestException)
            {
                _lastErrorTime = DateTime.Now;
            }
        }

    private string MakeRequest(string method, object data)
    {
        if (!IsAvailable())
        {
            return string.Empty;
        }

        var request = new HttpRequestMessage();
        request.Headers.Add("Authorization", CreateAuthToken());
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(GetMethod(method));

        var jsonData = JsonConvert.SerializeObject(data);
        Logger.DebugFormat("Method:{0}, Data:{1}", method, jsonData);

        request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var httpClient = ClientFactory.CreateClient();

        using (var response = httpClient.Send(request))
        using (var stream = response.Content.ReadAsStream())
        using (var streamReader = new StreamReader(stream))
        {
            return streamReader.ReadToEnd();
        }
    }

    private T MakeRequest<T>(string method, object data)
    {
        var resultMakeRequest = MakeRequest(method, data);

        return JsonConvert.DeserializeObject<T>(resultMakeRequest);
    }

    private bool IsAvailable()
    {
        return EnableSignalr && _lastErrorTime + _timeout < DateTime.Now;
    }

    private string GetMethod(string method)
    {
        return $"{Url.TrimEnd('/')}/controller/{Hub}/{method}";
    }

    public string CreateAuthToken(string pkey = "socketio")
    {
        using var hasher = new HMACSHA1(SKey);
        var now = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var hash = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", now, pkey))));

        return $"ASC {pkey}:{now}:{hash}";
    }
}
