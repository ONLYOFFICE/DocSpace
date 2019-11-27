using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using ASC.FederatedLogin.LoginProviders;
using ASC.Web.Studio.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ASC.Web.Core.Utility
{
    public interface IUrlShortener
    {
        string GetShortenLink(string shareLink, CommonLinkUtility commonLinkUtility);
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
                    if (BitlyLoginProvider.Enabled)
                    {
                        _instance = new BitLyShortener();
                    }
                    else if (!string.IsNullOrEmpty(Configuration["web:url-shortener"]))
                    {
                        _instance = new OnlyoShortener(Configuration);
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

        public UrlShortener(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    }

    public class BitLyShortener : IUrlShortener
    {
        public string GetShortenLink(string shareLink, CommonLinkUtility commonLinkUtility)
        {
            return BitlyLoginProvider.GetShortenLink(shareLink);
        }
    }

    public class OnlyoShortener : IUrlShortener
    {
        private readonly string url;
        private readonly string internalUrl;
        private readonly string sKey;

        public OnlyoShortener(IConfiguration configuration)
        {
            url = configuration["web.url-shortener"];
            internalUrl = configuration["web.url-shortener.internal"];
            sKey = configuration["core.machinekey"];

            if (!url.EndsWith("/"))
                url += '/';
        }

        public string GetShortenLink(string shareLink, CommonLinkUtility commonLinkUtility)
        {
            using var client = new WebClient { Encoding = Encoding.UTF8 };
            client.Headers.Add("Authorization", CreateAuthToken());
            return commonLinkUtility.GetFullAbsolutePath(url + client.DownloadString(new Uri(internalUrl + "?url=" + HttpUtility.UrlEncode(shareLink))));
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
        public string GetShortenLink(string shareLink, CommonLinkUtility commonLinkUtility)
        {
            return null;
        }
    }

    public static class UrlShortenerExtension
    {
        public static IServiceCollection AddUrlShortener(this IServiceCollection services)
        {
            services.TryAddSingleton<UrlShortener>();
            return services;
        }
    }
}
