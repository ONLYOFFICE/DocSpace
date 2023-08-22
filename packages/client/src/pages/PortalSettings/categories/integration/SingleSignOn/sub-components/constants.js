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

export const ssoBindingOptions = [
  {
    id: "sso-post",
    value: "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST",
    label: "POST",
    disabled: false,
  },
  {
    id: "sso-redirect",
    value: "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect",
    label: "Redirect",
    disabled: false,
  },
];

export const sloBindingOptions = [
  {
    id: "slo-post",
    value: "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST",
    label: "POST",
    disabled: false,
  },
  {
    id: "slo-redirect",
    value: "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect",
    label: "Redirect",
    disabled: false,
  },
];
