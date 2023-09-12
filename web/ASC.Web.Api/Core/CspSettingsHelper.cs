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

using ASC.Data.Storage.S3;
using ASC.Web.Files.Classes;

namespace ASC.Web.Api.Core;

[Scope]
public class CspSettingsHelper
{
    private readonly SettingsManager _settingsManager;
    private readonly FilesLinkUtility _filesLinkUtility;
    private readonly TenantManager _tenantManager;
    private readonly CoreSettings _coreSettings;
    private readonly GlobalStore _globalStore;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IDistributedCache _distributedCache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public CspSettingsHelper(
        SettingsManager settingsManager,
        FilesLinkUtility filesLinkUtility,
        TenantManager tenantManager,
        CoreSettings coreSettings,
        GlobalStore globalStore,
        CoreBaseSettings coreBaseSettings,
        IDistributedCache distributedCache,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _settingsManager = settingsManager;
        _filesLinkUtility = filesLinkUtility;
        _tenantManager = tenantManager;
        _coreSettings = coreSettings;
        _globalStore = globalStore;
        _coreBaseSettings = coreBaseSettings;
        _distributedCache = distributedCache;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<string> Save(IEnumerable<string> domains, bool setDefaultIfEmpty)
    {
        var tenant = _tenantManager.GetCurrentTenant();
        var domain = tenant.GetTenantDomain(_coreSettings);
        List<string> headerKeys = new()
        {
            GetKey(domain)
        };

        if (domain == Tenant.LocalHost && tenant.Alias == Tenant.LocalHost)
        {
            headerKeys.Add(GetKey(Tenant.HostName));
            var ips = Dns.GetHostAddresses(Dns.GetHostName(), AddressFamily.InterNetwork);

            foreach (var ip in ips)
            {
                headerKeys.Add(GetKey(ip.ToString()));
            }

            headerKeys.Add(GetKey(_httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString()));
        }

        var headerValue = await CreateHeaderAsync(domains, setDefaultIfEmpty);

        if (!string.IsNullOrEmpty(headerValue))
        {
            await Parallel.ForEachAsync(headerKeys, async (headerKey, _) => await _distributedCache.SetStringAsync(headerKey, headerValue));
        }
        else
        {
            await Parallel.ForEachAsync(headerKeys, async (headerKey, _) => await _distributedCache.RemoveAsync(headerKey));
        }

        var current = _settingsManager.Load<CspSettings>();
        current.Domains = domains;
        current.SetDefaultIfEmpty = setDefaultIfEmpty;
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

    public async Task<string> CreateHeaderAsync(IEnumerable<string> domains, bool setDefaultIfEmpty = false)
    {
        if (domains == null || !domains.Any())
        {
            if (setDefaultIfEmpty)
            {
                domains = Enumerable.Empty<string>();
            }
            else
            {
                return null;
            }
        }

        var csp = new CspBuilder();

        var def = csp.ByDefaultAllow
            .FromSelf()
            .From("data:")
            .From(_filesLinkUtility.DocServiceUrl);

        var scriptBuilder = csp.AllowScripts
            .FromSelf()
            .From(_filesLinkUtility.DocServiceUrl)
            .AllowUnsafeInline();

        var firebaseDomain = _configuration["firebase:authDomain"];
        if (!string.IsNullOrEmpty(firebaseDomain))
        {
            def.From(firebaseDomain);

            var googleapi = "*.googleapis.com";
            def.From(googleapi);
            scriptBuilder.From(googleapi);
        }

        var styleBuilder = csp.AllowStyles
            .FromSelf()
            .AllowUnsafeInline();

        var imageBuilder = csp.AllowImages
            .FromSelf()
            .From("data:")
            .From("blob:");

        var frameBuilder = csp.AllowFraming
            .FromSelf();

        if (!_coreBaseSettings.Standalone && !string.IsNullOrEmpty(_coreBaseSettings.Basedomain))
        {
            def.From($"*.{_coreBaseSettings.Basedomain}");
        }

        if (!string.IsNullOrEmpty(_configuration["web:zendesk-key"]))
        {
            def.From("*.zdassets.com");
            scriptBuilder.From("*.zdassets.com");

            def.From("*.zendesk.com");
            def.From("*.zopim.com");
            def.From("wss:");

            scriptBuilder
                .From("*.zopim.com")
                .AllowUnsafeEval();//zendesk;

            imageBuilder.From("*.zopim.io");
        }

        foreach (var domain in domains)
        {
            scriptBuilder.From(domain);
            styleBuilder.From(domain);
            imageBuilder.From(domain);
            frameBuilder.From(domain);
        }

        if (await _globalStore.GetStoreAsync() is S3Storage s3Storage && !string.IsNullOrEmpty(s3Storage.CdnDistributionDomain))
        {
            imageBuilder.From(s3Storage.CdnDistributionDomain);
        }

        var (_, headerValue) = csp.BuildCspOptions().ToString(null);
        return headerValue;
    }

    private string GetKey(string domain)
    {
        return $"csp:{domain}";
    }
}
