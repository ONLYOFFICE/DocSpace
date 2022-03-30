const Endpoints = require("./mocking/endpoints.js");

const browser = process.env.profile || "chromium";
const deviceType = process.env.DEVICE_TYPE || "desktop";

const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Client actions on '${browser}' with '${deviceType}' dimension (model)`
  : `Client actions on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

Scenario("Tfa auth success", async ({ I }) => {
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.mockEndpoint(Endpoints.code, "code");
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "info");
  I.mockEndpoint(Endpoints.self, "self");

  I.amOnPage("/confirm/TfaAuth");
  I.fillField("code", "123456");
  I.click({
    react: "Button",
  });

  I.see("Documents");
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

Scenario("Tfa auth error", async ({ I }) => {
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.mockEndpoint(Endpoints.code, "codeError");

  I.amOnPage("/confirm/TfaAuth");
  I.fillField("code", "123456");
  I.click({
    react: "Button",
  });

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
    props: {
      className: "password-button",
    },
  });

  I.see("Documents");
});

Scenario("Change language", async ({ I }) => {
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.timezones, "timezones");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");

  I.amOnPage("/settings/common/customization");

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

  I.see("You have unsaved changes");

  I.click("Save");

  I.seeElement({
    react: "ComboButton",
    props: {
      selectedOption: { key: "fr", label: "French (France)" },
    },
  });

  I.dontSee("You have unsaved changes");
});

Scenario("Change time zone", async ({ I }) => {
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.timezones, "timezones");
  I.mockEndpoint(Endpoints.settings, "settingsCustomization");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "infoSettings");
  I.mockEndpoint(Endpoints.self, "selfSettings");

  I.amOnPage("/settings/common/customization");

  I.click({
    react: "ComboBox",
    props: {
      id: "comboBoxTimezone",
    },
  });

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

  I.see("You have unsaved changes");

  I.click("Save");

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
});
