namespace ASC.Web.Core.Helpers
{
    [Scope]
    public class ApiSystemHelper
    {
        public string ApiSystemUrl { get; private set; }
        public string ApiCacheUrl { get; private set; }
        private static byte[] Skey { get; set; }
        private CommonLinkUtility CommonLinkUtility { get; }
        private IHttpClientFactory ClientFactory { get; }

        public ApiSystemHelper(IConfiguration configuration,
            CommonLinkUtility commonLinkUtility,
            MachinePseudoKeys machinePseudoKeys, 
            IHttpClientFactory clientFactory)
        {
            ApiSystemUrl = configuration["web:api-system"];
            ApiCacheUrl = configuration["web:api-cache"];
            CommonLinkUtility = commonLinkUtility;
            Skey = machinePseudoKeys.GetMachineConstant();
            ClientFactory = clientFactory;
        }


        public string CreateAuthToken(string pkey)
        {
            using var hasher = new HMACSHA1(Skey);
            var now = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var hash = WebEncoders.Base64UrlEncode(hasher.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", now, pkey))));
            return $"ASC {pkey}:{now}:{hash}";
        }

        #region system

        public async Task ValidatePortalNameAsync(string domain, Guid userId)
        {
            try
            {
                var data = $"portalName={HttpUtility.UrlEncode(domain)}";
                await SendToApiAsync(ApiSystemUrl, "portal/validateportalname", WebRequestMethods.Http.Post, userId, data);
            }
            catch (WebException exception)
            {
                if (exception.Status != WebExceptionStatus.ProtocolError || exception.Response == null) return;

                var response = exception.Response;
                try
                {
                    using var stream = response.GetResponseStream();
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    var result = await reader.ReadToEndAsync();

                    var resObj = JObject.Parse(result);
                    if (resObj["error"] != null)
                    {
                        if (resObj["error"].ToString() == "portalNameExist")
                        {
                            var varians = resObj.Value<JArray>("variants").Select(jv => jv.Value<string>());
                            throw new TenantAlreadyExistsException("Address busy.", varians);
                        }

                        throw new Exception(resObj["error"].ToString());
                    }
                }
                finally
                {
                    if (response != null)
                    {
                        response.Close();
                    }
                }
            }
        }

        #endregion

        #region cache

        public async Task AddTenantToCacheAsync(string domain, Guid userId)
        {
            var data = $"portalName={HttpUtility.UrlEncode(domain)}";
            await SendToApiAsync(ApiCacheUrl, "portal/add", WebRequestMethods.Http.Post, userId, data);
        }

        public async Task RemoveTenantFromCacheAsync(string domain, Guid userId)
        {
            await SendToApiAsync(ApiCacheUrl, "portal/remove?portalname=" + HttpUtility.UrlEncode(domain), "DELETE", userId);
        }

        public async Task<IEnumerable<string>> FindTenantsInCacheAsync(string domain, Guid userId)
        {
            var result = await SendToApiAsync(ApiCacheUrl, "portal/find?portalname=" + HttpUtility.UrlEncode(domain), WebRequestMethods.Http.Get, userId);
            var resObj = JObject.Parse(result);

            var variants = resObj.Value<JArray>("variants");
            return variants?.Select(jv => jv.Value<string>()).ToList();
        }

        #endregion

        private async Task<string> SendToApiAsync(string absoluteApiUrl, string apiPath, string httpMethod, Guid userId, string data = null)
        {
            if (!Uri.TryCreate(absoluteApiUrl, UriKind.Absolute, out var uri))
            {
                var appUrl = CommonLinkUtility.GetFullAbsolutePath("/");
                absoluteApiUrl = $"{appUrl.TrimEnd('/')}/{absoluteApiUrl.TrimStart('/')}".TrimEnd('/');
            }

            var url = $"{absoluteApiUrl}/{apiPath}";

            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(url);
            request.Method = new HttpMethod(httpMethod);
            request.Headers.Add("Authorization", CreateAuthToken(userId.ToString()));
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

            if (data != null)
            {
                request.Content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            var httpClient = ClientFactory.CreateClient();
            using var response = await httpClient.SendAsync(request);
            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
    }
}