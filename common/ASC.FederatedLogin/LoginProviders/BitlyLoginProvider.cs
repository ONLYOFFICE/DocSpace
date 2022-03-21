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

namespace ASC.FederatedLogin.LoginProviders;

[Scope]
public class BitlyLoginProvider : Consumer, IValidateKeysProvider
{
    private string BitlyClientId => this["bitlyClientId"];
    private string BitlyClientSecret => this["bitlyClientSecret"];
    private string BitlyUrl => this["bitlyUrl"];

    public BitlyLoginProvider() { }

    public BitlyLoginProvider(
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory,
        string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
        : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, props, additional)
    {
    }

    public bool ValidateKeys()
    {
        try
        {
            return !string.IsNullOrEmpty(GetShortenLink("https://www.onlyoffice.com"));
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool Enabled
    {
        get
        {
            return !string.IsNullOrEmpty(BitlyClientId) &&
                   !string.IsNullOrEmpty(BitlyClientSecret) &&
                   !string.IsNullOrEmpty(BitlyUrl);
        }
    }

    public string GetShortenLink(string shareLink)
    {
        var uri = new Uri(shareLink);

        var bitly = string.Format(BitlyUrl, BitlyClientId, BitlyClientSecret, Uri.EscapeDataString(uri.ToString()));
        XDocument response;
        try
        {
            response = XDocument.Load(bitly);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message, e);
        }

        var status = response.XPathSelectElement("/response/status_code").Value;
        if (status != ((int)HttpStatusCode.OK).ToString(CultureInfo.InvariantCulture))
        {
            throw new InvalidOperationException(status);
        }

        var data = response.XPathSelectElement("/response/data/url");

        return data.Value;
    }
}
