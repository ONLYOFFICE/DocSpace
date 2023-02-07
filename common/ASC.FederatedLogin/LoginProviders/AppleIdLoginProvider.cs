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
public class AppleIdLoginProvider : BaseLoginProvider<AppleIdLoginProvider>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly RequestHelper _requestHelper;

    public override string AccessTokenUrl { get { return "https://appleid.apple.com/auth/token"; } }
    public override string RedirectUri { get { return this["appleIdRedirectUrl"]; } }
    public override string ClientID { get { return (this["appleIdClientIdMobile"] != null && _httpContextAccessor?.HttpContext != null && _httpContextAccessor.HttpContext.Request.MobileApp()) ? this["appleIdClientIdMobile"] : this["appleIdClientId"]; } }
    public override string ClientSecret => GenerateSecret();
    public override string CodeUrl { get { return "https://appleid.apple.com/auth/authorize"; } }
    public override string Scopes { get { return ""; } }

    public string TeamId { get { return this["appleIdTeamId"]; } }
    public string KeyId { get { return this["appleIdKeyId"]; } }
    public string PrivateKey { get { return this["appleIdPrivateKey"]; } }

    public AppleIdLoginProvider() { }
    public AppleIdLoginProvider(
        OAuth20TokenHelper oAuth20TokenHelper,
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory,
        Signature signature,
        InstanceCrypto instanceCrypto,
        IHttpContextAccessor httpContextAccessor,
        RequestHelper requestHelper,
        string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
            : base(oAuth20TokenHelper, tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, signature, instanceCrypto, name, order, props, additional)
    {
        _httpContextAccessor = httpContextAccessor;
        _requestHelper = requestHelper;
    }

    public override LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params, IDictionary<string, string> additionalStateArgs)
    {
        try
        {
            var token = Auth(context, Scopes, out var redirect, @params, additionalStateArgs);
            var claims = ValidateIdToken(JObject.Parse(token.OriginJson).Value<string>("id_token"));
            return GetProfileFromClaims(claims);
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

    public override LoginProfile GetLoginProfile(string authCode)
    {
        if (string.IsNullOrEmpty(authCode))
        {
            throw new Exception("Login failed");
        }

        var token = _oAuth20TokenHelper.GetAccessToken<AppleIdLoginProvider>(ConsumerFactory, authCode);
        var claims = ValidateIdToken(JObject.Parse(token.OriginJson).Value<string>("id_token"));
        return GetProfileFromClaims(claims);
    }
    public override LoginProfile GetLoginProfile(OAuth20Token token)
    {
        if (token == null)
        {
            throw new Exception("Login failed");
        }

        var claims = ValidateIdToken(JObject.Parse(token.OriginJson).Value<string>("id_token"));
        return GetProfileFromClaims(claims);
    }

    private LoginProfile GetProfileFromClaims(ClaimsPrincipal claims)
    {
        return new LoginProfile(Signature, InstanceCrypto)
        {
            Id = claims.FindFirst(ClaimTypes.NameIdentifier).Value,
            EMail = claims.FindFirst(ClaimTypes.Email)?.Value,
            Provider = ProviderConstants.AppleId,
        };
    }

    private string GenerateSecret()
    {
        using var ecdsa = ECDsa.Create();

        ecdsa.ImportPkcs8PrivateKey(Convert.FromBase64String(PrivateKey), out _);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateJwtSecurityToken(
            issuer: TeamId,
            audience: "https://appleid.apple.com",
            subject: new ClaimsIdentity(new List<Claim> { new Claim("sub", ClientID) }),
            issuedAt: DateTime.UtcNow,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(new ECDsaSecurityKey(ecdsa), SecurityAlgorithms.EcdsaSha256)
        );

        token.Header.Add("kid", KeyId);

        return handler.WriteToken(token);
    }

    private ClaimsPrincipal ValidateIdToken(string idToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var claims = handler.ValidateToken(idToken, new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = GetApplePublicKeys(),

            ValidateIssuer = true,
            ValidIssuer = "https://appleid.apple.com",

            ValidateAudience = true,
            ValidAudience = ClientID,

            ValidateLifetime = true

        }, out var _);

        return claims;
    }

    private IEnumerable<SecurityKey> GetApplePublicKeys()
    {
        var appplePublicKeys = _requestHelper.PerformRequest("https://appleid.apple.com/auth/keys");

        var keys = new List<SecurityKey>();
        foreach (var webKey in JObject.Parse(appplePublicKeys).Value<JArray>("keys"))
        {
            var e = Base64UrlEncoder.DecodeBytes(webKey.Value<string>("e"));
            var n = Base64UrlEncoder.DecodeBytes(webKey.Value<string>("n"));

            var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n })
            {
                KeyId = webKey.Value<string>("kid")
            };

            keys.Add(key);
        }

        return keys;
    }
}