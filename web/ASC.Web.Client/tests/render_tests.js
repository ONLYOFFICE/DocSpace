const browser = process.env.profile || "chromium";
const deviceType = process.env.DEVICE_TYPE || "desktop";

const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Client render on '${browser}' with '${deviceType}' dimension (model)`
  : `Client render on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

Before(async ({ I }) => {
  I.amOnPage("/confirm/TfaAuth");
  I.wait(2);
});

Scenario("TfaAuth page render test", async ({ I }) => {
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
