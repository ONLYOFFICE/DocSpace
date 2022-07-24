const Endpoints = require("./mocking/endpoints.js");

const browser = process.env.profile || "chromium";
const isModel = !!process.env.MODEL;
const deviceType = process.env.DEVICE_TYPE || "desktop";

const featureName = isModel
  ? `Render test on '${browser}' with '${deviceType}' dimension (model)`
  : `Render test on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

Before(async ({ I }) => {
  I.mockEndpoint(Endpoints.self, "self");
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "info");
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "duplicate");
  I.mockEndpoint(Endpoints.fileSettings, "default");
  I.mockEndpoint(Endpoints.getFolder(1), "1");
  I.mockEndpoint(Endpoints.capabilities, "capabilities");
  I.mockEndpoint(Endpoints.thirdparty, "thirdparty");
  I.mockEndpoint(Endpoints.thumbnails, "thumbnails");
});

Scenario("Duplicate id test", async ({ I }) => {
  I.amOnPage("/products/files");
  I.wait(3);

  if (deviceType !== "desktop") {
    I.click({ react: "SortButton" });
    I.click({ name: "view-selector-name_tile" });
  } else {
    I.click({ name: "view-selector-name_tile" });
  }

  I.wait(3);
  I.saveScreenshot("duplicate-id-tile-view.png");
  if (!isModel) {
    I.seeVisualDiff("duplicate-id-tile-view.png", {
      tolerance: 0.16,
      prepareBaseImage: false,
    });
  }
});
