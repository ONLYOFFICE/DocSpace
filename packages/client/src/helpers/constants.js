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

export const GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
export const ID_NO_GROUP_MANAGER = "4a515a15-d4d6-4b8e-828e-e0586f18f3a3";
export const ADS_TIMEOUT = 300000; // 5 min

/**
 * Enum for type of confirm link.
 * @readonly
 */
export const CategoryType = Object.freeze({
  Personal: 0,
  Shared: 1,
  SharedRoom: 2,
  Archive: 3,
  ArchivedRoom: 4,
  Favorite: 5,
  Recent: 6,
  Trash: 7,
});
