import EnUSReactSvgUrl from "PUBLIC_DIR/images/flags/en-US.react.svg?url";

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
  Settings: 8,
  Accounts: 9,
});

/**
 * Enum for table columns version
 * @readonly
 */
export const TableVersions = Object.freeze({
  Rooms: "1",
  Files: "2",
  Accounts: "3",
  Trash: "4",
});

/**
 * Enum for quotas bar
 * @readonly
 */
export const QuotaBarTypes = Object.freeze({
  ConfirmEmail: "confirm-email",
  RoomQuota: "room-quota",
  StorageQuota: "storage-quota",
  UserQuota: "user-quota",
  UserAndStorageQuota: "user-storage-quota",
  RoomAndStorageQuota: "room-storage-quota",
});

export const BINDING_POST = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST";
export const BINDING_REDIRECT =
  "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect";
export const SSO_NAME_ID_FORMAT =
  "urn:oasis:names:tc:SAML:2.0:nameid-format:transient";
export const SSO_GIVEN_NAME = "givenName";
export const SSO_SN = "sn";
export const SSO_EMAIL = "email";
export const SSO_LOCATION = "location";
export const SSO_TITLE = "title";
export const SSO_PHONE = "phone";

export const DEFAULT_SELECT_TIMEZONE = {
  key: "UTC",
  label: "(UTC) Coordinated Universal Time",
};

export const DEFAULT_SELECT_LANGUAGE = {
  key: "en-US",
  label: "English (United States)",
  icon: EnUSReactSvgUrl,
};

/**
 * Enum for sort by field name
 * @readonly
 */
export const SortByFieldName = Object.freeze({
  Name: "AZ",
  ModifiedDate: "DateAndTime",
  CreationDate: "DateAndTimeCreation",
  Author: "Author",
  Size: "Size",
  Type: "Type",
  Room: "Room",
  Tags: "Tags",
  RoomType: "roomType",
});

export const SSO_LABEL = "SSO";
