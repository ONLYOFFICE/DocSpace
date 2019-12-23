using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

using ASC.Core.Common.Configuration;
using ASC.FederatedLogin.LoginProviders;
using ASC.Web.Studio.Utility;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ASC.Web.Core.Utility
{
    public interface IUrlShortener
    {
        string GetShortenLink(string shareLink);
    }

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
                    else if (!string.IsNullOrEmpty(Configuration["web:url-shortener"]))
                    {
                        _instance = new OnlyoShortener(Configuration, CommonLinkUtility);
                    }
                    else
                    {
                        _instance = new NullShortener();
                    }
                }

                return _instance;
            }
        }

        public IConfiguration Configuration { get; }
        public ConsumerFactory ConsumerFactory { get; }
        public CommonLinkUtility CommonLinkUtility { get; }

        public UrlShortener(IConfiguration configuration, ConsumerFactory consumerFactory, CommonLinkUtility commonLinkUtility)
        {
            Configuration = configuration;
            ConsumerFactory = consumerFactory;
            CommonLinkUtility = commonLinkUtility;
        }
    }

    public class BitLyShortener : IUrlShortener
    {
        public BitLyShortener(ConsumerFactory consumerFactory)
        {
            ConsumerFactory = consumerFactory;
        }

        public ConsumerFactory ConsumerFactory { get; }

        public string GetShortenLink(string shareLink)
        {
            return ConsumerFactory.Get<BitlyLoginProvider>().GetShortenLink(shareLink);
        }
    }

    public class OnlyoShortener : IUrlShortener
    {
        private readonly string url;
        private readonly string internalUrl;
        private readonly string sKey;

        public OnlyoShortener(IConfiguration configuration, CommonLinkUtility commonLinkUtility)
        {
            url = configuration["web.url-shortener"];
            internalUrl = configuration["web.url-shortener.internal"];
            sKey = configuration["core.machinekey"];

            if (!url.EndsWith("/"))
                url += '/';
            CommonLinkUtility = commonLinkUtility;
        }

        public CommonLinkUtility CommonLinkUtility { get; }

        public string GetShortenLink(string shareLink)
        {
            using var client = new WebClient { Encoding = Encoding.UTF8 };
            client.Headers.Add("Authorization", CreateAuthToken());
            return CommonLinkUtility.GetFullAbsolutePath(url + client.DownloadString(new Uri(internalUrl + "?url=" + HttpUtility.UrlEncode(shareLink))));
        }

        private string CreateAuthToken(string pkey = "urlShortener")
        {
            using var hasher = new HMACSHA1(Encoding.UTF8.GetBytes(sKey));
            var now = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var hash = Convert.ToBase64String(hasher.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", now, pkey))));
            return string.Format("ASC {0}:{1}:{2}", pkey, now, hash);
        }
    }

    public class NullShortener : IUrlShortener
    {
        public string GetShortenLink(string shareLink)
        {
            return null;
        }
    }

    public static class UrlShortenerExtension
    {
        public static IServiceCollection AddUrlShortener(this IServiceCollection services)
        {
            services.TryAddScoped<UrlShortener>();
            return services
                .AddConsumerFactoryService()
                .AddCommonLinkUtilityService();
        }
    }
}
