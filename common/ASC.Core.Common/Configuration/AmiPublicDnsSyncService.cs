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



namespace ASC.Core.Configuration;

[Scope]
public class AmiPublicDnsSyncService : BackgroundService
{
    private readonly TenantManager _tenantManager;
    private readonly CoreBaseSettings _coreBaseSettings;
    private readonly IHttpClientFactory _clientFactory;

    public AmiPublicDnsSyncService(TenantManager tenantManager, CoreBaseSettings coreBaseSettings, IHttpClientFactory clientFactory)
    {
        _tenantManager = tenantManager;
        _coreBaseSettings = coreBaseSettings;
        _clientFactory = clientFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Synchronize();
        return Task.CompletedTask;
    }

    private void Synchronize()
    {
        if (_coreBaseSettings.Standalone)
        {
            var tenants = _tenantManager.GetTenants(false).Where(t => MappedDomainNotSettedByUser(t.MappedDomain));
            if (tenants.Any())
            {
                var dnsname = GetAmiPublicDnsName(_clientFactory);
                foreach (var tenant in tenants.Where(t => !string.IsNullOrEmpty(dnsname) && t.MappedDomain != dnsname))
                {
                    tenant.MappedDomain = dnsname;
                    _tenantManager.SaveTenant(tenant);
                }
            }
        }
    }

    private static bool MappedDomainNotSettedByUser(string domain)
    {
        return string.IsNullOrEmpty(domain) || Regex.IsMatch(domain, "^ec2.+\\.compute\\.amazonaws\\.com$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    }

    private static string GetAmiPublicDnsName(IHttpClientFactory clientFactory)
    {
        try
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://169.254.169.254/latest/meta-data/public-hostname"),
                Method = HttpMethod.Get
            };

            var httpClient = clientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromMilliseconds(5000);

            using var responce = httpClient.Send(request);
            using var stream = responce.Content.ReadAsStream();
            using var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == HttpStatusCode.NotFound || ex.StatusCode == HttpStatusCode.Conflict)
            {
                throw;
            }
        }

        return null;
    }
}
