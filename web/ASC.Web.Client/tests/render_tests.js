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

Scenario("Profile Remove page render test", async ({ I }) => {
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.amOnPage(
    "/confirm/ProfileRemove?type=ProfileRemove&key=EXAMPLEKEY1&email=test%40example.com&uid=EXAMPLEUID"
  );

  I.see("Web Office");
  I.see("Attention! You are about to delete your account.");
  I.see('By clicking "Delete my account" you agree with our Privacy policy.');

  I.seeElement({
    react: "Button",
  });

  I.saveScreenshot(`2.profile-remove.png`);
  if (!isModel) {
    I.seeVisualDiff(`2.profile-remove.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario("Change phone page render test", async ({ I }) => {
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.confirm, "confirm");
  I.amOnPage(
    "/confirm/PhoneActivation?type=PhoneActivation&key=EXAMPLEKEY1&email=test%40example.com&uid=EXAMPLEUID"
  );

  I.see("Enter mobile phone number");

  I.seeElement({
    react: "TextInput",
  });

  I.seeElement({
    react: "Button",
  });

  I.saveScreenshot(`3.change-phone.png`);
  if (!isModel) {
    I.seeVisualDiff(`3.change-phone.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario("Create user page render test", async ({ I }) => {
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.password, "password");
  I.mockEndpoint(Endpoints.providers, "providers");
  I.mockEndpoint(Endpoints.confirm, "confirm");

  I.amOnPage("/confirm/LinkInvite?type=LinkInvite&key=EXAMPLEKEY1&emplType=1");

  I.see("Web Office");

  I.saveScreenshot(`4.create-user.png`);
  if (!isModel) {
    I.seeVisualDiff(`4.create-user.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario("Change owner page render test", async ({ I }) => {
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.confirm, "confirm");

  I.amOnPage("/confirm/PortalOwnerChange");

  I.see("Web Office");
  I.see("Please confirm that you want to change portal owner");

  I.seeElement({
    react: "Button",
    props: {
      className: "owner-button owner-buttons",
    },
  });

  I.seeElement({
    react: "Button",
    props: {
      className: "owner-buttons",
    },
  });

  I.saveScreenshot(`5.change-owner.png`);
  if (!isModel) {
    I.seeVisualDiff(`5.change-owner.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario("Activate user page render test", async ({ I }) => {
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.password, "password");
  I.mockEndpoint(Endpoints.providers, "providers");
  I.mockEndpoint(Endpoints.confirm, "confirm");

  I.amOnPage("/confirm/Activation");

  I.see("Web Office");

  I.seeElement({
    react: "TextInput",
    props: {
      className: "confirm-row",
      id: "name",
    },
  });

  I.seeElement({
    react: "TextInput",
    props: {
      className: "confirm-row",
      id: "surname",
    },
  });

  I.seeElement({
    react: "PasswordInput",
    props: {
      className: "confirm-row",
      id: "password",
    },
  });

  I.seeElement({
    react: "Button",
    props: {
      className: "confirm-row",
    },
  });

  I.saveScreenshot(`6.activate-user.png`);
  if (!isModel) {
    I.seeVisualDiff(`6.activate-user.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});
