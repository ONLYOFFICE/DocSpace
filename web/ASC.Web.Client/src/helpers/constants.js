/**
 * Enum for type of confirm link.
 * @readonly
 */
export const ConfirmType = Object.freeze({
  EmpInvite: 0,
  LinkInvite: 1,
  PortalSuspend: 2,
  PortalContinue: 3,
  PortalRemove: 4,
  DnsChange: 5,
  PortalOwnerChange: 6,
  Activation: 7,
  EmailChange: 8,
  EmailActivation: 9,
  PasswordChange: 10,
  ProfileRemove: 11,
  PhoneActivation: 12,
  PhoneAuth: 13,
  Auth: 14,
  TfaActivation: 15,
  TfaAuth: 16,
});

/**
 * Enum for result of validation confirm link.
 * @readonly
 */
export const ValidationResult = Object.freeze({
  Ok: 0,
  Invalid: 1,
  Expired: 2,
});

export const BINDING_POST = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";
export const BINDING_REDIRECT =
  "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect";
