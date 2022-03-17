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
            {
                return string.Empty;
            }

            mobilePhone = GetPhoneValueDigits(mobilePhone);

            const int startLen = 4;
            const int endLen = 4;
            if (mobilePhone.Length < startLen + endLen)
            {
                return mobilePhone;
            }

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