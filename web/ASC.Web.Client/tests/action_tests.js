const Endpoints = require("./mocking/endpoints.js");

const browser = process.env.profile || "chromium";
const deviceType = process.env.DEVICE_TYPE || "desktop";

const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Login actions on '${browser}' with '${deviceType}' dimension (model)`
  : `Login actions on '${browser}' with '${deviceType}' dimension`;

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
