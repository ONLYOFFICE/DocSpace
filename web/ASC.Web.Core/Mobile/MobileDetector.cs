namespace ASC.Web.Core.Mobile
{
    [Scope]
    public class MobileDetector
    {
        private readonly Regex uaMobileRegex;

        private ICache cache { get; set; }

        private IHttpContextAccessor HttpContextAccessor { get; }

        public bool IsMobile()
        {
            return IsRequestMatchesMobile();
        }


        public MobileDetector(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ICache cache)
        {
            this.cache = cache;
            if (!string.IsNullOrEmpty(configuration["mobile:regex"]))
            {
                uaMobileRegex = new Regex(configuration["mobile:regex"], RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            }

            HttpContextAccessor = httpContextAccessor;
        }


        public bool IsRequestMatchesMobile()
        {
            bool? result = false;
            var ua = HttpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString();
            var regex = uaMobileRegex;
            if (!string.IsNullOrEmpty(ua) && regex != null)
            {
                var key = "mobileDetector/" + ua.GetHashCode();


                if (bool.TryParse(cache.Get<string>(key), out var fromCache))
                {
                    result = fromCache;
                }
                else
                {
                    result = regex.IsMatch(ua);
                    cache.Insert(key, result.ToString(), TimeSpan.FromMinutes(10));
                }
            }
            return result.GetValueOrDefault();
        }
    }
}