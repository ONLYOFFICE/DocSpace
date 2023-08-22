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

/// <summary>
/// </summary>
public enum LoginProviderEnum
{
    Facebook,
    Google,
    Dropbox,
    Docusign,
    Box,
    OneDrive,
    GosUslugi,
    LinkedIn,
    MailRu,
    VK,
    Wordpress,
    Yahoo,
    Yandex
}

public abstract class BaseLoginProvider<T> : Consumer, ILoginProvider where T : Consumer, ILoginProvider, new()
{
    public T Instance => ConsumerFactory.Get<T>();
    public virtual bool IsEnabled
    {
        get
        {
            return !string.IsNullOrEmpty(ClientID) &&
                   !string.IsNullOrEmpty(ClientSecret) &&
                   !string.IsNullOrEmpty(RedirectUri);
        }
    }

    public abstract string CodeUrl { get; }
    public abstract string AccessTokenUrl { get; }
    public abstract string RedirectUri { get; }
    public abstract string ClientID { get; }
    public abstract string ClientSecret { get; }
    public virtual string Scopes => string.Empty;

    internal readonly Signature Signature;
    internal readonly InstanceCrypto InstanceCrypto;

    protected readonly OAuth20TokenHelper _oAuth20TokenHelper;

    protected BaseLoginProvider() { }

    protected BaseLoginProvider(
        OAuth20TokenHelper oAuth20TokenHelper,
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory,
        Signature signature,
        InstanceCrypto instanceCrypto,
        string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
        : base(tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, name, order, props, additional)
    {
        _oAuth20TokenHelper = oAuth20TokenHelper;
        Signature = signature;
        InstanceCrypto = instanceCrypto;
    }

    public virtual LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params, IDictionary<string, string> additionalStateArgs)
    {
        try
        {
            var token = Auth(context, Scopes, out var redirect, @params, additionalStateArgs);

            if (redirect)
            {
                return null;
            }

            return GetLoginProfile(token?.AccessToken);
        }
        catch (ThreadAbortException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return LoginProfile.FromError(Signature, InstanceCrypto, ex);
        }
    }

    public abstract LoginProfile GetLoginProfile(string accessToken);

    protected virtual OAuth20Token Auth(HttpContext context, string scopes, out bool redirect, IDictionary<string, string> additionalArgs = null, IDictionary<string, string> additionalStateArgs = null)
    {
        var error = context.Request.Query["error"];
        if (!string.IsNullOrEmpty(error))
        {
            if (error == "access_denied")
            {
                error = "Canceled at provider";
            }

            throw new Exception(error);
        }

        var code = context.Request.Query["code"];
        if (string.IsNullOrEmpty(code))
        {
            context.Response.Redirect(_oAuth20TokenHelper.RequestCode<T>(scopes, additionalArgs, additionalStateArgs));
            redirect = true;

            return null;
        }

        redirect = false;

        return _oAuth20TokenHelper.GetAccessToken<T>(ConsumerFactory, code);
    }

    public virtual LoginProfile GetLoginProfile(OAuth20Token token)
    {
        return GetLoginProfile(token.AccessToken);
    }

    public OAuth20Token GetToken(string codeOAuth)
    {
        return _oAuth20TokenHelper.GetAccessToken<T>(ConsumerFactory, codeOAuth);
    }
}

public static class BaseLoginProviderExtension
{
    public static void Register(DIHelper services)
    {
        services.TryAdd<BoxLoginProvider>();
        services.TryAdd<DropboxLoginProvider>();
        services.TryAdd<OneDriveLoginProvider>();
        services.TryAdd<DocuSignLoginProvider>();
        services.TryAdd<GoogleLoginProvider>();
        services.TryAdd<WordpressLoginProvider>();
    }
}
