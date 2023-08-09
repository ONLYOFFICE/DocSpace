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

namespace ASC.Web.Studio.UserControls.Management.SingleSignOnSettings;

/// <summary>
/// </summary>
[Serializable]
public class SsoSettingsV2 : ISettings<SsoSettingsV2>
{
    [JsonIgnore]
    public Guid ID
    {
        get { return new Guid("{1500187F-B8AB-406F-97B8-04BFE8261DBE}"); }
    }

    public const string SSO_SP_LOGIN_LABEL = "Single Sign-on";

    public SsoSettingsV2 GetDefault()
    {
        return new SsoSettingsV2
        {
            EnableSso = false,

            IdpSettings = new SsoIdpSettings
            {
                EntityId = string.Empty,
                SsoUrl = string.Empty,
                SsoBinding = SsoBindingType.Saml20HttpPost,
                SloUrl = string.Empty,
                SloBinding = SsoBindingType.Saml20HttpPost,
                NameIdFormat = SsoNameIdFormatType.Saml20Transient
            },

            IdpCertificates = new List<SsoCertificate>(),
            IdpCertificateAdvanced = new SsoIdpCertificateAdvanced
            {
                DecryptAlgorithm = SsoEncryptAlgorithmType.AES_128,
                DecryptAssertions = false,
                VerifyAlgorithm = SsoSigningAlgorithmType.RSA_SHA1,
                VerifyAuthResponsesSign = false,
                VerifyLogoutRequestsSign = false,
                VerifyLogoutResponsesSign = false
            },

            SpCertificates = new List<SsoCertificate>(),
            SpCertificateAdvanced = new SsoSpCertificateAdvanced
            {
                DecryptAlgorithm = SsoEncryptAlgorithmType.AES_128,
                EncryptAlgorithm = SsoEncryptAlgorithmType.AES_128,
                EncryptAssertions = false,
                SigningAlgorithm = SsoSigningAlgorithmType.RSA_SHA1,
                SignAuthRequests = false,
                SignLogoutRequests = false,
                SignLogoutResponses = false
            },

            FieldMapping = new SsoFieldMapping
            {
                FirstName = "givenName",
                LastName = "sn",
                Email = "mail",
                Title = "title",
                Location = "l",
                Phone = "mobile"
            },
            SpLoginLabel = SSO_SP_LOGIN_LABEL,
            HideAuthPage = false
        };
    }

    /// <summary>Specifies if SSO is enabled or not</summary>
    /// <type>System.Boolean, System</type>
    public bool EnableSso { get; set; }

    /// <summary>IDP settings</summary>
    /// <type>ASC.Web.Studio.UserControls.Management.SingleSignOnSettings.SsoIdpSettings, ASC.Web.Core</type>
    public SsoIdpSettings IdpSettings { get; set; }

    /// <summary>List of IDP certificates</summary>
    /// <type>System.Collections.Generic.List{ASC.Web.Studio.UserControls.Management.SingleSignOnSettings.SsoCertificate}, System.Collections.Generic</type>
    public List<SsoCertificate> IdpCertificates { get; set; }

    /// <summary>IDP advanced certificate</summary>
    /// <type>ASC.Web.Studio.UserControls.Management.SingleSignOnSettings.SsoIdpCertificateAdvanced, ASC.Web.Core</type>
    public SsoIdpCertificateAdvanced IdpCertificateAdvanced { get; set; }

    /// <summary>SP login label</summary>
    /// <type>System.String, System</type>
    public string SpLoginLabel { get; set; }

    /// <summary>List of SP certificates</summary>
    /// <type>System.Collections.Generic.List{ASC.Web.Studio.UserControls.Management.SingleSignOnSettings.SsoCertificate}, System.Collections.Generic</type>
    public List<SsoCertificate> SpCertificates { get; set; }

    /// <summary>SP advanced certificate</summary>
    /// <type>ASC.Web.Studio.UserControls.Management.SingleSignOnSettings.SsoSpCertificateAdvanced, ASC.Web.Core</type>
    public SsoSpCertificateAdvanced SpCertificateAdvanced { get; set; }

    /// <summary>Field mapping</summary>
    /// <type>ASC.Web.Studio.UserControls.Management.SingleSignOnSettings.SsoFieldMapping, ASC.Web.Core</type>
    public SsoFieldMapping FieldMapping { get; set; }

    /// <summary>Specifies if the authentication page will be hidden or not</summary>
    /// <type>System.Boolean, System</type>
    public bool HideAuthPage { get; set; }
}


#region SpSettings

/// <summary>
/// </summary>
[Serializable]
public class SsoIdpSettings
{
    /// <summary>Entity ID</summary>
    /// <type>System.String, System</type>
    public string EntityId { get; set; }

    /// <summary>SSO URL</summary>
    /// <type>System.String, System</type>
    public string SsoUrl { get; set; }

    /// <summary>SSO binding</summary>
    /// <type>System.String, System</type>
    public string SsoBinding { get; set; }

    /// <summary>SLO URL</summary>
    /// <type>System.String, System</type>
    public string SloUrl { get; set; }

    /// <summary>SLO binding</summary>
    /// <type>System.String, System</type>
    public string SloBinding { get; set; }

    /// <summary>Name ID format</summary>
    /// <type>System.String, System</type>
    public string NameIdFormat { get; set; }
}

#endregion


#region FieldsMapping

[Serializable]
public class SsoFieldMapping
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Title { get; set; }

    public string Location { get; set; }

    public string Phone { get; set; }
}

#endregion


#region Certificates

/// <summary>
/// </summary>
[Serializable]
public class SsoCertificate
{
    /// <summary>Specifies if a certificate is self-signed or not</summary>
    /// <type>System.Boolean, System</type>
    public bool SelfSigned { get; set; }

    /// <summary>Certificate</summary>
    /// <type>System.String, System</type>
    public string Crt { get; set; }

    /// <summary>Key</summary>
    /// <type>System.String, System</type>
    public string Key { get; set; }

    /// <summary>Action</summary>
    /// <type>System.String, System</type>
    public string Action { get; set; }

    /// <summary>Domain name</summary>
    /// <type>System.String, System</type>
    public string DomainName { get; set; }

    /// <summary>Start date</summary>
    /// <type>System.DateTime, System</type>
    public DateTime StartDate { get; set; }

    /// <summary>Expiration date</summary>
    /// <type>System.DateTime, System</type>
    public DateTime ExpiredDate { get; set; }
}

/// <summary>
/// </summary>
[Serializable]
public class SsoIdpCertificateAdvanced
{
    /// <summary>Verification algorithm</summary>
    /// <type>System.String, System</type>
    public string VerifyAlgorithm { get; set; }

    /// <summary>Specifies if the signatures of the SAML authentication responses sent to SP will be verified or not</summary>
    /// <type>System.Boolean, System</type>
    public bool VerifyAuthResponsesSign { get; set; }

    /// <summary>Specifies if the signatures of the SAML logout requests sent to SP will be verified or not</summary>
    /// <type>System.Boolean, System</type>
    public bool VerifyLogoutRequestsSign { get; set; }

    /// <summary>Specifies if the signatures of the SAML logout responses sent to SP will be verified or not</summary>
    /// <type>System.Boolean, System</type>
    public bool VerifyLogoutResponsesSign { get; set; }

    /// <summary>Decryption algorithm</summary>
    /// <type>System.String, System</type>
    public string DecryptAlgorithm { get; set; }

    /// <summary>Specifies if the assertions will be decrypted or not</summary>
    /// <type>System.Boolean, System</type>
    public bool DecryptAssertions { get; set; }
}

/// <summary>
/// </summary>
[Serializable]
public class SsoSpCertificateAdvanced
{
    /// <summary>Signing algorithm</summary>
    /// <type>System.String, System</type>
    public string SigningAlgorithm { get; set; }

    /// <summary>Specifies if SP will sign the SAML authentication requests sent to IdP or not</summary>
    /// <type>System.Boolean, System</type>
    public bool SignAuthRequests { get; set; }

    /// <summary>Specifies if SP will sign the SAML logout requests sent to IdP or not</summary>
    /// <type>System.Boolean, System</type>
    public bool SignLogoutRequests { get; set; }

    /// <summary>Specifies if sign the SAML logout responses sent to IdP or not</summary>
    /// <type>System.Boolean, System</type>
    public bool SignLogoutResponses { get; set; }

    /// <summary>Encryption algorithm</summary>
    /// <type>System.String, System</type>
    public string EncryptAlgorithm { get; set; }

    /// <summary>Decryption algorithm</summary>
    /// <type>System.String, System</type>
    public string DecryptAlgorithm { get; set; }

    /// <summary>Specifies if the assertions will be encrypted or not</summary>
    /// <type>System.Boolean, System</type>
    public bool EncryptAssertions { get; set; }
}

#endregion


#region Types

[Serializable]
public class SsoNameIdFormatType
{
    public const string Saml11Unspecified = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";

    public const string Saml11EmailAddress = "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress";

    public const string Saml20Entity = "urn:oasis:names:tc:SAML:2.0:nameid-format:entity";

    public const string Saml20Transient = "urn:oasis:names:tc:SAML:2.0:nameid-format:transient";

    public const string Saml20Persistent = "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent";

    public const string Saml20Encrypted = "urn:oasis:names:tc:SAML:2.0:nameid-format:encrypted";

    public const string Saml20Unspecified = "urn:oasis:names:tc:SAML:2.0:nameid-format:unspecified";

    public const string Saml11X509SubjectName = "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName";

    public const string Saml11WindowsDomainQualifiedName = "urn:oasis:names:tc:SAML:1.1:nameid-format:WindowsDomainQualifiedName";

    public const string Saml20Kerberos = "urn:oasis:names:tc:SAML:2.0:nameid-format:kerberos";
}

[Serializable]
public class SsoBindingType
{
    public const string Saml20HttpPost = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";

    public const string Saml20HttpRedirect = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect";
}

[Serializable]
public class SsoMetadata
{
    public const string BaseUrl = "";

    public const string MetadataUrl = "/sso/metadata";

    public const string EntityId = "/sso/metadata";

    public const string ConsumerUrl = "/sso/acs";

    public const string LogoutUrl = "/sso/slo/callback";

}

[Serializable]
public class SsoSigningAlgorithmType
{
    public const string RSA_SHA1 = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

    public const string RSA_SHA256 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

    public const string RSA_SHA512 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512";
}

[Serializable]
public class SsoEncryptAlgorithmType
{
    public const string AES_128 = "http://www.w3.org/2001/04/xmlenc#aes128-cbc";

    public const string AES_256 = "http://www.w3.org/2001/04/xmlenc#aes256-cbc";

    public const string TRI_DEC = "http://www.w3.org/2001/04/xmlenc#tripledes-cbc";
}

[Serializable]
public class SsoSpCertificateActionType
{
    public const string Signing = "signing";

    public const string Encrypt = "encrypt";

    public const string SigningAndEncrypt = "signing and encrypt";
}

[Serializable]
public class SsoIdpCertificateActionType
{
    public const string Verification = "verification";

    public const string Decrypt = "decrypt";

    public const string VerificationAndDecrypt = "verification and decrypt";
}

#endregion