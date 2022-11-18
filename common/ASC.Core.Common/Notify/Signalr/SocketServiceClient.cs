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

namespace ASC.Core.Notify.Socket;

[Scope]
public class ConfigureSocketServiceClient : IConfigureNamedOptions<SocketServiceClient>
{
    internal readonly TenantManager _tenantManager;
    internal readonly CoreSettings _coreSettings;
    internal readonly MachinePseudoKeys _machinePseudoKeys;
    internal readonly IConfiguration _configuration;
    internal readonly ILogger<SocketServiceClient> logger;
    internal readonly IHttpClientFactory _clientFactory;

    public ConfigureSocketServiceClient(
        TenantManager tenantManager,
        CoreSettings coreSettings,
        MachinePseudoKeys machinePseudoKeys,
        IConfiguration configuration,
        ILogger<SocketServiceClient> logger,
        IHttpClientFactory clientFactory)
    {
        _tenantManager = tenantManager;
        _coreSettings = coreSettings;
        _machinePseudoKeys = machinePseudoKeys;
        _configuration = configuration;
        this.logger = logger;
        _clientFactory = clientFactory;
    }

    public void Configure(string name, SocketServiceClient options)
    {
        options._logger = logger;
        options._hub = name.Trim('/');
        options._tenantManager = _tenantManager;
        options._coreSettings = _coreSettings;
        options._clientFactory = _clientFactory;
        options._sKey = _machinePseudoKeys.GetMachineConstant();
        options._url = _configuration["web:hub:internal"];
        options.EnableSocket = !string.IsNullOrEmpty(options._url);

        try
        {
            var replaceSetting = _configuration["jabber:replace-domain"];
            if (!string.IsNullOrEmpty(replaceSetting))
            {
                options._jabberReplaceDomain = true;
                var q =
                    replaceSetting.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim().ToLowerInvariant())
                        .ToList();
                options._jabberReplaceFromDomain = q.ElementAt(0);
                options._jabberReplaceToDomain = q.ElementAt(1);
            }
        }
        catch (Exception) { }
    }

    public void Configure(SocketServiceClient options)
    {
        Configure("default", options);
    }
}

[Scope(typeof(ConfigureSocketServiceClient))]
public class SocketServiceClient
{
    private static readonly TimeSpan _timeout = TimeSpan.FromSeconds(1);
    internal ILogger<SocketServiceClient> _logger;
    private static DateTime _lastErrorTime;
    public bool EnableSocket { get; set; }
    internal byte[] _sKey;
    internal string _url;
    internal bool _jabberReplaceDomain;
    internal string _jabberReplaceFromDomain;
    internal string _jabberReplaceToDomain;

    internal string _hub;

    internal TenantManager _tenantManager;
    internal CoreSettings _coreSettings;
    internal IHttpClientFactory _clientFactory;

    public SocketServiceClient() { }

    public void SendMessage(string callerUserName, string calleeUserName, string messageText, int tenantId,
        string domain)
    {
        try
        {
            domain = ReplaceDomain(domain);
            var tenant = tenantId == -1
                ? _tenantManager.GetTenant(domain)
                : _tenantManager.GetTenant(tenantId);
            var isTenantUser = callerUserName.Length == 0;
            var message = new MessageClass
            {
                UserName = isTenantUser ? tenant.GetTenantDomain(_coreSettings) : callerUserName,
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

            var tenant = _tenantManager.GetTenant(domain);

            var message = new MessageClass
            {
                UserName = tenant.GetTenantDomain(_coreSettings),
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
                tenantId = _tenantManager.GetTenant(domain).Id;
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

            var tenant = _tenantManager.GetTenant(domain);

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

    public void SendMailNotification(int tenant, string userId, MailNotificationState state)
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
            var numberRoom = _tenantManager.GetCurrentTenant().Id + numberId;
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

    public void StopEdit<T>(T fileId, string room)
    {
        try
        {
            MakeRequest("stop-edit", new { room, fileId });
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

    public void UpdateFile<T>(T fileId, string room, string data)
    {
        try
        {
            MakeRequest("update-file", new { room, fileId, data });
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
        if (_jabberReplaceDomain && domain.EndsWith(_jabberReplaceFromDomain))
        {
            var place = domain.LastIndexOf(_jabberReplaceFromDomain);
            if (place >= 0)
            {
                return domain.Remove(place, _jabberReplaceFromDomain.Length).Insert(place, _jabberReplaceToDomain);
            }
        }

        return domain;
    }

    private void ProcessError(Exception e)
    {
        _logger.ErrorService(e);

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
        _logger.DebugMakeRequest(method, jsonData);

        request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var httpClient = _clientFactory.CreateClient();

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
        return EnableSocket && _lastErrorTime + _timeout < DateTime.Now;
    }

    private string GetMethod(string method)
    {
        return $"{_url.TrimEnd('/')}/controller/{_hub}/{method}";
    }

    public string CreateAuthToken(string pkey = "socketio")
    {
        using var hasher = new HMACSHA1(_sKey);
        var now = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var hash = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", now, pkey))));

        return $"ASC {pkey}:{now}:{hash}";
    }
}
