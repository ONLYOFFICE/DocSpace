// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode

using Constants = ASC.Core.Users.Constants;
using System.Threading.Tasks;

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
                TenantManager.SetTenantQuotaRow(new TenantQuotaRow { Tenant = TenantManager.GetCurrentTenant().Id, Path = "/sms", Counter = 1 }, true);
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
                SecurityContext.AuthenticateMe(user.Id);
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