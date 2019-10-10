import emailAddresses from "email-addresses";
import punycode from "punycode";
import { parseErrorTypes } from "./constants";

const getParts = string => {
  let mass = [];
  let e = string.replace(/[\s,;]*$/, ",");
  for (let t, i = false, o = 0, a = 0, s = e.length; s > a; a += 1) {
    switch (e.charAt(a)) {
      case ",":
      case ";":
        if (!i) {
          t = e.substring(o, a);
          t = t.trim();
          if (t) {
            mass.push(t);
          }
          o = a + 1;
        }
        break;
      case '"':
        "\\" !== e.charAt(a - 1) && '"' !== e.charAt(a + 1) && (i = !i);
    }
  }
  return mass;
};

const str2Obj = str => {
  let t = /^"(.*)"\s*<([^>]+)>$/,
    n = /^(.*)<([^>]+)>$/,
    i = str.match(t) || str.match(n);
  return i
    ? {
        name: i[1]
          .replace(/\\"/g, '"')
          .replace(/\\\\/g, "\\")
          .trim(),
        email: i[2].trim()
      }
    : {
        email: str
      };
};

const obj2str = object => {
  let t = undefined;
  if (object.email) {
    t = object.email;
    object.name &&
      (t =
        '"' +
        object.name.replace(/\\/g, "\\\\").replace(/"/g, '\\"') +
        '" <' +
        t +
        ">");
  }
  return t;
};

const normalizeString = str => {
  return obj2str(str2Obj(str));
};

const checkErrors = parsedAddress => {
  const errors = [];
  if (
    parsedAddress.domain.indexOf(".") === -1 ||
    !/(^((?!-)[a-zA-Z0-9-]{2,63}\.)+[a-zA-Z]{2,63}\.?$)/.test(
      parsedAddress.domain
    )
  ) {
    errors.push({
      message: "Incorrect domain",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress
    });
  }

  if (
    parsedAddress.domain.indexOf("[") === 0 &&
    parsedAddress.domain.indexOf("]") === parsedAddress.domain.length - 1
  ) {
    errors.push({
      message: "Domains as ip address are not supported",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress
    });
  }

  if (!/^[\x00-\x7F]+$/.test(punycode.toUnicode(parsedAddress.domain))) {
    errors.push({
      message: "Punycode domains are not supported",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress
    });
  }

  if (
    !/^[\x00-\x7F]+$/.test(parsedAddress.local) ||
    !/^([a-zA-Z0-9]+)([_\-\.\+][a-zA-Z0-9]+)*$/.test(parsedAddress.local)
  ) {
    errors.push({
      message: "Incorrect localpart",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress
    });
  }

  if (
    /\s+/.test(parsedAddress.local) ||
    parsedAddress.local !== parsedAddress.parts.local.tokens
  ) {
    errors.push({
      message: "Incorrect, localpart contains spaces",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress
    });
  }

  if (
    /\s+/.test(parsedAddress.domain) ||
    parsedAddress.domain !== parsedAddress.parts.domain.tokens
  ) {
    errors.push({
      message: "Incorrect, domain contains spaces",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress
    });
  }

  return errors;
};

/**
 * Parse addresses from string
 * @param {String} str
 * @return {Array} result with array of Email objects
 */
export const parseAddresses = (str, options = new EmailOptions()) => {
  if (!(options instanceof EmailOptions)) throw "Invalid options";

  const parts = getParts(str);
  const resultEmails = [];

  let i,
    n = parts.length;
  for (i = 0; i < n; i++) {
    const normalizedStr = normalizeString(parts[i]);
    const parsedAddress = emailAddresses.parseOneAddress(normalizedStr);

    const errors = [];

    if (!parsedAddress) {
      errors.push({
        message: "Incorrect email",
        type: parseErrorTypes.IncorrectEmail
      });
    } else {
      errors.concat(checkErrors(parsedAddress));
    }

    resultEmails.push(
      parsedAddress
        ? new Email(parsedAddress.name, parsedAddress.address, errors)
        : new Email(null, parts[i], errors)
    );
  }

  return resultEmails;
};

/**
 * Parse address from string
 * @param {String} str
 * @return {Email} result
 */
export const parseAddress = (str, options = new EmailOptions()) => {
  const parsedEmails = parseAddresses(str, options);

  if (!parseAddresses.length) {
    return new Email("", str, [
      { message: "No one email parsed", type: parseErrorTypes.EmptyRecipients }
    ]);
  }

  if (parseAddresses.length > 1) {
    return new Email("", str, [
      { message: "To many email parsed", type: parseErrorTypes.IncorrectEmail }
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
export const isValidDomainName = domain => {
  let parsed = emailAddresses.parseOneAddress("test@" + domain);
  return parsed && parsed.domain === domain && domain.indexOf(".") !== -1;
};

/**
 * Compare emails
 * @param {String}/{Object} email1
 * @param {String}/{Object} email2
 * @return {Bool} result
 */
export const isEqualEmail = (email1, email2) => {
  let parsed1 = parseAddress(email1);
  let parsed2 = parseAddress(email2);

  if (!parsed1.isValid || !parsed2.isValid) {
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

  equals = function(addr) {
    if (typeof addr === "object" && addr instanceof Email) {
      return this.email === addr.email && this.name === addr.name;
    } else if (typeof addr === "string") {
      var parsed = parseAddress(addr);
      return this.email === parsed.email && this.name === parsed.name;
    }

    return false;
  };
}

export class EmailOptions {
  constructor() {
    this.allowDomainPunycode = false;
    this.allowLocalPartPunycode = false;
    this.allowDomainIp = false;
    this.allowStrictLocalPart = true;
    this.allowSpaces = false;
    this.allowName = true;
  }
}
