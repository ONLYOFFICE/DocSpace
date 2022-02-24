export const verifyAlgorithms = {
  "rsa-sha1": "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
  "rsa-sha256": "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256",
  "rsa-sha512": "http://www.w3.org/2001/04/xmldsig-more#rsa-sha512",
};

export const decryptAlgorithms = {
  "aes128-cbc": "http://www.w3.org/2001/04/xmlenc#aes128-cbc",
  "aes256-cbc": "http://www.w3.org/2001/04/xmlenc#aes256-cbc",
  "tripledes-cbc": "http://www.w3.org/2001/04/xmlenc#tripledes-cbc",
};

export const algorithms = { ...verifyAlgorithms, ...decryptAlgorithms };

export const verifyAlgorithmsOptions = Object.keys(verifyAlgorithms).map(
  (key) => ({
    key: verifyAlgorithms[key],
    label: key,
  })
);

export const decryptAlgorithmsOptions = Object.keys(decryptAlgorithms).map(
  (key) => ({
    key: decryptAlgorithms[key],
    label: key,
  })
);

export const nameIdFormats = {
  "unspecified_1.1": "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified",
  emailAddress: "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress",
  entity: "urn:oasis:names:tc:SAML:2.0:nameid-format:entity",
  transient: "urn:oasis:names:tc:SAML:2.0:nameid-format:transient",
  persistent: "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent",
  encrypted: "urn:oasis:names:tc:SAML:2.0:nameid-format:encrypted",
  "unspecified_2.0": "urn:oasis:names:tc:SAML:2.0:nameid-format:unspecified",
  X509SubjectName: "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName",
  WindowsDomainQualifiedName:
    "urn:oasis:names:tc:SAML:1.1:nameid-format:WindowsDomainQualifiedName",
  kerberos: "urn:oasis:names:tc:SAML:2.0:nameid-format:kerberos",
};

export const nameIdOptions = Object.keys(nameIdFormats).map((key) => ({
  key: nameIdFormats[key],
  label: nameIdFormats[key],
}));

export const bindingOptions = [
  {
    value: "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST",
    label: "POST",
    disabled: false,
  },
  {
    value: "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect",
    label: "Redirect",
    disabled: false,
  },
];

export const defaultStore = {
  spLoginLabel: "",

  enableSso: false,

  // idpSettings
  entityId: "",
  ssoUrl: "",
  ssoBinding: "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST",
  sloUrl: "",
  sloBinding: "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST",
  nameIdFormat: "urn:oasis:names:tc:SAML:2.0:nameid-format:transient",

  idp_certificate: "",
  idp_privateKey: null,
  idp_action: "signing",
  idp_certificates: [],

  // idpCertificateAdvanced
  idp_decryptAlgorithm: "http://www.w3.org/2001/04/xmlenc#aes128-cbc",
  // no checkbox for that
  ipd_decryptAssertions: false,
  idp_verifyAlgorithm: "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
  idp_verifyAuthResponsesSign: false,
  idp_verifyLogoutRequestsSign: false,
  idp_verifyLogoutResponsesSign: false,

  sp_certificate: "",
  sp_privateKey: "",
  sp_action: "signing",
  sp_certificates: [],

  // spCertificateAdvanced
  // null for some reason
  sp_decryptAlgorithm: null,
  sp_encryptAlgorithm: "http://www.w3.org/2001/04/xmlenc#aes128-cbc",
  sp_encryptAssertions: false,
  sp_signAuthRequests: false,
  sp_signLogoutRequests: false,
  sp_signLogoutResponses: false,
  sp_signingAlgorithm: "http://www.w3.org/2000/09/xmldsig#rsa-sha1",
  // sp_verifyAlgorithm : "http://www.w3.org/2000/09/xmldsig#rsa-sha1",

  firstName: "",
  lastName: "",
  email: "",
  location: "",
  title: "",
  phone: "",

  hideAuthPage: false,

  // sp metadata
  sp_entityId: "",
  sp_assertionConsumerUrl: "",
  sp_singleLogoutUrl: "",

  // hide parts of form
  ServiceProviderSettings: true,
  ShowAdditionalParameters: true,
  SPMetadata: true,
  isIdpModalVisible: false,
  isSpModalVisible: false,

  // errors
  uploadXmlUrlHasError: false,
  spLoginLabelHasError: false,

  entityIdHasError: false,
  ssoUrlHasError: false,
  sloUrlHasError: false,

  firstNameHasError: false,
  lastNameHasError: false,
  emailHasError: false,
  locationHasError: false,
  titleHasError: false,
  phoneHasError: false,

  sp_entityIdHasError: false,
  sp_assertionConsumerUrlHasError: false,
  sp_singleLogoutUrlHasError: false,

  // error messages
  uploadXmlUrlErrorMessage: null,
  spLoginLabelErrorMessage: null,

  entityIdErrorMessage: null,
  ssoUrlErrorMessage: null,
  sloUrlErrorMessage: null,

  firstNameErrorMessage: null,
  lastNameErrorMessage: null,
  emailErrorMessage: null,
  locationErrorMessage: null,
  titleErrorMessage: null,
  phoneErrorMessage: null,

  sp_entityIdErrorMessage: null,
  sp_assertionConsumerUrlErrorMessage: null,
  sp_singleLogoutUrlErrorMessage: null,
};
