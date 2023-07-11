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

namespace ASC.Web.Api.Core;

public class CspSettings : ISettings<CspSettings>
{
    [JsonIgnore]
    public Guid ID => new Guid("27504162-16FF-405F-8530-1537B0F2B89D");

    public IEnumerable<string> Domains { get; set; }

    public CspSettings GetDefault()
    {
        return new CspSettings();
    }
}

[Scope]
public class CspSettingsHelper
{
    private readonly SettingsManager _settingsManager;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly TenantManager _tenantManager;
    private readonly CoreSettings _coreSettings;
    private readonly IDistributedCache _distributedCache;

    public CspSettingsHelper(
        SettingsManager settingsManager,
        FilesLinkUtility filesLinkUtility,
        TenantManager tenantManager,
        CoreSettings coreSettings,
        IDistributedCache distributedCache)
    {
        _settingsManager = settingsManager;
        _filesLinkUtility = filesLinkUtility;
        _tenantManager = tenantManager;
        _coreSettings = coreSettings;
        _distributedCache = distributedCache;
    }

    public async Task<string> Save(IEnumerable<string> domains)
    {
        var headerKey = GetKey(_tenantManager.GetCurrentTenant().GetTenantDomain(_coreSettings));
        var headerValue = "";

        if (domains != null && domains.Any())
        {
            var csp = new CspBuilder();

            csp.ByDefaultAllow
                .FromSelf()
                .From(_filesLinkUtility.DocServiceUrl);

            var scriptBuilder = csp.AllowScripts
                .FromSelf()
                .From(_filesLinkUtility.DocServiceUrl)
                .AllowUnsafeInline();

            var styleBuilder = csp.AllowStyles
                .FromSelf()
                .AllowUnsafeInline();

            var imageBuilder = csp.AllowImages
                .FromSelf();

            var frameBuilder = csp.AllowFraming;

            foreach (var domain in domains)
            {
                scriptBuilder.From(domain);
                styleBuilder.From(domain);
                imageBuilder.From(domain);
                frameBuilder.From(domain);
            }

            (_, headerValue) = csp.BuildCspOptions().ToString(null);

            await _distributedCache.SetStringAsync(headerKey, headerValue);
        }
        else
        {
            await _distributedCache.RemoveAsync(headerKey);
        }

        var current = _settingsManager.Load<CspSettings>();
        current.Domains = domains;
        _settingsManager.Save(current);

        return headerValue;
    }

    public CspSettings Load()
    {
        return _settingsManager.Load<CspSettings>();
    }

    public async Task RenameDomain(string oldDomain, string newDomain)
    {
        var oldKey = GetKey(oldDomain);
        var val = await _distributedCache.GetStringAsync(oldKey);
        if (!string.IsNullOrEmpty(val))
        {
            await _distributedCache.RemoveAsync(oldKey);
            await _distributedCache.SetStringAsync(GetKey(newDomain), val);
        }
    }

    private string GetKey(string domain)
    {
        return $"csp:{domain}";
    }
}
