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

const checkErrors = (parsedAddress, options) => {
  const errors = [];

  if (!options.allowLocalDomainName &&
    (parsedAddress.domain.indexOf(".") === -1)) {
    errors.push({
      message: "Local domains are not supported",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress
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
      errorItem: parsedAddress
    });
  }

  if (!options.allowDomainPunycode && !/^[\x00-\x7F]+$/.test(punycode.toUnicode(parsedAddress.domain))) {
    errors.push({
      message: "Punycode domains are not supported",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress
    });
  }

  if (!options.allowLocalPartPunycode && parsedAddress.local.length > 0 && !/^[\x00-\x7F]+$/.test(punycode.toUnicode(parsedAddress.local))) {
    errors.push({
      message: "Punycode local part are not supported",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress
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
      errorItem: parsedAddress
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
      errorItem: parsedAddress
    });
  }

  if (!options.allowSpaces &&
    (/\s+/.test(parsedAddress.domain) ||
      parsedAddress.domain !== parsedAddress.parts.domain.tokens)
  ) {
    errors.push({
      message: "Incorrect, domain contains spaces",
      type: parseErrorTypes.IncorrectEmail,
      errorItem: parsedAddress
    });
  }

  if (parsedAddress.local.length > 64) {
    errors.push({
      message: "The maximum total length of a user name or other local-part is 64 characters. See RFC2821",
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
export const parseAddresses = (str, options = new EmailSettings()) => {
  if (!(options instanceof EmailSettings)) throw "Invalid options";

  const parts = getParts(str);
  const resultEmails = [];

  let i,
    n = parts.length;
  for (i = 0; i < n; i++) {
    const normalizedStr = normalizeString(parts[i]);
    const parsedAddress = emailAddresses.parseOneAddress(normalizedStr);

    const errors = [];

    if (!parsedAddress || (parsedAddress.name && !options.allowName)) {
      errors.push({
        message: "Incorrect email",
        type: parseErrorTypes.IncorrectEmail
      });
    } else {
      const checkOptionErrors = checkErrors(parsedAddress, options)
      checkOptionErrors.length && errors.push(checkOptionErrors);
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
export const parseAddress = (str, options = new EmailSettings()) => {
  const parsedEmails = parseAddresses(str, options);

  if (!parsedEmails.length) {
    return new Email("", str, [
      { message: "No one email parsed", type: parseErrorTypes.EmptyRecipients }
    ]);
  }

  if (parsedEmails.length > 1) {
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

  equals = function (addr) {
    if (typeof addr === "object" && addr instanceof Email) {
      return this.email === addr.email && this.name === addr.name;
    } else if (typeof addr === "string") {
      var parsed = parseAddress(addr);
      return this.email === parsed.email && this.name === parsed.name;
    }

    return false;
  };
}

export class EmailSettings {
  constructor() {
    this.allowDomainPunycode = false;
    this.allowLocalPartPunycode = false;
    this.allowDomainIp = false;
    this.allowStrictLocalPart = true;
    this.allowSpaces = false;
    this.allowName = false;
    this.allowLocalDomainName = false;
  }

  get allowDomainPunycode() {
    return this._allowDomainPunycode;
  }

  set allowDomainPunycode(value) {
    if (value !== undefined && typeof value === 'boolean') {
      this._allowDomainPunycode = value;
    }
    else {
      throw `Invalid value ${value} for allowDomainPunycode option. Use boolean value`
    }
  }

  get allowLocalPartPunycode() {
    return this._allowLocalPartPunycode;
  }

  set allowLocalPartPunycode(value) {
    if (value !== undefined && typeof value === 'boolean') {
      this._allowLocalPartPunycode = value;
    }
    else {
      throw `Invalid value ${value} for allowLocalPartPunycode option. Use boolean value`
    }
  }

  get allowDomainIp() {
    return this._allowDomainIp;
  }

  set allowDomainIp(value) {
    if (value !== undefined && typeof value === 'boolean') {
      this._allowDomainIp = value;
    }
    else {
      throw `Invalid value ${value} for allowDomainIp option. Use boolean value`
    }
  }

  get allowStrictLocalPart() {
    return this._allowStrictLocalPart;
  }

  set allowStrictLocalPart(value) {
    if (value !== undefined && typeof value === 'boolean') {
      this._allowStrictLocalPart = value;
    }
    else {
      throw `Invalid value ${value} for allowStrictLocalPart option. Use boolean value`
    }
  }

  get allowSpaces() {
    return this._allowSpaces;
  }

  set allowSpaces(value) {
    if (value !== undefined && typeof value === 'boolean') {
      this._allowSpaces = value;
    }
    else {
      throw `Invalid value ${value} for allowSpaces option. Use boolean value`
    }
  }

  get allowName() {
    return this._allowName;
  }

  set allowName(value) {
    if (value !== undefined && typeof value === 'boolean') {
      this._allowName = value;
    }
    else {
      throw `Invalid value ${value} for allowName option. Use boolean value`
    }
  }

  get allowLocalDomainName() {
    return this._allowLocalDomainName;
  }

  set allowLocalDomainName(value) {
    if (value !== undefined && typeof value === 'boolean') {
      this._allowLocalDomainName = value;
    }
    else {
      throw `Invalid value ${value} for allowLocalDomainName option. Use boolean value`
    }
  }

  getSettings() {
    return {
      allowDomainPunycode: this.allowDomainPunycode,
      allowLocalPartPunycode: this.allowLocalPartPunycode,
      allowDomainIp: this.allowDomainIp,
      allowStrictLocalPart: this.allowStrictLocalPart,
      allowSpaces: this.allowSpaces,
      allowName: this.allowName,
      allowLocalDomainName: this.allowLocalDomainName
    }
  }
}

export const checkAndConvertEmailSettings = (settings) => {
  if (typeof settings === 'object' && !(settings instanceof EmailSettings)) {
    const defaultSettings = new EmailSettings();
    console.log('NEW SETTINGS:', defaultSettings.getSettings())
    Object.keys(settings).map((item) => {
      if (defaultSettings[item] !== null && defaultSettings[item] != settings[item]) {
        defaultSettings[item] = settings[item];
      }
    });
    return defaultSettings;
  }

  else if (typeof settings === 'object' && settings instanceof EmailSettings) {
    return settings;
  }
}

export const isEqualEmailSettings = (settings1, settings2) => {
  const comparedProperties = [
    'allowDomainPunycode',
    'allowLocalPartPunycode',
    'allowDomainIp',
    'allowStrictLocalPart',
    'allowSpaces',
    'allowName',
    'allowLocalDomainName'
  ];
  const propLength = comparedProperties.length;
  for (let i = 0; i < propLength; i++) {
    const comparedProp = comparedProperties[i]
    if (settings1[comparedProp] !== settings2[comparedProp]) {
      return false;
    }
  }
  return true;
}