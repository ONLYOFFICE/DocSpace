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
