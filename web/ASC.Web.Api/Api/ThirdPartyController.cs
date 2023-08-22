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

namespace ASC.Web.Api.Controllers;

///<summary>
/// Third-party API.
///</summary>
///<name>thirdparty</name>
[Scope(Additional = typeof(BaseLoginProviderExtension))]
[DefaultRoute]
[ApiController]
public class ThirdPartyController : ControllerBase
{
    private readonly OAuth20TokenHelper _oAuth20TokenHelper;

    public ThirdPartyController(OAuth20TokenHelper oAuth20TokenHelper)
    {
        _oAuth20TokenHelper = oAuth20TokenHelper;
    }

    /// <summary>
    /// Returns a request to get the confirmation code from URL.
    /// </summary>
    /// <short>Get the code request</short>
    /// <param type="ASC.FederatedLogin.LoginProviders.LoginProviderEnum, ASC.FederatedLogin.LoginProviders" method="url" name="provider">Provider</param>
    /// <returns type="System.Object, System">Code request</returns>
    /// <remarks>List of providers: Google, Dropbox, Docusign, Box, OneDrive, Wordpress.</remarks>
    /// <path>api/2.0/thirdparty/{provider}</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("{provider}")]
    public object Get(LoginProviderEnum provider)
    {
        var desktop = HttpContext.Request.Query["desktop"] == "true";
        var additionals = new Dictionary<string, string>();

        if (desktop)
        {
            additionals = HttpContext.Request.Query.ToDictionary(r => r.Key, r => r.Value.FirstOrDefault());
        }

        switch (provider)
        {
            case LoginProviderEnum.Google:
                return _oAuth20TokenHelper.RequestCode<GoogleLoginProvider>(
                                                                    GoogleLoginProvider.GoogleScopeDrive,
                                                                    new Dictionary<string, string>
                                                                        {
                                                                    { "access_type", "offline" },
                                                                    { "prompt", "consent" }
                                                                        }, additionalStateArgs: additionals);

            case LoginProviderEnum.Dropbox:
                return _oAuth20TokenHelper.RequestCode<DropboxLoginProvider>(
                                                    additionalArgs: new Dictionary<string, string>
                                                        {
                                                                        { "force_reauthentication", "true" },
                                                                        { "token_access_type","offline" }
                                                        }, additionalStateArgs: additionals);

            case LoginProviderEnum.Docusign:
                return _oAuth20TokenHelper.RequestCode<DocuSignLoginProvider>(
                                                                        DocuSignLoginProvider.DocuSignLoginProviderScopes,
                                                                        new Dictionary<string, string>
                                                                            {
                                                                        { "prompt", "login" }
                                                                            }, additionalStateArgs: additionals);
            case LoginProviderEnum.Box:
                return _oAuth20TokenHelper.RequestCode<BoxLoginProvider>(additionalStateArgs: additionals);

            case LoginProviderEnum.OneDrive:
                return _oAuth20TokenHelper.RequestCode<OneDriveLoginProvider>(OneDriveLoginProvider.OneDriveLoginProviderScopes, additionalStateArgs: additionals);

            case LoginProviderEnum.Wordpress:
                return _oAuth20TokenHelper.RequestCode<WordpressLoginProvider>(additionalStateArgs: additionals);

        }

        return null;
    }

    /// <summary>
    /// Returns the confirmation code for requesting an OAuth token.
    /// </summary>
    /// <short>Get the confirmation code</short>
    /// <param type="System.String, System" name="redirect">URL where the user will be redirected to after they have granted the application access</param>
    /// <param type="System.String, System" method="url" name="code">Confirmation code that can be exchanged for an OAuth token</param>
    /// <param type="System.String, System" name="error">Error</param>
    /// <returns type="System.Object, System">Confirmation code</returns>
    /// <path>api/2.0/thirdparty/{provider}/code</path>
    /// <httpMethod>GET</httpMethod>
    [HttpGet("{provider}/code")]
    public object GetCode(string redirect, string code, string error)
    {
        try
        {
            if (!string.IsNullOrEmpty(error))
            {
                if (error == "access_denied")
                {
                    error = "Canceled at provider";
                }
                throw new Exception(error);
            }

            if (!string.IsNullOrEmpty(redirect))
            {
                return AppendCode(redirect, code);
            }

            return code;
        }
        catch (ThreadAbortException)
        {
        }
        catch (Exception ex)
        {
            if (!string.IsNullOrEmpty(redirect))
            {
                return AppendCode(redirect, error: ex.Message);
            }

            return ex.Message;
        }

        return null;
    }


    private static string AppendCode(string url, string code = null, string error = null)
    {
        url += (url.Contains('#') ? "&" : "#")
                + (string.IsNullOrEmpty(error)
                        ? (string.IsNullOrEmpty(code)
                                ? string.Empty
                                : "code=" + HttpUtility.UrlEncode(code))
                        : ("error/" + HttpUtility.UrlEncode(error)));

        return url;
    }
}
