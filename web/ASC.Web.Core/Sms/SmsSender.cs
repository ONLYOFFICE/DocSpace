namespace ASC.Web.Core.Sms
{
    [Scope]
    public class SmsSender
    {
        private IConfiguration Configuration { get; }
        private TenantManager TenantManager { get; }
        private SmsProviderManager SmsProviderManager { get; }
        public ILog Log { get; }

        public SmsSender(
            IConfiguration configuration,
            TenantManager tenantManager,
            IOptionsMonitor<ILog> options,
            SmsProviderManager smsProviderManager)
        {
            Configuration = configuration;
            TenantManager = tenantManager;
            SmsProviderManager = smsProviderManager;
            Log = options.CurrentValue;
        }

        public Task<bool> SendSMSAsync(string number, string message)
        {
            ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(number);
            ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(message);

            if (!SmsProviderManager.Enabled())
            {
                throw new MethodAccessException();
            }

            if ("log".Equals(Configuration["core:notify:postman"], StringComparison.InvariantCultureIgnoreCase))
            {
                var tenant = TenantManager.GetCurrentTenant(false);
                var tenantId = tenant == null ? Tenant.DefaultTenant : tenant.Id;

                Log.InfoFormat("Tenant {0} send sms to phoneNumber {1} Message: {2}", tenantId, number, message);
                return Task.FromResult(false);
            }

            number = new Regex("[^\\d+]").Replace(number, string.Empty);
            return SmsProviderManager.SendMessageAsync(number, message);
        }

        public static string GetPhoneValueDigits(string mobilePhone)
        {
            var reg = new Regex(@"[^\d]");
            mobilePhone = reg.Replace(mobilePhone ?? "", string.Empty).Trim();
            return mobilePhone.Substring(0, Math.Min(64, mobilePhone.Length));
        }

        public static string BuildPhoneNoise(string mobilePhone)
        {
            if (string.IsNullOrEmpty(mobilePhone))
                return string.Empty;

            mobilePhone = GetPhoneValueDigits(mobilePhone);

            const int startLen = 4;
            const int endLen = 4;
            if (mobilePhone.Length < startLen + endLen)
                return mobilePhone;

            var sb = new StringBuilder();
            sb.Append('+');
            sb.Append(mobilePhone, 0, startLen);
            for (var i = startLen; i < mobilePhone.Length - endLen; i++)
            {
                sb.Append('*');
            }
            sb.Append(mobilePhone, mobilePhone.Length - endLen, mobilePhone.Length - (endLen + 1));
            return sb.ToString();
        }
    }
}