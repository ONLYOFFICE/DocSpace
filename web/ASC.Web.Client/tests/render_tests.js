const Endpoints = require("./mocking/endpoints.js");

const browser = process.env.profile || "chromium";
const deviceType = process.env.DEVICE_TYPE || "desktop";

const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Client render on '${browser}' with '${deviceType}' dimension (model)`
  : `Client render on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

Scenario("TfaAuth page render test", async ({ I }) => {
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.amOnPage(
    "/confirm/TfaAuth?type=TfaAuth&key=EXAMPLEKEY1&email=test%40example.com"
  );

  I.see("Enter code from authentication app");

  I.seeElement({
    react: "TextInput",
    props: {
      id: "code",
    },
  });

  I.seeElement({
    react: "Button",
  });

  I.saveScreenshot(`1.tfa-auth.png`);
  if (!isModel) {
    I.seeVisualDiff(`1.tfa-auth.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});
