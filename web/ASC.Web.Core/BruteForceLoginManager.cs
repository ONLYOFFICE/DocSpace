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

namespace ASC.Web.Core;

[Scope]
public class BruteForceLoginManager
{
    private readonly SettingsManager _settingsManager;
    private readonly UserManager _userManager;
    private readonly TenantManager _tenantManager;
    private readonly IDistributedCache _distributedCache;
    private readonly SetupInfo _setupInfo;
    private readonly Recaptcha _recaptcha;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

    public BruteForceLoginManager(SettingsManager settingsManager,
        UserManager userManager,
        TenantManager tenantManager,
        IDistributedCache distributedCache,
        SetupInfo setupInfo,
        Recaptcha recaptcha)
    {
        _settingsManager = settingsManager;
        _userManager = userManager;
        _tenantManager = tenantManager;
        _distributedCache = distributedCache;
        _setupInfo = setupInfo;
        _recaptcha = recaptcha;
    }

    public async Task<(bool, bool)> IncrementAsync(string key, string requestIp, bool throwException, string exceptionMessage = null)
    {
        var blockCacheKey = GetBlockCacheKey(key, requestIp);
        
        if (GetFromCache<string>(blockCacheKey) != null)
        {
            if (throwException)
            {
                throw new BruteForceCredentialException(exceptionMessage);
            }

            return (false, true);
        }

        try
        {
            await _semaphore.WaitAsync();
            
            if (GetFromCache<string>(blockCacheKey) != null)
            {
                throw new BruteForceCredentialException(exceptionMessage);
            }

            var historyCacheKey = GetHistoryCacheKey(key, requestIp);
            var settings = new LoginSettingsWrapper(_settingsManager.Load<LoginSettings>());
            var history = GetFromCache<List<DateTime>>(historyCacheKey) ?? new List<DateTime>();

            var now = DateTime.UtcNow;
            var checkTime = now.Subtract(settings.CheckPeriod);

            history = history.Where(item => item > checkTime).ToList();
            history.Add(now);

            var showRecaptcha = history.Count > settings.AttemptCount - 1;

            if (history.Count > settings.AttemptCount)
            {
                SetToCache(blockCacheKey, "block", now.Add(settings.BlockTime));
                await _distributedCache.RemoveAsync(historyCacheKey);

                if (throwException)
                {
                    throw new BruteForceCredentialException(exceptionMessage);
                }

                return (false, showRecaptcha);
            }

            SetToCache(historyCacheKey, history, now.Add(settings.CheckPeriod));

            return (true, showRecaptcha);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task DecrementAsync(string key, string requestIp)
    {
        try
        {
            await _semaphore.WaitAsync();

            var settings = new LoginSettingsWrapper(_settingsManager.Load<LoginSettings>());
            var historyCacheKey = GetHistoryCacheKey(key, requestIp);
            var history = GetFromCache<List<DateTime>>(historyCacheKey) ?? new List<DateTime>();

            if (history.Count > 0)
            {
                history.RemoveAt(history.Count - 1);
            }

            SetToCache(historyCacheKey, history, DateTime.UtcNow.Add(settings.CheckPeriod));
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<UserInfo> AttemptAsync(string login, string passwordHash, string requestIp, string recaptchaResponse)
    {
        UserInfo user = null;

        var secretEmail = SetupInfo.IsSecretEmail(login);

        var recaptchaPassed = secretEmail || await CheckRecaptchaAsync(recaptchaResponse, requestIp);

        var blockCacheKey = GetBlockCacheKey(login, requestIp);

        if (!recaptchaPassed && GetFromCache<string>(blockCacheKey) != null)
        {
            throw new BruteForceCredentialException();
        }

        try
        {
            await _semaphore.WaitAsync();
            if (!recaptchaPassed && GetFromCache<string>(blockCacheKey) != null)
            {
                throw new BruteForceCredentialException();
            }

            string historyCacheKey = null;
            var now = DateTime.UtcNow;
            LoginSettingsWrapper settings = null;
            List<DateTime> history = null;

            if (!recaptchaPassed)
            {
                historyCacheKey = GetHistoryCacheKey(login, requestIp);

                settings = new LoginSettingsWrapper(await _settingsManager.LoadAsync<LoginSettings>());
                var checkTime = now.Subtract(settings.CheckPeriod);

                history = GetFromCache<List<DateTime>>(historyCacheKey) ?? new List<DateTime>();
                history = history.Where(item => item > checkTime).ToList();
                history.Add(now);

                if (history.Count > settings.AttemptCount)
                {
                    SetToCache(blockCacheKey, "block", now.Add(settings.BlockTime));
                    _distributedCache.Remove(historyCacheKey);
                    throw new BruteForceCredentialException();
                }

                SetToCache(historyCacheKey, history, now.Add(settings.CheckPeriod));
            }

            user = await _userManager.GetUsersByPasswordHashAsync(
                   await _tenantManager.GetCurrentTenantIdAsync(),
                   login,
                   passwordHash);

            if (user == null || !_userManager.UserExists(user))
            {
                throw new Exception("user not found");
            }

            if (!recaptchaPassed)
            {
                history.RemoveAt(history.Count - 1);

                SetToCache(historyCacheKey, history, now.Add(settings.CheckPeriod));
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            _semaphore.Release();
        }

        return user;
    }

    private async Task<bool> CheckRecaptchaAsync(string recaptchaResponse, string requestIp)
    {
        var recaptchaPassed = false;

        if (!string.IsNullOrEmpty(_setupInfo.RecaptchaPublicKey) &&
            !string.IsNullOrEmpty(_setupInfo.RecaptchaPrivateKey) &&
            !string.IsNullOrEmpty(recaptchaResponse))
        {
            recaptchaPassed = await _recaptcha.ValidateRecaptchaAsync(recaptchaResponse, requestIp);

            if (!recaptchaPassed)
            {
                throw new RecaptchaException();
            }
        }

        return recaptchaPassed;
    }

    private T GetFromCache<T>(string key)
    {
        var serializedObject = _distributedCache.Get(key);

        if (serializedObject == null)
        {
            return default;
        }

        using var ms = new MemoryStream(serializedObject);

        return Serializer.Deserialize<T>(ms);
    }

    private void SetToCache<T>(string key, T value, DateTime ExpirationPeriod)
    {
        using var ms = new MemoryStream();

        Serializer.Serialize(ms, value);

        _distributedCache.Set(key, ms.ToArray(), new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = ExpirationPeriod
        });
    }

    private static string GetBlockCacheKey(string login, string requestIp) => $"loginblock/{login}/{requestIp}";

    private static string GetHistoryCacheKey(string login, string requestIp) => $"loginsec/{login}/{requestIp}";
}
