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

namespace ASC.ApiSystem.Controllers
{
    [Scope]
    public class CommonMethods
    {
        private IHttpContextAccessor HttpContextAccessor { get; }

        private IConfiguration Configuration { get; }

        private ILog Log { get; }

        private CoreSettings CoreSettings { get; }

        private CommonLinkUtility CommonLinkUtility { get; }

        private EmailValidationKeyProvider EmailValidationKeyProvider { get; }

        private TimeZoneConverter TimeZoneConverter { get; }

        private CommonConstants CommonConstants { get; }

        private HostedSolution HostedSolution { get; }

        private IMemoryCache MemoryCache { get; }

        private CoreBaseSettings CoreBaseSettings { get; }

        private TenantManager TenantManager { get; }

        public CommonMethods(
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IOptionsMonitor<ILog> option,
            CoreSettings coreSettings,
            CommonLinkUtility commonLinkUtility,
            EmailValidationKeyProvider emailValidationKeyProvider,
            TimeZoneConverter timeZoneConverter, CommonConstants commonConstants,
            IMemoryCache memoryCache,
            IOptionsSnapshot<HostedSolution> hostedSolutionOptions,
            CoreBaseSettings coreBaseSettings,
            TenantManager tenantManager)
        {
            HttpContextAccessor = httpContextAccessor;

            Configuration = configuration;

            Log = option.Get("ASC.ApiSystem");

            CoreSettings = coreSettings;

            CommonLinkUtility = commonLinkUtility;

            EmailValidationKeyProvider = emailValidationKeyProvider;

            TimeZoneConverter = timeZoneConverter;

            CommonConstants = commonConstants;

            MemoryCache = memoryCache;
            CoreBaseSettings = coreBaseSettings;
            TenantManager = tenantManager;
            HostedSolution = hostedSolutionOptions.Get(CommonConstants.BaseDbConnKeyString);
        }


        public object ToTenantWrapper(Tenant t)
        {
            return new
            {
                created = t.CreatedDateTime,
                domain = t.GetTenantDomain(CoreSettings),
                hostedRegion = t.HostedRegion,
                industry = t.Industry,
                language = t.Language,
                name = t.Name,
                ownerId = t.OwnerId,
                partnerId = t.PartnerId,
                paymentId = t.PaymentId,
                portalName = t.TenantAlias,
                status = t.Status.ToString(),
                tenantId = t.TenantId,
                timeZoneName = TimeZoneConverter.GetTimeZone(t.TimeZone).DisplayName,
            };
        }

        public string CreateReference(string requestUriScheme, string tenantDomain, string email, bool first = false, string module = "", bool sms = false)
        {
            return string.Format("{0}{1}{2}/{3}{4}{5}{6}",
                                 requestUriScheme,
                                 Uri.SchemeDelimiter,
                                 tenantDomain,
                                 CommonLinkUtility.GetConfirmationUrlRelative(email, ConfirmType.Auth, (first ? "true" : "") + module + (sms ? "true" : "")),
                                 first ? "&first=true" : "",
                                 string.IsNullOrEmpty(module) ? "" : "&module=" + module,
                                 sms ? "&sms=true" : ""

                );
        }

        public bool SendCongratulations(string requestUriScheme, Tenant tenant, bool skipWelcome, out string url)
        {
            var validationKey = EmailValidationKeyProvider.GetEmailKey(tenant.TenantId, tenant.OwnerId.ToString() + ConfirmType.Auth);

            url = string.Format("{0}{1}{2}{3}{4}?userid={5}&key={6}",
                                requestUriScheme,
                                Uri.SchemeDelimiter,
                                tenant.GetTenantDomain(CoreSettings),
                                CommonConstants.WebApiBaseUrl,
                                "portal/sendcongratulations",
                                tenant.OwnerId,
                                validationKey);

            if (skipWelcome)
            {
                Log.DebugFormat("congratulations skiped");
                return false;
            }

            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/x-www-form-urlencoded"));

            try
            {
                using var httpClient = new HttpClient();
                using var response = httpClient.Send(request);
                using var stream = response.Content.ReadAsStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);

                var result = reader.ReadToEnd();

                Log.DebugFormat("congratulations result = {0}", result);

                var resObj = JObject.Parse(result);

                if (resObj["errors"] != null && resObj["errors"].HasValues)
                {
                    throw new Exception(result);
                }
            }
            catch (Exception ex)
            {
                Log.Error("SendCongratulations error", ex);
                return false;
            }

            url = null;
            return true;
        }

        public bool GetTenant(IModel model, out Tenant tenant)
        {
            if (CoreBaseSettings.Standalone && model != null && !string.IsNullOrEmpty((model.PortalName ?? "").Trim()))
            {
                tenant = TenantManager.GetTenant((model.PortalName ?? "").Trim());
                return true;
            }

            if (model != null && model.TenantId.HasValue)
            {
                tenant = HostedSolution.GetTenant(model.TenantId.Value);
                return true;
            }

            if (model != null && !string.IsNullOrEmpty((model.PortalName ?? "").Trim()))
            {
                tenant = HostedSolution.GetTenant((model.PortalName ?? "").Trim());
                return true;
            }

            tenant = null;
            return false;
        }

        public bool IsTestEmail(string email)
        {
            //the point is not needed in gmail.com
            email = Regex.Replace(email ?? "", "\\.*(?=\\S*(@gmail.com$))", "").ToLower();
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(CommonConstants.AutotestSecretEmails))
                return false;

            var regex = new Regex(CommonConstants.AutotestSecretEmails, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
            return regex.IsMatch(email);
        }

        public bool CheckMuchRegistration(TenantModel model, string clientIP, Stopwatch sw)
        {
            if (IsTestEmail(model.Email)) return false;

            Log.DebugFormat("clientIP = {0}", clientIP);

            var cacheKey = "ip_" + clientIP;

            if (MemoryCache.TryGetValue(cacheKey, out int ipAttemptsCount))
            {
                MemoryCache.Remove(cacheKey);
            }

            ipAttemptsCount++;

            MemoryCache.Set(
                // String that represents the name of the cache item,
                // could be any string
                cacheKey,
                // Something to store in the cache
                ipAttemptsCount,
                new MemoryCacheEntryOptions
                {
                    // Will not use absolute cache expiration
                    AbsoluteExpiration = DateTime.MaxValue,
                    // Cache will expire after one hour
                    // You can change this time interval according
                    // to your requriements
                    SlidingExpiration = CommonConstants.MaxAttemptsTimeInterval,
                    // Cache will not be removed before expired
                    Priority = CacheItemPriority.NeverRemove
                });

            if (ipAttemptsCount <= CommonConstants.MaxAttemptsCount) return false;

            Log.DebugFormat("PortalName = {0}; Too much reqests from ip: {1}", model.PortalName, clientIP);
            sw.Stop();

            return true;
        }

        public string GetClientIp()
        {
            return HttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            //TODO: check old version

            //if (request.Properties.ContainsKey("MS_HttpContext"))
            //{
            //    return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            //}

            //if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            //{
            //    var prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
            //    return prop.Address;
            //}

            //return null;
        }

        public bool ValidateRecaptcha(string response, RecaptchaType recaptchaType, string ip)
        {
            try
            {
                string privateKey;
                switch (recaptchaType)
                {
                    case RecaptchaType.AndroidV2:
                        privateKey = Configuration["recaptcha:private-key:android"];
                        break;
                    case RecaptchaType.iOSV2:
                        privateKey = Configuration["recaptcha:private-key:ios"];
                        break;
                    default:
                        privateKey = Configuration["recaptcha:private-key"];
                        break;
                }

                var data = string.Format("secret={0}&remoteip={1}&response={2}", privateKey, ip, response);
                var url = Configuration["recaptcha:verify-url"] ?? "https://www.recaptcha.net/recaptcha/api/siteverify";

                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(url);
                request.Method = HttpMethod.Post;
                request.Content = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                using var httpClient = new HttpClient();
                using var httpClientResponse = httpClient.Send(request);
                using var stream = httpClientResponse.Content.ReadAsStream();
                using var reader = new StreamReader(stream);

                var resp = reader.ReadToEnd();
                var resObj = JObject.Parse(resp);

                if (resObj["success"] != null && resObj.Value<bool>("success"))
                {
                    return true;
                }
                else
                {
                    Log.DebugFormat("Recaptcha error: {0}", resp);
                }

                if (resObj["error-codes"] != null && resObj["error-codes"].HasValues)
                {
                    Log.DebugFormat("Recaptcha api returns errors: {0}", resp);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return false;
        }
    }
}