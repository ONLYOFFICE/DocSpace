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

namespace ASC.Web.Core.Utility;

public interface IUrlShortener
{
    Task<string> GetShortenLinkAsync(string shareLink);
}

[Scope]
public class UrlShortener
{
    public bool Enabled { get { return Instance is not NullShortener; } }

    private IUrlShortener _instance;
    public IUrlShortener Instance
    {
        get
        {
            if (_instance == null)
            {
                if (_consumerFactory.Get<BitlyLoginProvider>().Enabled)
                {
                    _instance = new BitLyShortener(_consumerFactory);
                }
                else if (!string.IsNullOrEmpty(_configuration["web:url-shortener:value"]))
                {
                    _instance = new OnlyoShortener(_configuration, _commonLinkUtility, _machinePseudoKeys, _clientFactory);
                }
                else
                {
                    _instance = new NullShortener();
                }
            }

            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    private readonly IConfiguration _configuration;
    private readonly ConsumerFactory _consumerFactory;
    private readonly CommonLinkUtility _commonLinkUtility;
    private readonly MachinePseudoKeys _machinePseudoKeys;
    private readonly IHttpClientFactory _clientFactory;

    public UrlShortener(
        IConfiguration configuration,
        ConsumerFactory consumerFactory,
        CommonLinkUtility commonLinkUtility,
        MachinePseudoKeys machinePseudoKeys,
        IHttpClientFactory clientFactory)
    {
        _configuration = configuration;
        _consumerFactory = consumerFactory;
        _commonLinkUtility = commonLinkUtility;
        _machinePseudoKeys = machinePseudoKeys;
        _clientFactory = clientFactory;
    }
}

public class BitLyShortener : IUrlShortener
{
    public BitLyShortener(ConsumerFactory consumerFactory)
    {
        ConsumerFactory = consumerFactory;
    }

    private ConsumerFactory ConsumerFactory { get; }

    public Task<string> GetShortenLinkAsync(string shareLink)
    {
        return Task.FromResult(ConsumerFactory.Get<BitlyLoginProvider>().GetShortenLink(shareLink));
    }
}

public class OnlyoShortener : IUrlShortener
{
    private readonly string _url;
    private readonly string _internalUrl;
    private readonly byte[] _sKey;

    private CommonLinkUtility CommonLinkUtility { get; }
    private IHttpClientFactory ClientFactory { get; }

    public OnlyoShortener(
        IConfiguration configuration,
        CommonLinkUtility commonLinkUtility,
        MachinePseudoKeys machinePseudoKeys,
        IHttpClientFactory clientFactory)
    {
        _url = configuration["web:url-shortener:value"];
        _internalUrl = configuration["web:url-shortener:internal"];
        _sKey = machinePseudoKeys.GetMachineConstant();

        if (!_url.EndsWith('/'))
        {
            _url += '/';
        }

        CommonLinkUtility = commonLinkUtility;
        ClientFactory = clientFactory;
    }

    public async Task<string> GetShortenLinkAsync(string shareLink)
    {
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(_internalUrl + "?url=" + HttpUtility.UrlEncode(shareLink))
        };
        request.Headers.Add("Authorization", CreateAuthToken());
        request.Headers.Add("Encoding", Encoding.UTF8.ToString());//todo check 

        var httpClient = ClientFactory.CreateClient();
        using var response = await httpClient.SendAsync(request);
        using var stream = await response.Content.ReadAsStreamAsync();
        using var rs = new StreamReader(stream);
        return CommonLinkUtility.GetFullAbsolutePath(_url + await rs.ReadToEndAsync());
    }

    private string CreateAuthToken(string pkey = "urlShortener")
    {
        using var hasher = new HMACSHA1(_sKey);
        var now = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var hash = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", now, pkey))));
        return $"ASC {pkey}:{now}:{hash}";
    }
}

public class NullShortener : IUrlShortener
{
    public Task<string> GetShortenLinkAsync(string shareLink)
    {
        return Task.FromResult<string>(null);
    }
}
