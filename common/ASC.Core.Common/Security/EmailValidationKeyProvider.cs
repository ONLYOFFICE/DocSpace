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


using System;
using System.Text;

using ASC.Common;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Utility;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using static ASC.Security.Cryptography.EmailValidationKeyProvider;

namespace ASC.Security.Cryptography
{
    public class EmailValidationKeyProvider
    {
        public enum ValidationResult
        {
            Ok,
            Invalid,
            Expired
        }

        private readonly ILog log;
        private static readonly DateTime _from = new DateTime(2010, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        internal readonly TimeSpan ValidInterval;

        private MachinePseudoKeys MachinePseudoKeys { get; }
        private TenantManager TenantManager { get; }

        public EmailValidationKeyProvider(MachinePseudoKeys machinePseudoKeys, TenantManager tenantManager, IConfiguration configuration, IOptionsMonitor<ILog> options)
        {
            MachinePseudoKeys = machinePseudoKeys;
            TenantManager = tenantManager;
            if (!TimeSpan.TryParse(configuration["email:validinterval"], out var validInterval))
            {
                validInterval = TimeSpan.FromDays(7);
            }

            ValidInterval = validInterval;
            log = options.CurrentValue;
        }

        public string GetEmailKey(string email)
        {
            return GetEmailKey(TenantManager.GetCurrentTenant().TenantId, email);
        }

        public string GetEmailKey(int tenantId, string email)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("email");

            email = FormatEmail(tenantId, email);

            var ms = (long)(DateTime.UtcNow - _from).TotalMilliseconds;
            var hash = GetMashineHashedData(BitConverter.GetBytes(ms), Encoding.ASCII.GetBytes(email));
            return string.Format("{0}.{1}", ms, DoStringFromBytes(hash));
        }

        private string FormatEmail(int tenantId, string email)
        {
            if (email == null) throw new ArgumentNullException("email");
            try
            {
                return string.Format("{0}|{1}|{2}", email.ToLowerInvariant(), tenantId, Encoding.UTF8.GetString(MachinePseudoKeys.GetMachineConstant()));
            }
            catch (Exception e)
            {
                log.Fatal("Failed to format tenant specific email", e);
                return email.ToLowerInvariant();
            }
        }

        public ValidationResult ValidateEmailKey(string email, string key)
        {
            return ValidateEmailKey(email, key, TimeSpan.MaxValue);
        }

        public ValidationResult ValidateEmailKey(string email, string key, TimeSpan validInterval)
        {
            var result = ValidateEmailKeyInternal(email, key, validInterval);
            log.DebugFormat("validation result: {0}, source: {1} with key: {2} interval: {3} tenant: {4}", result, email, key, validInterval, TenantManager.GetCurrentTenant().TenantId);
            return result;
        }


        private ValidationResult ValidateEmailKeyInternal(string email, string key, TimeSpan validInterval)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException("email");
            if (key == null) throw new ArgumentNullException("key");

            email = FormatEmail(TenantManager.GetCurrentTenant().TenantId, email);
            var parts = key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return ValidationResult.Invalid;

            if (!long.TryParse(parts[0], out var ms)) return ValidationResult.Invalid;

            var hash = GetMashineHashedData(BitConverter.GetBytes(ms), Encoding.ASCII.GetBytes(email));
            var key2 = DoStringFromBytes(hash);
            var key2_good = string.Compare(parts[1], key2, StringComparison.InvariantCultureIgnoreCase) == 0;
            if (!key2_good) return ValidationResult.Invalid;
            var ms_current = (long)(DateTime.UtcNow - _from).TotalMilliseconds;
            return validInterval >= TimeSpan.FromMilliseconds(ms_current - ms) ? ValidationResult.Ok : ValidationResult.Expired;
        }

        internal static string DoStringFromBytes(byte[] data)
        {
            var str = Convert.ToBase64String(data);
            str = str.Replace("=", "").Replace("+", "").Replace("/", "").Replace("\\", "");
            return str.ToUpperInvariant();
        }

        internal static byte[] GetMashineHashedData(byte[] salt, byte[] data)
        {
            var alldata = new byte[salt.Length + data.Length];
            Array.Copy(data, alldata, data.Length);
            Array.Copy(salt, 0, alldata, data.Length, salt.Length);
            return Hasher.Hash(alldata, HashAlg.SHA256);
        }
    }

    public class EmailValidationKeyModel
    {
        public string Key { get; set; }
        public EmployeeType? EmplType { get; set; }
        public string Email { get; set; }
        public Guid? UiD { get; set; }
        public ConfirmType? Type { get; set; }
        public int? P { get; set; }
        public EmailValidationKeyProvider Provider { get; }
        public AuthContext AuthContext { get; }
        public TenantManager TenantManager { get; }
        public UserManager UserManager { get; }
        public AuthManager Authentication { get; }

        public EmailValidationKeyModel(
            IHttpContextAccessor httpContextAccessor,
            EmailValidationKeyProvider provider,
            AuthContext authContext,
            TenantManager tenantManager,
            UserManager userManager,
            AuthManager authentication)
        {
            Provider = provider;
            AuthContext = authContext;
            TenantManager = tenantManager;
            UserManager = userManager;
            Authentication = authentication;

            var request = QueryHelpers.ParseQuery(httpContextAccessor.HttpContext.Request.Headers["confirm"]);

            _ = request.TryGetValue("type", out var type);

            ConfirmType? cType = null;
            if (Enum.TryParse<ConfirmType>(type, out var confirmType))
            {
                cType = confirmType;
            }

            _ = request.TryGetValue("key", out var key);

            _ = request.TryGetValue("p", out var pkey);
            _ = int.TryParse(pkey, out var p);

            _ = request.TryGetValue("emplType", out var emplType);
            _ = Enum.TryParse<EmployeeType>(emplType, out var employeeType);

            _ = request.TryGetValue("email", out var _email);
            _ = request.TryGetValue("uid", out var userIdKey);
            _ = Guid.TryParse(userIdKey, out var userId);

            Key = key;
            Type = cType;
            Email = _email;
            EmplType = employeeType;
            UiD = userId;
            P = p;
        }

        public ValidationResult Validate()
        {
            ValidationResult checkKeyResult;

            switch (Type)
            {
                case ConfirmType.EmpInvite:
                    checkKeyResult = Provider.ValidateEmailKey(Email + Type + (int)EmplType, Key, Provider.ValidInterval);
                    break;
                case ConfirmType.LinkInvite:
                    checkKeyResult = Provider.ValidateEmailKey(Type.ToString() + (int)EmplType, Key, Provider.ValidInterval);
                    break;
                case ConfirmType.PortalOwnerChange:
                    checkKeyResult = Provider.ValidateEmailKey(Email + Type + UiD.HasValue, Key, Provider.ValidInterval);
                    break;
                case ConfirmType.EmailChange:
                    checkKeyResult = Provider.ValidateEmailKey(Email + Type + AuthContext.CurrentAccount.ID, Key, Provider.ValidInterval);
                    break;
                case ConfirmType.PasswordChange:

                    var hash = Authentication.GetUserPasswordStamp(UserManager.GetUserByEmail(Email).ID).ToString("s");

                    checkKeyResult = Provider.ValidateEmailKey(Email + Type + hash, Key, Provider.ValidInterval);
                    break;
                case ConfirmType.Activation:
                    checkKeyResult = Provider.ValidateEmailKey(Email + Type + UiD, Key, Provider.ValidInterval);
                    break;
                case ConfirmType.ProfileRemove:
                    // validate UiD
                    if (P == 1)
                    {
                        var user = UserManager.GetUsers(UiD.GetValueOrDefault());
                        if (user == null || user.Status == EmployeeStatus.Terminated || AuthContext.IsAuthenticated && AuthContext.CurrentAccount.ID != UiD)
                            return ValidationResult.Invalid;
                    }

                    checkKeyResult = Provider.ValidateEmailKey(Email + Type + UiD, Key, Provider.ValidInterval);
                    break;
                case ConfirmType.Wizard:
                    checkKeyResult = Provider.ValidateEmailKey("" + Type, Key, Provider.ValidInterval);
                    break;
                default:
                    checkKeyResult = Provider.ValidateEmailKey(Email + Type, Key, Provider.ValidInterval);
                    break;
            }

            return checkKeyResult;
        }

        public void Deconstruct(out string key, out string email, out EmployeeType? employeeType, out Guid? userId, out ConfirmType? confirmType, out int? p)
            => (key, email, employeeType, userId, confirmType, p) = (Key, Email, EmplType, UiD, Type, P);
    }

    public static class EmailValidationKeyProviderExtension
    {
        public static DIHelper AddEmailValidationKeyProviderService(this DIHelper services)
        {
            services.TryAddTransient<EmailValidationKeyModel>();
            services.TryAddScoped<EmailValidationKeyProvider>();

            return services
                .AddTenantManagerService();
        }
    }
}