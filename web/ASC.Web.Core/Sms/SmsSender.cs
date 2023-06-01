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

namespace ASC.Web.Core.Sms;

[Scope]
public class SmsSender
{
    private readonly IConfiguration _configuration;
    private readonly TenantManager _tenantManager;
    private readonly SmsProviderManager _smsProviderManager;
    private readonly ILogger<SmsSender> _log;

    public SmsSender(
        IConfiguration configuration,
        TenantManager tenantManager,
        ILogger<SmsSender> logger,
        SmsProviderManager smsProviderManager)
    {
        _configuration = configuration;
        _tenantManager = tenantManager;
        _smsProviderManager = smsProviderManager;
        _log = logger;
    }

    public async Task<bool> SendSMSAsync(string number, string message)
    {
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(number);
        ArgumentNullOrEmptyException.ThrowIfNullOrEmpty(message);

        if (!_smsProviderManager.Enabled())
        {
            throw new MethodAccessException();
        }

        if ("log".Equals(_configuration["core:notify:postman"], StringComparison.InvariantCultureIgnoreCase))
        {
            var tenant = await _tenantManager.GetCurrentTenantAsync(false);
            var tenantId = tenant == null ? Tenant.DefaultTenant : tenant.Id;

            _log.InformationSendSmsToPhoneNumber(tenantId, number, message);
            return false;
        }

        number = new Regex("[^\\d+]").Replace(number, string.Empty);
        return await _smsProviderManager.SendMessageAsync(number, message);
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
