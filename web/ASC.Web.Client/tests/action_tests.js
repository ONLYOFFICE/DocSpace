const Endpoints = require("./mocking/endpoints.js");

const browser = process.env.profile || "chromium";
const deviceType = process.env.DEVICE_TYPE || "desktop";

const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Client actions on '${browser}' with '${deviceType}' dimension (model)`
  : `Client actions on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

const saveData = {
  lng: "fr",
  timeZoneID: "Pacific/Tahiti",
};

const saveDataWelcomePageSettings = {
  title: "Hello",
};

const saveDataPortalRenaming = {
  alias: "NewPortalName",
};

Scenario("Tfa auth success", async ({ I }) => {
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.mockEndpoint(Endpoints.code, "code");
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "info");
  I.mockEndpoint(Endpoints.self, "self");
  I.mockEndpoint(Endpoints.validation, "validation");

  I.amOnPage("/confirm/TfaAuth");
  I.fillField("code", "123456");
  I.click({
    react: "Button",
  });

  I.see("Documents");
});

Scenario("Tfa auth error", async ({ I }) => {
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.providers, "providers");
  I.mockEndpoint(Endpoints.capabilities, "capabilities");
  I.mockEndpoint(Endpoints.code, "codeError");
  I.mockEndpoint(Endpoints.validation, "validation");

  I.amOnPage("/confirm/TfaAuth");
  I.fillField("code", "123456");
  I.click({
    react: "Button",
  });

  I.see("Web Office");
});

Scenario("Tfa activation success", async ({ I }) => {
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.mockEndpoint(Endpoints.setup, "setup");
  I.mockEndpoint(Endpoints.validation, "validation");
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "info");
  I.mockEndpoint(Endpoints.self, "self");

  I.amOnPage("/confirm/TfaActivation");

  I.fillField("code", "123456");

  I.click({
    react: "Button",
  });

  I.see("Documents");
});

Scenario("Profile remove success", async ({ I }) => {
  I.mockEndpoint(Endpoints.confirm, "confirm");

  I.amOnPage("/confirm/ProfileRemove");

  I.click({
    react: "Button",
  });

  I.wait(2);
  I.see("Web Office");
});

Scenario("Change email", async ({ I }) => {
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "info");
  I.mockEndpoint(Endpoints.self, "self");

  I.amOnPage("/confirm/EmailChange");
  I.see("People");
});

Scenario("Activate email", async ({ I }) => {
  I.mockEndpoint(Endpoints.confirm, "confirm");

  I.amOnPage("/confirm/EmailActivation");
  I.see("Web Office");
});

Scenario("Change password", async ({ I }) => {
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.password, "password");
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "info");
  I.mockEndpoint(Endpoints.self, "self");

  I.amOnPage("/confirm/PasswordChange");

  I.fillField("password", "qwerty12");
  I.click({
    react: "Button",
  });

  I.see("Documents");
});

Scenario("Customization change language", async ({ I }) => {
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.timezones, "timezones");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");
  I.mockEndpoint(Endpoints.timeandlanguage, "timeandlanguage");

  if (deviceType !== "mobile") {
    if (browser === "webkit") {
      I.wait(30);
    }
    I.amOnPage("/settings/common/customization");
  }

  if (deviceType === "mobile") {
    I.amOnPage("/settings/common/customization/language-and-time-zone");
  }

  const languageCurrent = await I.grabTextFrom("#comboBoxLanguage");

  I.click({
    react: "ComboBox",
    props: {
      id: "comboBoxLanguage",
    },
  });

  I.seeElement(".dropdown-container");

  I.click({
    react: "div",
    props: {
      label: "French (France)",
    },
  });

  I.seeElement({
    react: "ComboButton",
    props: {
      selectedOption: { key: "fr", label: "French (France)" },
    },
  });

  const languageNew = await I.grabTextFrom("#comboBoxLanguage");

  if (languageCurrent === languageNew) {
    I.dontSee("You have unsaved changes");

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: true,
      },
    });

    I.seeElement({
      react: "Button",
      props: {
        label: "Cancel",
        isDisabled: true,
      },
    });
  }

  if (languageCurrent !== languageNew) {
    I.see("You have unsaved changes");

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: false,
      },
    });

    I.seeElement({
      react: "Button",
      props: {
        label: "Cancel",
        isDisabled: false,
      },
    });

    I.click("Save");

    I.checkRequest(
      "http://localhost:8092/api/2.0/settings/timeandlanguage.json",
      saveData,
      "settings",
      "timeandlanguage"
    );

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: true,
      },
    });

    I.seeElement({
      react: "Button",
      props: {
        label: "Cancel",
        isDisabled: true,
      },
    });

    I.seeElement({
      react: "ComboButton",
      props: {
        selectedOption: { key: "fr", label: "French (France)" },
      },
    });

    I.dontSee("You have unsaved changes");
  }
});

Scenario("Customization change time zone", async ({ I }) => {
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.timezones, "timezones");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");
  I.mockEndpoint(Endpoints.timeandlanguage, "timeandlanguage");

  if (deviceType !== "mobile") {
    if (browser === "webkit") {
      I.wait(30);
    }
    I.amOnPage("/settings/common/customization");
  }

  if (deviceType === "mobile") {
    I.amOnPage("/settings/common/customization/language-and-time-zone");
  }

  I.click({
    react: "ComboBox",
    props: {
      id: "comboBoxTimezone",
    },
  });

  const timeZoneCurrent = await I.grabTextFrom("#comboBoxTimezone");

  I.seeElement(".dropdown-container");

  I.click({
    react: "div",
    props: {
      label: "(UTC-10:00) Pacific/Tahiti",
    },
  });

  I.seeElement({
    react: "ComboButton",
    props: {
      selectedOption: {
        key: "Pacific/Tahiti",
        label: "(UTC-10:00) Pacific/Tahiti",
      },
    },
  });

  const timeZoneNew = await I.grabTextFrom("#comboBoxTimezone");

  if (timeZoneCurrent === timeZoneNew) {
    I.dontSee("You have unsaved changes");

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: true,
      },
    });

    I.seeElement({
      react: "Button",
      props: {
        label: "Cancel",
        isDisabled: true,
      },
    });
  }

  if (timeZoneCurrent !== timeZoneNew) {
    I.see("You have unsaved changes");

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: false,
      },
    });

    I.seeElement({
      react: "Button",
      props: {
        label: "Cancel",
        isDisabled: false,
      },
    });

    I.click("Save");

    I.checkRequest(
      "http://localhost:8092/api/2.0/settings/timeandlanguage.json",
      saveData,
      "settings",
      "timeandlanguage"
    );

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: true,
      },
    });

    I.seeElement({
      react: "Button",
      props: {
        label: "Cancel",
        isDisabled: true,
      },
    });

    I.seeElement({
      react: "ComboButton",
      props: {
        selectedOption: {
          key: "Pacific/Tahiti",
          label: "(UTC-10:00) Pacific/Tahiti",
        },
      },
    });

    I.dontSee("You have unsaved changes");
  }
});

Scenario("Customization cancel button test language", async ({ I }) => {
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.timezones, "timezones");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");

  if (deviceType !== "mobile") {
    if (browser === "webkit") {
      I.wait(30);
    }
    I.amOnPage("/settings/common/customization");
  }

  if (deviceType === "mobile") {
    I.amOnPage("/settings/common/customization/language-and-time-zone");
  }

  I.click({
    react: "ComboBox",
    props: {
      id: "comboBoxLanguage",
    },
  });

  const languageCurrent = await I.grabTextFrom("#comboBoxLanguage");

  I.seeElement(".dropdown-container");

  I.click({
    react: "div",
    props: {
      label: "French (France)",
    },
  });

  I.seeElement({
    react: "ComboButton",
    props: {
      selectedOption: { key: "fr", label: "French (France)" },
    },
  });

  const languageNew = await I.grabTextFrom("#comboBoxLanguage");

  if (languageCurrent !== languageNew) {
    I.dontSee(languageCurrent);

    I.see("You have unsaved changes");

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: false,
      },
    });

    I.seeElement({
      react: "Button",
      props: {
        label: "Cancel",
        isDisabled: false,
      },
    });

    I.click("Cancel");

    I.dontSee("You have unsaved changes");

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: true,
      },
    });

    I.seeElement({
      react: "Button",
      props: {
        label: "Cancel",
        isDisabled: true,
      },
    });

    I.dontSee(languageNew);

    I.see(languageCurrent);
  }
});

Scenario("Customization cancel button test time zone", async ({ I }) => {
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.timezones, "timezones");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");

  if (deviceType !== "mobile") {
    if (browser === "webkit") {
      I.wait(30);
    }
    I.amOnPage("/settings/common/customization");
  }

  if (deviceType === "mobile") {
    I.amOnPage("/settings/common/customization/language-and-time-zone");
  }

  I.click({
    react: "ComboBox",
    props: {
      id: "comboBoxTimezone",
    },
  });

  const timeZoneCurrent = await I.grabTextFrom("#comboBoxTimezone");

  I.seeElement(".dropdown-container");

  I.click({
    react: "div",
    props: {
      label: "(UTC-10:00) Pacific/Tahiti",
    },
  });

  I.seeElement({
    react: "ComboButton",
    props: {
      selectedOption: {
        key: "Pacific/Tahiti",
        label: "(UTC-10:00) Pacific/Tahiti",
      },
    },
  });

  const timeZoneNew = await I.grabTextFrom("#comboBoxTimezone");

  if (timeZoneCurrent !== timeZoneNew) {
    I.dontSee(timeZoneCurrent);

    I.see("You have unsaved changes");

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: false,
      },
    });

    I.seeElement({
      react: "Button",
      props: {
        label: "Cancel",
        isDisabled: false,
      },
    });

    I.click("Cancel");

    I.dontSee("You have unsaved changes");

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: true,
      },
    });

    I.seeElement({
      react: "Button",
      props: {
        label: "Cancel",
        isDisabled: true,
      },
    });

    I.dontSee(timeZoneNew);

    I.see(timeZoneCurrent);
  }
});

Scenario("Welcome Page Settings Save button test", async ({ I }) => {
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.timezones, "timezones");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");
  I.mockEndpoint(Endpoints.greetingsettings, "greetingsettings");

  if (deviceType !== "mobile") {
    if (browser === "webkit") {
      I.wait(30);
    }
    I.amOnPage("/settings/common/customization");
  }

  if (deviceType === "mobile") {
    I.amOnPage("/settings/common/customization/welcome-page-settings");
  }

  const titleCurrent = await I.grabValueFrom("#textInputContainerWelcomePage");

  I.fillField("#textInputContainerWelcomePage", "Hello");

  I.seeElement({
    react: "TextInput",
    props: {
      value: "Hello",
    },
  });

  const titleNew = await I.grabValueFrom("#textInputContainerWelcomePage");

  if (titleCurrent === titleNew) {
    I.dontSee("You have unsaved changes");

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: true,
      },
    });
  }

  if (titleCurrent !== titleNew) {
    I.see("You have unsaved changes");

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: false,
      },
    });

    I.click({
      react: "Button",
      props: {
        label: "Save",
        id: "buttonsWelcomePage",
      },
    });

    I.checkRequest(
      "http://localhost:8092/api/2.0/settings/greetingsettings.json",
      saveDataWelcomePageSettings,
      "settings",
      "greetingsettings"
    );

    I.seeElement({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: true,
      },
    });

    I.seeElement({
      react: "TextInput",
      props: {
        value: "Hello",
      },
    });

    I.dontSee("You have unsaved changes");
  }
});

Scenario(
  "Welcome Page Settings Restore to Default button test",
  async ({ I }) => {
    I.mockEndpoint(Endpoints.common, "common");
    I.mockEndpoint(Endpoints.cultures, "cultures");
    I.mockEndpoint(Endpoints.timezones, "timezones");
    I.mockEndpoint(Endpoints.settings, "settingsCustomization");
    I.mockEndpoint(Endpoints.build, "build");
    I.mockEndpoint(Endpoints.info, "infoSettings");
    I.mockEndpoint(Endpoints.self, "selfSettings");
    I.mockEndpoint(Endpoints.restore, "restore");

    if (deviceType !== "mobile") {
      if (browser === "webkit") {
        I.wait(30);
      }
      I.amOnPage("/settings/common/customization");
    }

    if (deviceType === "mobile") {
      I.amOnPage("/settings/common/customization/welcome-page-settings");
    }

    I.fillField("#textInputContainerWelcomePage", "Hello");

    I.seeElement({
      react: "TextInput",
      props: {
        value: "Hello",
      },
    });

    I.click({
      react: "Button",
      props: {
        label: "Restore to Default",
        id: "buttonsWelcomePage",
      },
    });

    I.checkRequest(
      "http://localhost:8092/api/2.0/settings/greetingsettings/restore.json",
      "",
      "settings",
      "restore"
    );

    I.seeElement({
      react: "TextInput",
      props: {
        value: "Cloud Office Applications",
      },
    });
  }
);

Scenario("Portal Renaming cancel button test", async ({ I }) => {
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.timezones, "timezones");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");

  if (deviceType !== "mobile") {
    if (browser === "webkit") {
      I.wait(30);
    }
    I.amOnPage("/settings/common/customization");
  }

  if (deviceType === "mobile") {
    I.amOnPage("/settings/common/customization/portal-renaming");
  }

  const nameCurrent = await I.grabValueFrom(
    "#textInputContainerPortalRenaming"
  );

  I.fillField("#textInputContainerPortalRenaming", "NewPortalName");

  I.seeElement({
    react: "TextInput",
    props: {
      value: "NewPortalName",
    },
  });

  within("#buttonsPortalRenaming", () => {
    I.click({
      react: "Button",
      props: {
        label: "Cancel",
        isDisabled: false,
      },
    });
  });

  I.dontSee("You have unsaved changes");
  I.dontSee("NewPortalName");

  I.seeInField("#textInputContainerPortalRenaming", nameCurrent);
});

Scenario("Portal Renaming save button test", async ({ I }) => {
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.timezones, "timezones");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");
  I.mockEndpoint(Endpoints.portalRenaming, "portalRenaming");

  if (deviceType !== "mobile") {
    if (browser === "webkit") {
      I.wait(30);
    }
    I.amOnPage("/settings/common/customization");
  }

  if (deviceType === "mobile") {
    I.amOnPage("/settings/common/customization/portal-renaming");
  }

  I.fillField("#textInputContainerPortalRenaming", "NewPortalName");

  I.seeElement({
    react: "TextInput",
    props: {
      value: "NewPortalName",
    },
  });

  within("#buttonsPortalRenaming", () => {
    I.click({
      react: "Button",
      props: {
        label: "Save",
        isDisabled: false,
      },
    });
  });

  I.checkRequest(
    "http://localhost:8092/api/2.0/portal/portalrename.json",
    saveDataPortalRenaming,
    "settings",
    "portalRenaming"
  );

  I.dontSee("You have unsaved changes");

  I.seeElement({
    react: "TextInput",
    props: {
      value: "NewPortalName",
    },
  });
});

Scenario("Portal Renaming error PortalNameLength test", async ({ I }) => {
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.timezones, "timezones");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");

  if (deviceType !== "mobile") {
    if (browser === "webkit") {
      I.wait(30);
    }
    I.amOnPage("/settings/common/customization");
  }

  if (deviceType === "mobile") {
    I.amOnPage("/settings/common/customization/portal-renaming");
  }

  I.fillField("#textInputContainerPortalRenaming", "12345");

  I.see("The account name must be between 6 and 50 characters long");
});

Scenario("Portal Renaming error PortalNameIncorrect test", async ({ I }) => {
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.timezones, "timezones");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");

  if (deviceType !== "mobile") {
    if (browser === "webkit") {
      I.wait(30);
    }
    I.amOnPage("/settings/common/customization");
  }

  if (deviceType === "mobile") {
    I.amOnPage("/settings/common/customization/portal-renaming");
  }

  I.fillField("#textInputContainerPortalRenaming", "Новое имя");

  I.see("Incorrect account name");
});

// SECURITY SETTINGS TESTS

Scenario("Setting password strength change test success", async ({ I }) => {
  I.mockEndpoint(Endpoints.self, "selfSettings");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.password, "password");
  I.mockEndpoint(Endpoints.tfaapp, "tfaapp");

  if (deviceType === "mobile") {
    I.amOnPage("/settings/security/access-portal/password");

    I.see("Minimal password length");

    I.click({
      react: "Checkbox",
      props: {
        value: "digits",
      },
    });

    I.see("You have unsaved changes");
    I.click("Save");

    I.see("Settings have been successfully updated");
  }
});

Scenario("Setting password strength change test error", async ({ I }) => {
  I.mockEndpoint(Endpoints.self, "selfSettings");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.password, "password");
  I.mockEndpoint(Endpoints.tfaapp, "tfaapp");

  if (deviceType === "mobile") {
    I.amOnPage("/settings/security/access-portal/password");

    I.see("Minimal password length");

    I.click({
      react: "Checkbox",
      props: {
        value: "digits",
      },
    });

    I.see("You have unsaved changes");

    I.mockEndpoint(Endpoints.password, "passwordError");

    I.click("Save");

    I.see("Error");
  }
});

Scenario("Tfa on from settings", async ({ I }) => {
  I.mockEndpoint(Endpoints.self, "selfSettings");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.password, "password");
  I.mockEndpoint(Endpoints.tfaapp, "tfaapp");
  I.mockEndpoint(Endpoints.tfaconfirm, "tfaconfirm");
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.mockEndpoint(Endpoints.setup, "setup");

  if (deviceType === "mobile") {
    I.amOnPage("/settings/security/access-portal/tfa");

    I.see("Two-factor authentication");

    I.click({
      react: "RadioButton",
      props: {
        value: "app",
      },
    });

    I.click("Save");
    I.see("Configure your authenticator application");
  }
});

Scenario("Tfa on from settings custom scenario", async ({ I }) => {
  I.mockEndpoint(Endpoints.self, "selfSettings");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.password, "password");
  I.mockEndpoint(Endpoints.tfaapp, "tfaapp");
  I.mockEndpoint(Endpoints.tfaconfirm, "tfaconfirm");
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.mockEndpoint(Endpoints.setup, "setup");
  I.mockEndpoint(Endpoints.providers, "providers");
  I.mockEndpoint(Endpoints.capabilities, "capabilities");
  I.mockEndpoint(Endpoints.auth, "auth");

  if (deviceType === "mobile") {
    I.amOnPage("/settings/security/access-portal/tfa");

    I.see("Two-factor authentication");

    I.click({
      react: "RadioButton",
      props: {
        value: "app",
      },
    });

    I.click("Save");
    I.see("Configure your authenticator application");

    I.click({
      react: "Avatar",
    });
    I.see("Settings");
    I.click("Settings");
    I.see("Common");

    I.click({
      react: "Avatar",
    });
    I.click("Sign Out");
    I.see("Cloud Office Applications");

    I.fillField("login", "test@example.com");
    I.fillField("password", "12345678");
    I.click({
      react: "Button",
      props: {
        className: "login-button",
        type: "page",
      },
    });

    I.wait(2);

    I.see("Configure your authenticator application");
  }
});

Scenario("Trusted mail settings change test success", async ({ I }) => {
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.password, "password");
  I.mockEndpoint(Endpoints.tfaapp, "tfaapp");
  I.mockEndpoint(Endpoints.maildomainsettings, "maildomainsettings");

  if (deviceType === "mobile") {
    I.amOnPage("/settings/security/access-portal/trusted-mail");

    I.see("Trusted mail domain settings");

    I.click({
      react: "RadioButton",
      props: {
        value: "1",
      },
    });

    I.see("You have unsaved changes");

    I.see("Add trusted domain");
    I.click("Add trusted domain");

    I.seeElement("#domain-input-0");
    I.fillField("#user-input-0", "test.com");

    I.click("Save");

    I.dontSee("You have unsaved changes");
    I.see("Settings have been successfully updated");
  }
});

Scenario("Trusted mail settings change test error", async ({ I }) => {
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.password, "password");
  I.mockEndpoint(Endpoints.tfaapp, "tfaapp");
  I.mockEndpoint(Endpoints.maildomainsettings, "maildomainsettings");

  if (deviceType === "mobile") {
    I.amOnPage("/settings/security/access-portal/trusted-mail");

    I.see("Trusted mail domain settings");

    I.click({
      react: "RadioButton",
      props: {
        value: "1",
      },
    });

    I.see("You have unsaved changes");

    I.see("Add trusted domain");
    I.click("Add trusted domain");

    I.seeElement("#domain-input-0");
    I.fillField("#user-input-0", "test");

    I.click("Save");

    I.see("You have unsaved changes");
    I.see("Incorrect domain");
  }
});

Scenario("Trusted mail settings change test server error", async ({ I }) => {
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.password, "password");
  I.mockEndpoint(Endpoints.tfaapp, "tfaapp");
  I.mockEndpoint(Endpoints.maildomainsettings, "maildomainsettingsError");

  if (deviceType === "mobile") {
    I.amOnPage("/settings/security/access-portal/trusted-mail");

    I.see("Trusted mail domain settings");

    I.click({
      react: "RadioButton",
      props: {
        value: "1",
      },
    });

    I.see("You have unsaved changes");

    I.see("Add trusted domain");
    I.click("Add trusted domain");

    I.seeElement("#domain-input-0");
    I.fillField("#domain-input-0", "test.com");

    I.click("Save");

    I.see("You have unsaved changes");
    I.see("Request failed with status code 400");
  }
});
