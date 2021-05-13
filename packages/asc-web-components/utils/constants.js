/**
 * Enum for type of email value errors.
 * @readonly
 */
export const parseErrorTypes = Object.freeze({
  None: 0,
  EmptyRecipients: 1,
  IncorrectEmail: 2,
});

export const errorKeys = Object.freeze({
  LocalDomain: "LocalDomain",
  IncorrectDomain: "IncorrectDomain",
  DomainIpAddress: "DomainIpAddress",
  PunycodeDomain: "PunycodeDomain",
  PunycodeLocalPart: "PunycodeLocalPart",
  IncorrectLocalPart: "IncorrectLocalPart",
  SpacesInLocalPart: "SpacesInLocalPart",
  MaxLengthExceeded: "MaxLengthExceeded",
  IncorrectEmail: "IncorrectEmail",
  ManyEmails: "ManyEmails",
  EmptyEmail: "EmptyEmail",
});
