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

Scenario("Tfa on from settings", async ({ I }) => {
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");
  I.mockEndpoint(Endpoints.tfaapp, "tfaapp");
  I.mockEndpoint(Endpoints.tfaconfirm, "tfaconfirm");
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.mockEndpoint(Endpoints.setup, "setup");

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
