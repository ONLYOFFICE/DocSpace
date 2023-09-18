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

namespace ASC.Web.Studio.Core.TFA;

public class TfaAppUserSettings : ISettings<TfaAppUserSettings>
{
    [JsonPropertyName("BackupCodes")]
    public IEnumerable<BackupCode> CodesSetting { get; set; }

    [JsonPropertyName("Salt")]
    public long SaltSetting { get; set; }

    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{EAF10611-BE1E-4634-B7A1-57F913042F78}"); }
    }

    public TfaAppUserSettings GetDefault()
    {
        return new TfaAppUserSettings
        {
            CodesSetting = new List<BackupCode>(),
            SaltSetting = 0
        };
    }

    public static async Task<long> GetSaltAsync(SettingsManager settingsManager, Guid userId)
    {
        var settings = await settingsManager.LoadAsync<TfaAppUserSettings>(userId);
        var salt = settings.SaltSetting;
        if (salt == 0)
        {
            var from = new DateTime(2018, 07, 07, 0, 0, 0, DateTimeKind.Utc);
            settings.SaltSetting = salt = (long)(DateTime.UtcNow - from).TotalMilliseconds;

            await settingsManager.SaveAsync(settings, userId);
        }
        return salt;
    }

    public static async Task<IEnumerable<BackupCode>> BackupCodesForUserAsync(SettingsManager settingsManager, Guid userId)
    {
        return (await settingsManager.LoadAsync<TfaAppUserSettings>(userId)).CodesSetting;
    }

    public static async Task DisableCodeForUserAsync(SettingsManager settingsManager, InstanceCrypto instanceCrypto, Signature signature, Guid userId, string code)
    {
        var settings = await settingsManager.LoadAsync<TfaAppUserSettings>(userId);
        var query = settings.CodesSetting.Where(x => x.GetEncryptedCode(instanceCrypto, signature) == code).ToList();

        if (query.Count > 0)
        {
            query.First().IsUsed = true;
        }

        await settingsManager.SaveAsync(settings, userId);
    }

    public static async Task<bool> EnableForUserAsync(SettingsManager settingsManager, Guid guid)
    {
        return (await settingsManager.LoadAsync<TfaAppUserSettings>(guid)).CodesSetting.Any();
    }

    public static async Task DisableForUserAsync(SettingsManager settingsManager, Guid guid)
    {
        var defaultSettings = settingsManager.GetDefault<TfaAppUserSettings>();
        await settingsManager.SaveAsync(defaultSettings, guid);
    }
}
