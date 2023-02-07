const Endpoints = require("./mocking/endpoints.js");

const browser = process.env.profile || "chromium";
const deviceType = process.env.DEVICE_TYPE || "desktop";

const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Login actions on '${browser}' with '${deviceType}' dimension (model)`
  : `Login actions on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

// doing it before others scenario
Before(async ({ I }) => {
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.providers, "providers");
  I.mockEndpoint(Endpoints.capabilities, "capabilities");
  I.mockEndpoint(Endpoints.people, "");
  I.amOnPage("/login");
  I.wait(2);
});

Scenario("Checkbox click test", async ({ I }) => {
  I.seeElement({
    react: "Checkbox",
    props: {
      className: "login-checkbox",
      isChecked: false,
    },
  });

  I.click({ react: "Checkbox" });

  I.seeElement({
    react: "Checkbox",
    props: {
      className: "login-checkbox",
      isChecked: true,
    },
  });

  I.saveScreenshot(`6.checked-checkbox.png`);
  if (!isModel) {
    I.seeVisualDiff(`6.checked-checkbox.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario("Test login error", async ({ I }) => {
  I.mockEndpoint(Endpoints.auth, "authError");
  I.click({
    react: "Button",
    props: {
      className: "login-button",
      type: "page",
    },
  });
  I.see("Required field");
  I.fillField("login", "test@mail.ru");
  I.click({
    react: "Button",
    props: {
      className: "login-button",
      type: "page",
    },
  });
  I.see("Required field");
  I.fillField("password", secret("0000000"));
  I.click({
    react: "Button",
    props: {
      className: "login-button",
      type: "page",
    },
  });
  I.see("User authentication failed");
});

Scenario("Test login success", async ({ I }) => {
  I.mockEndpoint(Endpoints.people, "self");
  I.mockEndpoint(Endpoints.modules, "info");
  I.mockEndpoint(Endpoints.auth, "authSuccess");
  I.fillField("login", "test@mail.ru");
  I.fillField("password", secret("12345678"));
  I.click({
    react: "Button",
    props: {
      className: "login-button",
      type: "page",
    },
  });
  I.see("Documents");
});

Scenario("Test fix wrong login", async ({ I }) => {
  I.mockEndpoint(Endpoints.people, "self");
  I.mockEndpoint(Endpoints.modules, "info");
  I.mockEndpoint(Endpoints.auth, "authError");
  I.fillField("login", "test@mail.ru");
  I.fillField("password", secret("12345678!"));
  I.click({
    react: "Button",
    props: {
      className: "login-button",
      type: "page",
    },
  });
  I.see("User authentication failed");

  I.mockEndpoint(Endpoints.auth, "authSuccess");
  I.fillField("password", secret("12345678"));
  I.click({
    react: "Button",
    props: {
      className: "login-button",
      type: "page",
    },
  });
  I.see("Documents");
});
