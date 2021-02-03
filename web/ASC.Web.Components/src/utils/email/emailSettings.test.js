import { EmailSettings } from "./index";

const defaultEmailSettingsObj = {
  allowDomainPunycode: false,
  allowLocalPartPunycode: false,
  allowDomainIp: false,
  allowStrictLocalPart: true,
  allowSpaces: false,
  allowName: false,
  allowLocalDomainName: false,
};
describe("emailSettings", () => {
  it("get default settings from instance", () => {
    const email = new EmailSettings();
    const settings = email.toObject();
    expect(settings).toStrictEqual(defaultEmailSettingsObj);
  });

  it("change and get settings from instance", () => {
    const emailSettingsObj = {
      allowDomainPunycode: false,
      allowLocalPartPunycode: false,
      allowDomainIp: false,
      allowStrictLocalPart: true,
      allowSpaces: false,
      allowName: false,
      allowLocalDomainName: true,
    };

    const emailSettings = new EmailSettings();
    emailSettings.allowLocalDomainName = true;
    const settings = emailSettings.toObject();

    expect(settings).toStrictEqual(emailSettingsObj);
  });

  it("set and get allowStrictLocalPart setting", () => {
    const emailSettings = new EmailSettings();
    emailSettings.allowStrictLocalPart = false;

    expect(emailSettings.allowStrictLocalPart).toBe(false);
  });

  it("disable settings", () => {
    const disabledSettings = {
      allowDomainPunycode: true,
      allowLocalPartPunycode: true,
      allowDomainIp: true,
      allowStrictLocalPart: false,
      allowSpaces: true,
      allowName: true,
      allowLocalDomainName: true,
    };
    const emailSettings = new EmailSettings();
    emailSettings.disableAllSettings();
    const newSettings = emailSettings.toObject();

    expect(newSettings).toStrictEqual(disabledSettings);
  });

  it("set invalid (non-boolean) value for allowLocalDomainName setting", () => {
    const emailSettings = new EmailSettings();

    try {
      emailSettings.allowLocalDomainName = "1";
    } catch (err) {
      expect(err.name).toBe("TypeError");
    }
  });

  it("set invalid (non-boolean) value for allowDomainPunycode setting", () => {
    const emailSettings = new EmailSettings();

    try {
      emailSettings.allowDomainPunycode = "1";
    } catch (err) {
      expect(err.name).toBe("TypeError");
    }
  });

  it("set invalid (non-boolean) value for allowLocalPartPunycode setting", () => {
    const emailSettings = new EmailSettings();

    try {
      emailSettings.allowLocalPartPunycode = "1";
    } catch (err) {
      expect(err.name).toBe("TypeError");
    }
  });

  it("set invalid (non-boolean) value for allowDomainIp setting", () => {
    const emailSettings = new EmailSettings();

    try {
      emailSettings.allowDomainIp = "1";
    } catch (err) {
      expect(err.name).toBe("TypeError");
    }
  });

  it("set invalid (non-boolean) value for allowStrictLocalPart setting", () => {
    const emailSettings = new EmailSettings();

    try {
      emailSettings.allowStrictLocalPart = "1";
    } catch (err) {
      expect(err.name).toBe("TypeError");
    }
  });

  it("set invalid (non-boolean) value for allowSpaces setting", () => {
    const emailSettings = new EmailSettings();

    try {
      emailSettings.allowSpaces = "1";
    } catch (err) {
      expect(err.name).toBe("TypeError");
    }
  });

  it("set invalid (non-boolean) value for allowName setting", () => {
    const emailSettings = new EmailSettings();

    try {
      emailSettings.allowName = "1";
    } catch (err) {
      expect(err.name).toBe("TypeError");
    }
  });

  // test EmailSettings.equals function

  it("is not equal email settings", () => {
    const emailSettings = new EmailSettings();
    const emailSettings2 = new EmailSettings();

    emailSettings.allowStrictLocalPart = false;
    const isEqual = EmailSettings.equals(emailSettings, emailSettings2);

    expect(isEqual).toBe(false);
  });

  it("is equal email settings", () => {
    const emailSettings = new EmailSettings();
    const emailSettings2 = new EmailSettings();
    const isEqual = EmailSettings.equals(emailSettings, emailSettings2);

    expect(isEqual).toBe(true);
  });

  // test checkAndEmailSettings.parse function

  it("passed instance of default EmailSettings, return same instance", () => {
    const emailSettings = new EmailSettings();
    const convertedSettings = EmailSettings.parse(emailSettings);

    expect(convertedSettings).toStrictEqual(emailSettings);
  });

  it("passed object with default settings, return instance of default EmailSettings", () => {
    const convertedSettings = EmailSettings.parse(defaultEmailSettingsObj);
    const emailSettings = new EmailSettings();

    expect(convertedSettings).toStrictEqual(emailSettings);
  });

  it("passed instance of EmailSettings, return same instance", () => {
    const emailSettings = new EmailSettings();
    emailSettings.allowLocalDomainName = true;
    const convertedSettings = EmailSettings.parse(emailSettings);

    expect(convertedSettings).toStrictEqual(emailSettings);
  });

  it("passed object with settings, return instance of EmailSettings", () => {
    const emailSettingsObj = {
      allowDomainPunycode: true,
      allowLocalPartPunycode: true,
      allowDomainIp: false,
      allowStrictLocalPart: true,
      allowSpaces: false,
      allowName: false,
      allowLocalDomainName: false,
    };

    const convertedSettings = EmailSettings.parse(emailSettingsObj);
    const emailSettings = new EmailSettings();
    emailSettings.allowDomainPunycode = true;
    emailSettings.allowLocalPartPunycode = true;

    expect(convertedSettings).toStrictEqual(emailSettings);
  });

  it("passed invalid object with settings, return instance of EmailSettings", () => {
    const emailSettingsObj = {
      temp: "temp",
      allowDomainPunycode: true,
      allowLocalPartPunycode: true,
      allowDomainIp: false,
      allowStrictLocalPart: true,
      allowSpaces: false,
      allowName: false,
      allowLocalDomainName: false,
    };

    const convertedSettings = EmailSettings.parse(emailSettingsObj);

    const emailSettings = new EmailSettings();
    emailSettings.allowDomainPunycode = true;
    emailSettings.allowLocalPartPunycode = true;

    expect(convertedSettings).toStrictEqual(emailSettings);
  });
});
