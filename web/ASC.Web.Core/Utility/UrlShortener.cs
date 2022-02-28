using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using ASC.Common;
using ASC.Core.Common.Configuration;
using ASC.FederatedLogin.LoginProviders;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.Configuration;

namespace ASC.Web.Core.Utility
{
    public interface IUrlShortener
    {
        Task<string> GetShortenLinkAsync(string shareLink);
    }

    [Scope]
    public class UrlShortener
    {
        public bool Enabled { get { return !(Instance is NullShortener); } }

        private IUrlShortener _instance;
        public IUrlShortener Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (ConsumerFactory.Get<BitlyLoginProvider>().Enabled)
                    {
                        _instance = new BitLyShortener(ConsumerFactory);
                    }
                    else if (!string.IsNullOrEmpty(Configuration["web:url-shortener:value"]))
                    {
                        _instance = new OnlyoShortener(Configuration, CommonLinkUtility, MachinePseudoKeys, ClientFactory);
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

        private IConfiguration Configuration { get; }
        private ConsumerFactory ConsumerFactory { get; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private MachinePseudoKeys MachinePseudoKeys { get; }
        private IHttpClientFactory ClientFactory { get; }

        public UrlShortener(
            IConfiguration configuration,
            ConsumerFactory consumerFactory,
            CommonLinkUtility commonLinkUtility,
            MachinePseudoKeys machinePseudoKeys, 
            IHttpClientFactory clientFactory)
        {
            Configuration = configuration;
            ConsumerFactory = consumerFactory;
            CommonLinkUtility = commonLinkUtility;
            MachinePseudoKeys = machinePseudoKeys;
            ClientFactory = clientFactory;
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
        private readonly string url;
        private readonly string internalUrl;
        private readonly byte[] sKey;

        private CommonLinkUtility CommonLinkUtility { get; }
        private IHttpClientFactory ClientFactory { get; }

        public OnlyoShortener(
            IConfiguration configuration,
            CommonLinkUtility commonLinkUtility,
            MachinePseudoKeys machinePseudoKeys,
            IHttpClientFactory clientFactory)
        {
            url = configuration["web:url-shortener:value"];
            internalUrl = configuration["web:url-shortener:internal"];
            sKey = machinePseudoKeys.GetMachineConstant();

            if (!url.EndsWith('/'))
                url += '/';
            CommonLinkUtility = commonLinkUtility;
            ClientFactory = clientFactory;
        }

        public async Task<string> GetShortenLinkAsync(string shareLink)
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(internalUrl + "?url=" + HttpUtility.UrlEncode(shareLink));
            request.Headers.Add("Authorization", CreateAuthToken());
            request.Headers.Add("Encoding", Encoding.UTF8.ToString());//todo check 

            var httpClient = ClientFactory.CreateClient();
            using var response = await httpClient.SendAsync(request);
            using var stream = await response.Content.ReadAsStreamAsync();
            using var rs = new StreamReader(stream);
            return CommonLinkUtility.GetFullAbsolutePath(url + await rs.ReadToEndAsync());
        }

        private string CreateAuthToken(string pkey = "urlShortener")
        {
            using var hasher = new HMACSHA1(sKey);
            var now = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var hash = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", now, pkey))));
            return $"ASC {pkey}:{now}:{hash}";
        }
    }

    public class NullShortener : IUrlShortener
    {
        public Task<string> GetShortenLinkAsync(string shareLink)
        {
            return null;
        }
    }
}
