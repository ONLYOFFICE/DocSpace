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
public class ProviderManager
{
    public bool IsNotEmpty
    {
        get
        {
            return AuthProviders
                .Select(GetLoginProvider)
                .Any(loginProvider => loginProvider != null && loginProvider.IsEnabled);
        }
    }

    public static readonly List<string> AuthProviders = new List<string>
        {
            ProviderConstants.Google,
            ProviderConstants.Zoom,
            ProviderConstants.LinkedIn,
            ProviderConstants.Facebook ,
            ProviderConstants.Twitter,
            ProviderConstants.Microsoft,
            ProviderConstants.AppleId
        };

    public static List<string> InviteExceptProviders = new List<string>
            {
                ProviderConstants.Twitter,
                ProviderConstants.AppleId,
            };

    private readonly Signature _signature;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly ConsumerFactory _consumerFactory;

    public ProviderManager(Signature signature, InstanceCrypto instanceCrypto, ConsumerFactory consumerFactory)
    {
        _signature = signature;
        _instanceCrypto = instanceCrypto;
        _consumerFactory = consumerFactory;
    }

    public ILoginProvider GetLoginProvider(string providerType)
    {
        return _consumerFactory.GetByKey(providerType) as ILoginProvider;
    }

    public LoginProfile Process(string providerType, HttpContext context, IDictionary<string, string> @params, IDictionary<string, string> additionalStateArgs = null)
    {
        return GetLoginProvider(providerType).ProcessAuthoriztion(context, @params, additionalStateArgs);
    }

    public LoginProfile GetLoginProfile(string providerType, string accessToken = null, string codeOAuth = null)
    {
        var consumer = GetLoginProvider(providerType);
        if (consumer == null)
        {
            throw new ArgumentException("Unknown provider type", nameof(providerType));
        }

        try
        {
            if (accessToken == null && codeOAuth != null)
            {
                return consumer.GetLoginProfile(consumer.GetToken(codeOAuth));
            }
            return consumer.GetLoginProfile(accessToken);
        }
        catch (Exception ex)
        {
            return LoginProfile.FromError(_signature, _instanceCrypto, ex);
        }
    }
}
