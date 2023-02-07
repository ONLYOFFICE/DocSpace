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
public class GoogleLoginProvider : BaseLoginProvider<GoogleLoginProvider>
{
    public const string GoogleScopeContacts = "https://www.googleapis.com/auth/contacts.readonly";
    public const string GoogleScopeDrive = "https://www.googleapis.com/auth/drive";
    //https://developers.google.com/gmail/imap/xoauth2-protocol
    public const string GoogleScopeMail = "https://mail.google.com/";
    public const string GoogleUrlContacts = "https://www.google.com/m8/feeds/contacts/default/full/";
    public const string GoogleUrlFile = "https://www.googleapis.com/drive/v3/files/";
    public const string GoogleUrlFileUpload = "https://www.googleapis.com/upload/drive/v3/files";
    public const string GoogleUrlProfile = "https://people.googleapis.com/v1/people/me";

    public override string AccessTokenUrl => "https://www.googleapis.com/oauth2/v4/token";
    public override string CodeUrl => "https://accounts.google.com/o/oauth2/v2/auth";
    public override string RedirectUri => this["googleRedirectUrl"];
    public override string ClientID => this["googleClientId"];
    public override string ClientSecret => this["googleClientSecret"];
    public override string Scopes => "https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email";

    public static readonly string[] GoogleDriveExt = new[] { ".gdoc", ".gsheet", ".gslides", ".gdraw" };
    public static readonly string GoogleDriveMimeTypeFolder = "application/vnd.google-apps.folder";
    public static readonly string FilesFields = "id,name,mimeType,parents,createdTime,modifiedTime,owners/displayName,lastModifyingUser/displayName,capabilities/canEdit,size";
    public static readonly string ProfileFields = "emailAddresses,genders,names";

    private readonly RequestHelper _requestHelper;

    public GoogleLoginProvider() { }
    public GoogleLoginProvider(
        OAuth20TokenHelper oAuth20TokenHelper,
        TenantManager tenantManager,
        CoreBaseSettings coreBaseSettings,
        CoreSettings coreSettings,
        IConfiguration configuration,
        ICacheNotify<ConsumerCacheItem> cache,
        ConsumerFactory consumerFactory,
        Signature signature,
        InstanceCrypto instanceCrypto,
            RequestHelper requestHelper,
        string name, int order, Dictionary<string, string> props, Dictionary<string, string> additional = null)
            : base(oAuth20TokenHelper, tenantManager, coreBaseSettings, coreSettings, configuration, cache, consumerFactory, signature, instanceCrypto, name, order, props, additional)
    {
        _requestHelper = requestHelper;
    }

    public override LoginProfile GetLoginProfile(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new Exception("Login failed");
        }

        return RequestProfile(accessToken);
    }

    public OAuth20Token Auth(HttpContext context)
    {
        return Auth(context, GoogleScopeContacts, out var _, (context.Request.Query["access_type"].ToString() ?? "") == "offline"
            ? new Dictionary<string, string>
            {
                    { "access_type", "offline" },
                    { "prompt", "consent" }
            }
            : null);
    }

    private LoginProfile RequestProfile(string accessToken)
    {
        var googleProfile = _requestHelper.PerformRequest(GoogleUrlProfile + "?personFields=" + HttpUtility.UrlEncode(ProfileFields), headers: new Dictionary<string, string> { { "Authorization", "Bearer " + accessToken } });
        var loginProfile = ProfileFromGoogle(googleProfile);

        return loginProfile;
    }

    private LoginProfile ProfileFromGoogle(string googleProfile)
    {
        var jProfile = JObject.Parse(googleProfile);
        if (jProfile == null)
        {
            throw new Exception("Failed to correctly process the response");
        }

        var profile = new LoginProfile(Signature, InstanceCrypto)
        {
            Id = jProfile.Value<string>("resourceName").Replace("people/", ""),
            Provider = ProviderConstants.Google,
        };

        var emailsArr = jProfile.Value<JArray>("emailAddresses");
        if (emailsArr != null)
        {
            var emailsList = emailsArr.ToObject<List<GoogleEmailAddress>>();
            if (emailsList.Count > 0)
            {
                var ind = emailsList.FindIndex(googleEmail => googleEmail.Metadata.Primary);
                profile.EMail = emailsList[ind > -1 ? ind : 0].Value;
            }
        }

        var namesArr = jProfile.Value<JArray>("names");
        if (namesArr != null)
        {
            var namesList = namesArr.ToObject<List<GoogleName>>();
            if (namesList.Count > 0)
            {
                var ind = namesList.FindIndex(googleName => googleName.Metadata.Primary);
                var name = namesList[ind > -1 ? ind : 0];
                profile.DisplayName = name.DisplayName;
                profile.FirstName = name.GivenName;
                profile.LastName = name.FamilyName;
            }
        }

        var gendersArr = jProfile.Value<JArray>("genders");
        if (gendersArr != null)
        {
            var gendersList = gendersArr.ToObject<List<GoogleGender>>();
            if (gendersList.Count > 0)
            {
                var ind = gendersList.FindIndex(googleGender => googleGender.Metadata.Primary);
                profile.Gender = gendersList[ind > -1 ? ind : 0].Value;
            }
        }

        return profile;
    }

    private class GoogleEmailAddress
    {
        public GoogleMetadata Metadata { get; set; } = new GoogleMetadata();
        public string Value { get; set; }
    }

    private class GoogleGender
    {
        public GoogleMetadata Metadata { get; set; } = new GoogleMetadata();
        public string Value { get; set; }
    }

    private class GoogleName
    {
        public GoogleMetadata Metadata { get; set; } = new GoogleMetadata();
        public string DisplayName { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
    }

    private class GoogleMetadata
    {
        public bool Primary { get; set; }
    }
}
