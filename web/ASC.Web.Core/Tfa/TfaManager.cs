using Constants = ASC.Core.Users.Constants;

namespace ASC.Web.Studio.Core.TFA
{
    [Serializable]
    public class BackupCode
    {
        public bool IsUsed { get; set; }

        public string Code { get; set; }

        public string GetEncryptedCode(InstanceCrypto InstanceCrypto, Signature Signature)
        {
                try
                {
                return InstanceCrypto.Decrypt(Code);
                }
                catch
                {
                    //support old scheme stored in the DB
                return Signature.Read<string>(Code);
                }
            }

        public void SetEncryptedCode(InstanceCrypto InstanceCrypto, string code)
        {
            Code = InstanceCrypto.Encrypt(code);
        }
    }

    [Scope]
    public class TfaManager
    {
        private static readonly TwoFactorAuthenticator Tfa = new TwoFactorAuthenticator();
        private ICache Cache { get; set; }

        private SettingsManager SettingsManager { get; }
        private SecurityContext SecurityContext { get; }
        private CookiesManager CookiesManager { get; }
        private SetupInfo SetupInfo { get; }
        private Signature Signature { get; }
        private InstanceCrypto InstanceCrypto { get; }
        public MachinePseudoKeys MachinePseudoKeys { get; }

        public TfaManager(
            SettingsManager settingsManager,
            SecurityContext securityContext,
            CookiesManager cookiesManager,
            SetupInfo setupInfo,
            Signature signature,
            InstanceCrypto instanceCrypto,
            MachinePseudoKeys machinePseudoKeys,
            ICache cache)
        {
            Cache = cache;
            SettingsManager = settingsManager;
            SecurityContext = securityContext;
            CookiesManager = cookiesManager;
            SetupInfo = setupInfo;
            Signature = signature;
            InstanceCrypto = instanceCrypto;
            MachinePseudoKeys = machinePseudoKeys;
        }

        public SetupCode GenerateSetupCode(UserInfo user)
        {
            return Tfa.GenerateSetupCode(SetupInfo.TfaAppSender, user.Email, GenerateAccessToken(user), false, 4);
        }

        public bool ValidateAuthCode(UserInfo user, string code, bool checkBackup = true)
        {
            if (!TfaAppAuthSettings.IsVisibleSettings
                || !SettingsManager.Load<TfaAppAuthSettings>().EnableSetting)
            {
                return false;
            }

            if (user == null || Equals(user, Constants.LostUser)) throw new Exception(Resource.ErrorUserNotFound);

            code = (code ?? "").Trim();

            if (string.IsNullOrEmpty(code)) throw new Exception(Resource.ActivateTfaAppEmptyCode);

            int.TryParse(Cache.Get<string>("tfa/" + user.Id), out var counter);
            if (++counter > SetupInfo.LoginThreshold)
            {
                throw new BruteForceCredentialException(Resource.TfaTooMuchError);
            }
            Cache.Insert("tfa/" + user.Id, counter.ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)));

            if (!Tfa.ValidateTwoFactorPIN(GenerateAccessToken(user), code))
            {
                if (checkBackup && TfaAppUserSettings.BackupCodesForUser(SettingsManager, user.Id).Any(x => x.GetEncryptedCode(InstanceCrypto, Signature) == code && !x.IsUsed))
                {
                    TfaAppUserSettings.DisableCodeForUser(SettingsManager, InstanceCrypto, Signature, user.Id, code);
                }
                else
                {
                    throw new ArgumentException(Resource.TfaAppAuthMessageError);
                }
            }

            Cache.Insert("tfa/" + user.Id, (counter - 1).ToString(CultureInfo.InvariantCulture), DateTime.UtcNow.Add(TimeSpan.FromMinutes(1)));

            if (!SecurityContext.IsAuthenticated)
            {
                var cookiesKey = SecurityContext.AuthenticateMe(user.Id);
                CookiesManager.SetCookies(CookiesType.AuthKey, cookiesKey);
            }

            if (!TfaAppUserSettings.EnableForUser(SettingsManager, user.Id))
            {
                GenerateBackupCodes();
                return true;
            }

            return false;
        }

        public IEnumerable<BackupCode> GenerateBackupCodes()
        {
            var count = SetupInfo.TfaAppBackupCodeCount;
            var length = SetupInfo.TfaAppBackupCodeLength;

            const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_";

            byte[] data;

            var list = new List<BackupCode>();

            for (var i = 0; i < count; i++)
            {
                data = RandomNumberGenerator.GetBytes(length);

                var result = new StringBuilder(length);
                foreach (var b in data)
                {
                    result.Append(alphabet[b % alphabet.Length]);
                }

                    var code = new BackupCode();
                    code.SetEncryptedCode(InstanceCrypto, result.ToString());
                    list.Add(code);
            }
            var settings = SettingsManager.LoadForCurrentUser<TfaAppUserSettings>();
            settings.CodesSetting = list;
            SettingsManager.SaveForCurrentUser(settings);

            return list;
        }

        private string GenerateAccessToken(UserInfo user)
        {
            var userSalt = TfaAppUserSettings.GetSalt(SettingsManager, user.Id);

            //from Signature.Create
            var machineSalt = Encoding.UTF8.GetString(MachinePseudoKeys.GetMachineConstant());
            var token = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(userSalt + machineSalt)));
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            return encodedToken.Substring(0, 10);
        }
    }
}