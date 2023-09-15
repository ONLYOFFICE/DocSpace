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
public class SocketServiceClient
{
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(1);
    private DateTime _lastErrorTime;

    private readonly IHttpClientFactory _clientFactory;
    private readonly ILogger<SocketServiceClient> _logger;
    private readonly bool _enableSocket;
    private readonly byte[] _sKey;
    private readonly string _url;

    public virtual string Hub { get => "default"; }

    public SocketServiceClient(
        ILogger<SocketServiceClient> logger,
        IHttpClientFactory clientFactory,
        MachinePseudoKeys mashinePseudoKeys,
        IConfiguration configuration)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _sKey = mashinePseudoKeys.GetMachineConstant();
        _url = configuration["web:hub:internal"];
        _enableSocket = !string.IsNullOrEmpty(_url);
    }

    public async Task<string> MakeRequest(string method, object data)
    {
        if (!IsAvailable())
        {
            return string.Empty;
        }
        try
        {
            var request = GenerateRequest(method, data);
            var httpClient = _clientFactory.CreateClient();

            //async
            using (var response = await httpClient.SendAsync(request))
            await using (var stream = await response.Content.ReadAsStreamAsync())
            using (var streamReader = new StreamReader(stream))
            {
                return await streamReader.ReadToEndAsync();
            }
        }
        catch (Exception e)
        {
            _logger.ErrorService(e);

            if (e is HttpRequestException)
            {
                _lastErrorTime = DateTime.Now;
            }
        }

        return null;
    }

    public async Task<T> MakeRequest<T>(string method, object data)
    {
        var resultMakeRequest = await MakeRequest(method, data);

        return JsonConvert.DeserializeObject<T>(resultMakeRequest);
    }
    
    protected void SendNotAwaitableRequest(string method, object data)
    {
        var request = GenerateRequest(method, data);
        var httpClient = _clientFactory.CreateClient();
        
        _ = httpClient.SendAsync(request);
    }

    private bool IsAvailable()
    {
        return _enableSocket && _lastErrorTime + _timeout < DateTime.Now;
    }
    
    private HttpRequestMessage GenerateRequest(string method, object data)
    {
        var request = new HttpRequestMessage();
        request.Headers.Add("Authorization", CreateAuthToken());
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(GetMethod(method));

        var jsonData = JsonConvert.SerializeObject(data);
        _logger.DebugMakeRequest(method, jsonData);

        request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        return request;
    }

    private string GetMethod(string method)
    {
        return $"{_url.TrimEnd('/')}/controller/{Hub}/{method}";
    }

    private string CreateAuthToken(string pkey = "socketio")
    {
        using var hasher = new HMACSHA1(_sKey);
        var now = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var hash = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", now, pkey))));

        return $"ASC {pkey}:{now}:{hash}";
    }
}
