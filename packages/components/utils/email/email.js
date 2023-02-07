/* eslint-disable no-useless-escape, no-control-regex */
import emailAddresses from "email-addresses";
import punycode from "punycode";
import { parseErrorTypes, errorKeys } from "./../constants";
import { EmailSettings } from "./emailSettings";

const getParts = (str) => {
  const parts = [];
  let newStr = str.replace(/[\s,;]*$/, ",");
  const n = newStr.length;
  let flag = false,
    boundaryIndex = 0,
    index;
  for (index = 0; index < n; index++) {
    switch (newStr.charAt(index)) {
      case ",":
      case ";":
        if (flag) continue;

        let part = newStr.substring(boundaryIndex, index);
        part = part.trim();
        if (part) {
          parts.push(part);
        }
        boundaryIndex = index + 1;

        break;
      case '"':
        if (
          "\\" !== newStr.charAt(index - 1) &&
          '"' !== newStr.charAt(index + 1)
        ) {
          flag = !flag;
        }
    }
  }

  if (!parts.length) {
    parts.push(str.replace(/,\s*$/, ""));
  }

  return parts;
};

const normalizeString = (str) => {
  let r1 = /^"(.*)"\s*<([^>]+)>$/,
    r2 = /^(.*)<([^>]+)>$/,
    match = str.match(r1) || str.match(r2);

  let name, email;

  if (match) {
    name = match[1].replace(/\\"/g, '"').replace(/\\\\/g, "\\").trim();
    email = match[2].trim();
  } else {
    email = str;
  }

  const result = name
    ? `"${name.replace(/\\/g, "\\\\").replace(/"/g, '\\"')}" <${email}>`
    : email;

  return result;
};

const checkErrors = (parsedAddress, options) => {
  const errors = [];

  if (
    !options.allowLocalDomainName &&
    parsedAddress.domain.indexOf(".") === -1
  ) {
    errors.push({
      message: "Local domains are not supported",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress,
      errorKey: errorKeys.LocalDomain,
    });
  }

  if (
    !(
      options.allowDomainIp ||
      options.allowDomainPunycode ||
      options.allowLocalDomainName
    ) &&
    !/(^((?!-)[a-zA-Z0-9-]{1,63}\.)+[a-zA-Z]{2,63}\.?$)/.test(
      parsedAddress.domain
    )
  ) {
    errors.push({
      message: "Incorrect domain",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress,
      errorKey: errorKeys.IncorrectDomain,
    });
  }

  if (
    !options.allowDomainIp &&
    parsedAddress.domain.indexOf("[") === 0 &&
    parsedAddress.domain.indexOf("]") === parsedAddress.domain.length - 1
  ) {
    errors.push({
      message: "Domains as ip address are not supported",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress,
      errorKey: errorKeys.DomainIpAddress,
    });
  }

  if (
    !options.allowDomainPunycode &&
    !/^[\x00-\x7F]+$/.test(punycode.toUnicode(parsedAddress.domain))
  ) {
    errors.push({
      message: "Punycode domains are not supported",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress,
      errorKey: errorKeys.PunycodeDomain,
    });
  }

  if (
    !options.allowLocalPartPunycode &&
    parsedAddress.local.length > 0 &&
    !/^[\x00-\x7F]+$/.test(punycode.toUnicode(parsedAddress.local))
  ) {
    errors.push({
      message: "Punycode local part are not supported",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress,
      errorKey: errorKeys.PunycodeLocalPart,
    });
  }

  if (
    options.allowStrictLocalPart &&
    (!/^[\x00-\x7F]+$/.test(parsedAddress.local) ||
      !/^([a-zA-Z0-9]+)([_\-\.\+][a-zA-Z0-9]+)*$/.test(parsedAddress.local))
  ) {
    errors.push({
      message: "Incorrect localpart",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress,
      errorKey: errorKeys.IncorrectLocalPart,
    });
  }

  if (
    !options.allowSpaces &&
    (/\s+/.test(parsedAddress.local) ||
      parsedAddress.local !== parsedAddress.parts.local.tokens)
  ) {
    errors.push({
      message: "Incorrect, localpart contains spaces",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress,
      errorKey: errorKeys.SpacesInLocalPart,
    });
  }

  if (parsedAddress.local.length > 64) {
    errors.push({
      message:
        "The maximum total length of a user name or other local-part is 64 characters. See RFC2821",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress,
      errorKey: errorKeys.MaxLengthExceeded,
    });
  }

  return errors;
};

const parseOneAddress = (str, options) => {
  const normalizedStr = normalizeString(str);
  const parsedAddress = emailAddresses.parseOneAddress(normalizedStr);

  const errors = [];

  if (!parsedAddress || (parsedAddress.name && !options.allowName)) {
    errors.push({
      message: "Incorrect email",
      type: parseErrorTypes.IncorrectEmail,
      errorKey: errorKeys.IncorrectEmail,
    });
  } else {
    const checkOptionErrors = checkErrors(parsedAddress, options);
    checkOptionErrors.length && errors.push(...checkOptionErrors);
  }

  return parsedAddress
    ? new Email(parsedAddress.name, parsedAddress.address, errors)
    : new Email(null, str, errors);
};

/**
 * Parse addresses from string
 * @param {String} str
 * @return {Array} result with array of Email objects
 */
export const parseAddresses = (str, options = new EmailSettings()) => {
  if (!(options instanceof EmailSettings))
    throw new TypeError("Invalid options");

  const resultEmails = [];

  if (!str || !str.trim()) {
    return resultEmails;
  }

  const parts = getParts(str);

  let i,
    n = parts.length;
  for (i = 0; i < n; i++) {
    resultEmails.push(parseOneAddress(parts[i], options));
  }

  return resultEmails;
};

/**
 * Parse address from string
 * @param {String} str
 * @return {Email} result
 */
export const parseAddress = (str, options = new EmailSettings()) => {
  const parsedEmails = parseAddresses(str, options);

  if (!parsedEmails.length) {
    return new Email("", str, [
      {
        message: "No one email parsed",
        type: parseErrorTypes.EmptyRecipients,
        errorKey: errorKeys.EmptyEmail,
      },
    ]);
  }

  if (parsedEmails.length > 1) {
    return new Email("", str, [
      {
        message: "Too many email parsed",
        type: parseErrorTypes.IncorrectEmail,
        errorKey: errorKeys.ManyEmails,
      },
    ]);
  }

  const resultEmail = parsedEmails[0];

  return resultEmail;
};

/**
 * Check domain validity
 * @param {String} domain
 * @return {Bool} result
 */
export const isValidDomainName = (domain) => {
  let parsed = emailAddresses.parseOneAddress("test@" + domain);
  if (!parsed) return false;
  return parsed && parsed.domain === domain && domain.indexOf(".") !== -1;
};

/**
 * Compare emails
 * @param {String}/{Object} email1
 * @param {String}/{Object} email2
 * @return {Bool} result
 */
export const isEqualEmail = (email1, email2) => {
  const emailSettings = new EmailSettings();
  emailSettings.disableAllSettings();

  const parsed1 = parseAddress(email1, emailSettings);
  const parsed2 = parseAddress(email2, emailSettings);

  if (!parsed1.isValid() || !parsed2.isValid()) {
    return false;
  }

  return parsed1.email === parsed2.email;
};

export class Email {
  constructor(name, email, parseErrors) {
    this.name = name || "";
    this.email = email;
    this.parseErrors = parseErrors;
  }

  isValid = () => {
    return this.parseErrors.length === 0;
  };

  equals = function (addr) {
    if (typeof addr === "object" && addr instanceof Email) {
      return this.email === addr.email && this.name === addr.name;
    } else if (typeof addr === "string") {
      const parsed = parseAddress(addr);
      return this.email === parsed.email && this.name === parsed.name;
    }

    return false;
  };
}
