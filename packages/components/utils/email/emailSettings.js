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

  static equals = (settings1, settings2) => {
    const instance1 = EmailSettings.parse(settings1);
    const instance2 = EmailSettings.parse(settings2);
    const comparedProperties = [
      "allowDomainPunycode",
      "allowLocalPartPunycode",
      "allowDomainIp",
      "allowStrictLocalPart",
      "allowSpaces",
      "allowName",
      "allowLocalDomainName",
    ];
    const propLength = comparedProperties.length;
    for (let i = 0; i < propLength; i++) {
      const comparedProp = comparedProperties[i];
      if (instance1[comparedProp] !== instance2[comparedProp]) {
        return false;
      }
    }
    return true;
  };

  get allowDomainPunycode() {
    return this._allowDomainPunycode;
  }

  set allowDomainPunycode(value) {
    if (value !== undefined && typeof value === "boolean") {
      this._allowDomainPunycode = value;
    } else {
      throw new TypeError(
        `Invalid value ${value} for allowDomainPunycode option. Use boolean value`
      );
    }
  }

  get allowLocalPartPunycode() {
    return this._allowLocalPartPunycode;
  }

  set allowLocalPartPunycode(value) {
    if (value !== undefined && typeof value === "boolean") {
      this._allowLocalPartPunycode = value;
    } else {
      throw new TypeError(
        `Invalid value ${value} for allowLocalPartPunycode option. Use boolean value`
      );
    }
  }

  get allowDomainIp() {
    return this._allowDomainIp;
  }

  set allowDomainIp(value) {
    if (value !== undefined && typeof value === "boolean") {
      this._allowDomainIp = value;
    } else {
      throw new TypeError(
        `Invalid value ${value} for allowDomainIp option. Use boolean value`
      );
    }
  }

  get allowStrictLocalPart() {
    return this._allowStrictLocalPart;
  }

  set allowStrictLocalPart(value) {
    if (value !== undefined && typeof value === "boolean") {
      this._allowStrictLocalPart = value;
    } else {
      throw new TypeError(
        `Invalid value ${value} for allowStrictLocalPart option. Use boolean value`
      );
    }
  }

  get allowSpaces() {
    return this._allowSpaces;
  }

  set allowSpaces(value) {
    if (value !== undefined && typeof value === "boolean") {
      this._allowSpaces = value;
    } else {
      throw new TypeError(
        `Invalid value ${value} for allowSpaces option. Use boolean value`
      );
    }
  }

  get allowName() {
    return this._allowName;
  }

  set allowName(value) {
    if (value !== undefined && typeof value === "boolean") {
      this._allowName = value;
    } else {
      throw new TypeError(
        `Invalid value ${value} for allowName option. Use boolean value`
      );
    }
  }

  get allowLocalDomainName() {
    return this._allowLocalDomainName;
  }

  set allowLocalDomainName(value) {
    if (value !== undefined && typeof value === "boolean") {
      this._allowLocalDomainName = value;
    } else {
      throw new TypeError(
        `Invalid value ${value} for allowLocalDomainName option. Use boolean value`
      );
    }
  }

  toObject() {
    return {
      allowDomainPunycode: this.allowDomainPunycode,
      allowLocalPartPunycode: this.allowLocalPartPunycode,
      allowDomainIp: this.allowDomainIp,
      allowStrictLocalPart: this.allowStrictLocalPart,
      allowSpaces: this.allowSpaces,
      allowName: this.allowName,
      allowLocalDomainName: this.allowLocalDomainName,
    };
  }

  disableAllSettings() {
    this.allowDomainPunycode = true;
    this.allowLocalPartPunycode = true;
    this.allowDomainIp = true;
    this.allowStrictLocalPart = false;
    this.allowSpaces = true;
    this.allowName = true;
    this.allowLocalDomainName = true;
  }

  static parse = (settings) => {
    if (settings instanceof EmailSettings) return settings;

    if (typeof settings !== "object") throw new Error("Invalid argument");

    const defaultSettings = new EmailSettings();
    Object.keys(settings).map((key) => {
      if (!(key in defaultSettings) || defaultSettings[key] === settings[key])
        return;

      defaultSettings[key] = settings[key];
    });

    return defaultSettings;
  };
}
