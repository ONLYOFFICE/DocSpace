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
using System.Threading.Tasks;

using ASC.Common;
using ASC.Core;
using ASC.Core.Common.Security;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Core.PublicResources;
using ASC.Web.Core.Sms;

namespace ASC.Web.Studio.Core.SMS
{
    [Scope]
    public class SmsManager
    {
        private UserManager UserManager { get; }
        private SecurityContext SecurityContext { get; }
        private TenantManager TenantManager { get; }
        private SmsKeyStorage SmsKeyStorage { get; }
        private SmsSender SmsSender { get; }
        private StudioSmsNotificationSettingsHelper StudioSmsNotificationSettingsHelper { get; }

        public SmsManager(
            UserManager userManager,
            SecurityContext securityContext,
            TenantManager tenantManager,
            SmsKeyStorage smsKeyStorage,
            SmsSender smsSender,
            StudioSmsNotificationSettingsHelper studioSmsNotificationSettingsHelper)
        {
            UserManager = userManager;
            SecurityContext = securityContext;
            TenantManager = tenantManager;
            SmsKeyStorage = smsKeyStorage;
            SmsSender = smsSender;
            StudioSmsNotificationSettingsHelper = studioSmsNotificationSettingsHelper;
        }

        public Task<string> SaveMobilePhoneAsync(UserInfo user, string mobilePhone)
        {
            mobilePhone = SmsSender.GetPhoneValueDigits(mobilePhone);

            if (user == null || Equals(user, Constants.LostUser)) throw new Exception(Resource.ErrorUserNotFound);
            if (string.IsNullOrEmpty(mobilePhone)) throw new Exception(Resource.ActivateMobilePhoneEmptyPhoneNumber);
            if (!string.IsNullOrEmpty(user.MobilePhone) && user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.Activated) throw new Exception(Resource.MobilePhoneMustErase);

            return InternalSaveMobilePhoneAsync(user, mobilePhone);
        }

        private async Task<string> InternalSaveMobilePhoneAsync(UserInfo user, string mobilePhone)
        {
            user.MobilePhone = mobilePhone;
            user.MobilePhoneActivationStatus = MobilePhoneActivationStatus.NotActivated;
            if (SecurityContext.IsAuthenticated)
            {
                UserManager.SaveUserInfo(user);
            }
            else
            {
                try
                {
                    SecurityContext.AuthenticateMeWithoutCookie(ASC.Core.Configuration.Constants.CoreSystem);
                    UserManager.SaveUserInfo(user);
                }
                finally
                {
                    SecurityContext.Logout();
                }
            }

            if (StudioSmsNotificationSettingsHelper.Enable)
            {
                await PutAuthCodeAsync(user, false);
            }

            return mobilePhone;
        }

        public Task PutAuthCodeAsync(UserInfo user, bool again)
        {
            if (user == null || Equals(user, Constants.LostUser)) throw new Exception(Resource.ErrorUserNotFound);

            if (!StudioSmsNotificationSettingsHelper.IsVisibleSettings() || !StudioSmsNotificationSettingsHelper.Enable) throw new MethodAccessException();

            var mobilePhone = SmsSender.GetPhoneValueDigits(user.MobilePhone);

            if (SmsKeyStorage.ExistsKey(mobilePhone) && !again) return Task.CompletedTask;

            if (!SmsKeyStorage.GenerateKey(mobilePhone, out var key)) throw new Exception(Resource.SmsTooMuchError);
            return InternalPutAuthCodeAsync(mobilePhone, key);
        }

        private async Task InternalPutAuthCodeAsync(string mobilePhone, string key)
        {
            if (await SmsSender.SendSMSAsync(mobilePhone, string.Format(Resource.SmsAuthenticationMessageToUser, key)))
            {
                TenantManager.SetTenantQuotaRow(new TenantQuotaRow { Tenant = TenantManager.GetCurrentTenant().TenantId, Path = "/sms", Counter = 1 }, true);
            }
        }

        public void ValidateSmsCode(UserInfo user, string code)
        {
            if (!StudioSmsNotificationSettingsHelper.IsVisibleSettings()
                || !StudioSmsNotificationSettingsHelper.Enable)
            {
                return;
            }

            if (user == null || Equals(user, Constants.LostUser)) throw new Exception(Resource.ErrorUserNotFound);

            var valid = SmsKeyStorage.ValidateKey(user.MobilePhone, code);
            switch (valid)
            {
                case SmsKeyStorage.Result.Empty:
                    throw new Exception(Resource.ActivateMobilePhoneEmptyCode);
                case SmsKeyStorage.Result.TooMuch:
                    throw new BruteForceCredentialException(Resource.SmsTooMuchError);
                case SmsKeyStorage.Result.Timeout:
                    throw new TimeoutException(Resource.SmsAuthenticationTimeout);
                case SmsKeyStorage.Result.Invalide:
                    throw new ArgumentException(Resource.SmsAuthenticationMessageError);
            }
            if (valid != SmsKeyStorage.Result.Ok) throw new Exception("Error: " + valid);

            if (!SecurityContext.IsAuthenticated)
            {
                SecurityContext.AuthenticateMe(user.ID);
                //CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
            }

            if (user.MobilePhoneActivationStatus == MobilePhoneActivationStatus.NotActivated)
            {
                user.MobilePhoneActivationStatus = MobilePhoneActivationStatus.Activated;
                UserManager.SaveUserInfo(user);
            }
        }
    }
}