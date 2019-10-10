import emailAddresses from 'email-addresses';
import punycode from 'punycode';
import { parseErrorTypes } from './constants';

class Email {
  constructor(value) {
    this.value = value;
    this.parsedObjs = {
      addresses: [],
      errors: []
    };
  }

  addressesToArray = (value) => {
    if (!value) return null;
    const normalizedStr = this.normalizeString(value);
    let arrayOfAddressObj = [];
    const normalizedStrLength = normalizedStr.length;

    for (let i = 0; i < normalizedStrLength; i++) {
      const contact2Obj = this.contact2Obj(normalizedStr[i]);
      const obj2Contact = this.obj2Contact(contact2Obj);
      const addressObj = emailAddresses.parseOneAddress(obj2Contact);
      addressObj != null && arrayOfAddressObj.push(addressObj);
    }
    return arrayOfAddressObj.length === 0 ? null : arrayOfAddressObj;
  }

  normalizeString = (string) => {
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
  }

  writeError = () => {
    this.parsedObjs.errors.push({ message: "Incorrect email", type: parseErrorTypes.IncorrectEmail, errorItem: this.value });
    this.parsedObjs.addresses.push("", this.value, false);
  }


  contact2Obj = (object) => {
    let t = /^"(.*)"\s*<([^>]+)>$/,
      n = /^(.*)<([^>]+)>$/,
      i = object.match(t) || object.match(n);
    return i ? {
      name: (i[1].replace(/\\"/g, '"').replace(/\\\\/g, "\\")).trim(),
      email: (i[2]).trim()
    } : {
        email: object
      }
  };

  obj2Contact = (object) => {
    let t = undefined;
    if (object.email) {
      t = object.email;
      object.name && (t = '"' + object.name.replace(/\\/g, "\\\\").replace(/"/g, '\\"') + '" <' + t + ">");
    }
    return t;
  };

  ParseAddress = () => {
    const addresses = this.addressesToArray(this.value)
    if (addresses === null) {
      this.writeError(this.value);
    }
    else {
      this.validateEmail(addresses[0]);
    }

    return this.parsedObjs.addresses;
  }


  validateEmail = (parsedObject) => {

    let isValid = true;
    if (parsedObject.domain.indexOf(".") === -1 || !/(^((?!-)[a-zA-Z0-9-]{2,63}\.)+[a-zA-Z]{2,63}\.?$)/.test(parsedObject.domain)) {
      isValid = false;
      this.parsedObjs.errors.push({ message: "Incorrect domain", type: parseErrorTypes.IncorrectEmail, errorItem: parsedObject });
    }

    if (parsedObject.domain.indexOf('[') === 0 && parsedObject.domain.indexOf(']') === parsedObject.domain.length - 1) {
      this.parsedObjs.errors.push({ message: "Domains as ip address are not supported", type: parseErrorTypes.IncorrectEmail, errorItem: parsedObject });
    }

    if (!/^[\x00-\x7F]+$/.test(punycode.toUnicode(parsedObject.domain))) {
      isValid = false;
      this.parsedObjs.errors.push({ message: "Punycode domains are not supported", type: parseErrorTypes.IncorrectEmail, errorItem: parsedObject });
    }

    if (!/^[\x00-\x7F]+$/.test(parsedObject.local) || !/^([a-zA-Z0-9]+)([_\-\.\+][a-zA-Z0-9]+)*$/.test(parsedObject.local)) {
      isValid = false;
      this.parsedObjs.errors.push({ message: "Incorrect localpart", type: parseErrorTypes.IncorrectEmail, errorItem: parsedObject });
    }

    if (/\s+/.test(parsedObject.local) || parsedObject.local !== parsedObject.parts.local.tokens) {
      isValid = false;
      this.parsedObjs.errors.push({ message: "Incorrect, localpart contains spaces", type: parseErrorTypes.IncorrectEmail, errorItem: parsedObject });
    }

    if (/\s+/.test(parsedObject.domain) || parsedObject.domain !== parsedObject.parts.domain.tokens) {
      isValid = false;
      this.parsedObjs.errors.push({ message: "Incorrect, domain contains spaces", type: parseErrorTypes.IncorrectEmail, errorItem: parsedObject });
    }

    this.parsedObjs.addresses.push(parsedObject.name || "", parsedObject.address, isValid);
  }

  /**
   * Check domain validity
   * @param {String} domain
   * @return {Bool} result
   */
  IsValidDomainName = (domain) => {
    let parsed = emailAddresses.parseOneAddress("test@" + domain);
    return !!parsed && parsed.domain === domain && domain.indexOf(".") !== -1;
  }
  /**
   * Compare emails
   * @param {String}/{Object} email1
   * @param {String}/{Object} email2
   * @return {Bool} result
   */
  IsEqualEmail = (email1, email2) => {
    let parsed1 = this.ParseAddress(email1);
    let parsed2 = this.ParseAddress(email2);

    if (!parsed1.isValid ||
      !parsed2.isValid) {
      return false;
    }

    return parsed1.email === parsed2.email;
  }

}

export default Email;