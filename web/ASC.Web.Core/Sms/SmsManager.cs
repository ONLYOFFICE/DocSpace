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