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

namespace ASC.Security.Cryptography;

[Scope]
public class EmailValidationKeyProvider
{
    public enum ValidationResult
    {
        Ok,
        Invalid,
        Expired
    }

    public TimeSpan ValidEmailKeyInterval { get; }
    public TimeSpan ValidAuthKeyInterval { get; }
    public TimeSpan ValidVisitLinkInterval { get; }

    private readonly ILogger<EmailValidationKeyProvider> _logger;
    private static readonly DateTime _from = new DateTime(2010, 01, 01, 0, 0, 0, DateTimeKind.Utc);
    private readonly MachinePseudoKeys _machinePseudoKeys;
    private readonly TenantManager _tenantManager;

    public EmailValidationKeyProvider(MachinePseudoKeys machinePseudoKeys, TenantManager tenantManager, IConfiguration configuration, ILogger<EmailValidationKeyProvider> logger)
    {
        _machinePseudoKeys = machinePseudoKeys;
        _tenantManager = tenantManager;
        if (!TimeSpan.TryParse(configuration["email:validinterval"], out var validInterval))
        {
            validInterval = TimeSpan.FromDays(7);
        }
        if (!TimeSpan.TryParse(configuration["auth:validinterval"], out var authValidInterval))
        {
            authValidInterval = TimeSpan.FromHours(1);
        }
        if (!TimeSpan.TryParse(configuration["visit:validinterval"], out var validVisitLinkInterval))
        {
            validVisitLinkInterval = TimeSpan.FromMinutes(15);
        }
       
        ValidEmailKeyInterval = validInterval;
        ValidAuthKeyInterval = authValidInterval;
        ValidVisitLinkInterval = validVisitLinkInterval;
        _logger = logger;
    }

    public async Task<string> GetEmailKeyAsync(string email)
    {
        return GetEmailKey(await _tenantManager.GetCurrentTenantIdAsync(), email);
    }

    public string GetEmailKey(string email)
    {
        return GetEmailKey(_tenantManager.GetCurrentTenant().Id, email);
    }

    public string GetEmailKey(int tenantId, string email)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(email);

        email = FormatEmail(tenantId, email);

        var ms = (long)(DateTime.UtcNow - _from).TotalMilliseconds;
        var hash = GetMashineHashedData(BitConverter.GetBytes(ms), Encoding.ASCII.GetBytes(email));

        return string.Format("{0}.{1}", ms, DoStringFromBytes(hash));
    }

    private string FormatEmail(int tenantId, string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        try
        {
            return string.Format("{0}|{1}|{2}", email.ToLowerInvariant(), tenantId, Encoding.UTF8.GetString(_machinePseudoKeys.GetMachineConstant()));
        }
        catch (Exception e)
        {
            _logger.CriticalFormatEmail(e);

            return email.ToLowerInvariant();
        }
    }

    public async Task<ValidationResult> ValidateEmailKeyAsync(string email, string key)
    {
        return await ValidateEmailKeyAsync(email, key, TimeSpan.MaxValue);
    }

    public async Task<ValidationResult> ValidateEmailKeyAsync(string email, string key, TimeSpan validInterval)
    {
        var result = await ValidateEmailKeyInternalAsync(email, key, validInterval);
        _logger.DebugValidationResult(result, email, key, validInterval, await _tenantManager.GetCurrentTenantIdAsync());

        return result;
    }

    private async Task<ValidationResult> ValidateEmailKeyInternalAsync(string email, string key, TimeSpan validInterval)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(email);
        ArgumentNullException.ThrowIfNull(key);

        email = FormatEmail(await _tenantManager.GetCurrentTenantIdAsync(), email);
        var parts = key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
        {
            return ValidationResult.Invalid;
        }

        if (!long.TryParse(parts[0], out var ms))
        {
            return ValidationResult.Invalid;
        }

        var hash = GetMashineHashedData(BitConverter.GetBytes(ms), Encoding.ASCII.GetBytes(email));
        var key2 = DoStringFromBytes(hash);
        var key2_good = string.Equals(parts[1], key2, StringComparison.OrdinalIgnoreCase);
        if (!key2_good)
        {
            return ValidationResult.Invalid;
        }

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

/// <summary>
/// </summary>
public class EmailValidationKeyModel
{
    /// <summary>Key</summary>
    /// <type>System.String, System</type>
    public string Key { get; set; }

    /// <summary>Employee type</summary>
    /// <type>System.Nullabel{ASC.Core.Users.EmployeeType}, System</type>
    public EmployeeType? EmplType { get; set; }

    /// <summary>Email</summary>
    /// <type>System.String, System</type>
    public string Email { get; set; }

    /// <summary>User ID</summary>
    /// <type>System.Nullabel{System.Guid}, System</type>
    public Guid? UiD { get; set; }

    /// <summary>Confirmation email type</summary>
    /// <type>System.Nullabel{ASC.Web.Studio.Utility.ConfirmType}, System</type>
    public ConfirmType? Type { get; set; }

    public void Deconstruct(out string key, out EmployeeType? emplType, out string email, out Guid? uiD, out ConfirmType? type)
    {
        (key, emplType, email, uiD, type) = (Key, EmplType, Email, UiD, Type);
    }
}