const Endpoints = require("./mocking/endpoints.js");

const browser = process.env.profile || "chromium";
const deviceType = process.env.DEVICE_TYPE || "desktop";
const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Modal test on '${browser}' with '${deviceType}' dimension (model)`
  : `Modal test on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

Before(async ({ I }) => {
  I.mockEndpoint(Endpoints.self, "self");
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "info");
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "default");
  I.mockEndpoint(Endpoints.fileSettings, "default");
  I.mockEndpoint(Endpoints.getFolder(1), "1");
  I.mockEndpoint(Endpoints.capabilities, "capabilities");
  I.mockEndpoint(Endpoints.thirdparty, "thirdparty");
  I.mockEndpoint(Endpoints.thumbnails, "thumbnails");
});

Scenario("Modal test - Copy", ({ I }) => {
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "default");
  I.mockEndpoint(Endpoints.getFolder(1), "1");

  I.amOnPage("/products/files");
  I.wait(3);

  if (deviceType === "desktop") {
    I.click({ react: "TableCell", className: "files-item" });
  } else {
    I.click({ react: "Checkbox" });
  }
  I.wait(2);

  I.click("Copy");
  I.wait(1);
});

Scenario("Modal test - Move", ({ I }) => {
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "default");
  I.mockEndpoint(Endpoints.getFolder(1), "1");

  I.amOnPage("/products/files");
  I.wait(3);

  I.click({
    react: "Checkbox",
  });
  I.wait(1);

  I.click("Move");
  I.wait(1);
});

Scenario("Modal test - Delete", ({ I }) => {
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "default");
  I.mockEndpoint(Endpoints.getFolder(1), "1");

  I.amOnPage("/products/files");
  I.wait(3);

  if (deviceType === "desktop") {
    I.click({ react: "TableCell", className: "files-item" });
  } else {
    I.click({ react: "Checkbox" });
  }
  I.wait(2);

  I.click("Delete");
  I.wait(1);
});

Scenario("Modal test - Trash", ({ I }) => {
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "default");
  I.mockEndpoint(Endpoints.getFolder(1), "1");

  I.amOnPage("/products/files");
  I.wait(3);

  I.mockEndpoint(Endpoints.getFolder(7), "1");
  //I.mockEndpoint(Endpoints.getFileOperation(7), "7-empty");
  I.click({ react: "CatalogItem", props: { id: 7 } });
  I.wait(3);
});

Scenario("Modal test - Add account (List of thirdparties)", ({ I }) => {
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "default");
  I.mockEndpoint(Endpoints.getFolder(1), "1");

  I.amOnPage("/products/files");
  I.wait(3);

  I.click("Add account");
  I.wait(1);
});

Scenario("Modal test - Add account (Connection form)", ({ I }) => {
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "default");
  I.mockEndpoint(Endpoints.getFolder(1), "1");

  I.amOnPage("/products/files");
  I.wait(3);

  I.click({
    react: "IconButton",
    props: { iconName: "images/services/more.svg" },
  });
  I.wait(1);
});

Scenario("Modal test - Overwrite confirmation", ({ I }) => {
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "default");
  I.mockEndpoint(Endpoints.getFolder(1), "1");

  I.amOnPage("/products/files");
  I.wait(3);

  if (deviceType === "desktop") {
    I.click({ react: "TableCell", className: "files-item" });
  } else {
    I.click({ react: "Checkbox" });
  }
  I.wait(2);

  I.click("Copy");
  I.wait(1);
});

Scenario("Modal test - Sharing panel", ({ I }) => {
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "default");
  I.mockEndpoint(Endpoints.getFolder(1), "1");
  I.mockEndpoint(Endpoints.share, "share");

  I.amOnPage("/products/files");
  I.wait(3);

  if (deviceType === "desktop") {
    I.click({ react: "TableCell", className: "files-item" });
  } else {
    I.click({ react: "Checkbox" });
  }
  I.wait(2);

  I.click("Share");
  I.wait(1);
});

Scenario("Modal test - Version history panel", ({ I }) => {
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "default");
  I.mockEndpoint(Endpoints.getFolder(1), "1");
  I.mockEndpoint(Endpoints.history, "history");

  I.amOnPage("/products/files");
  I.wait(3);

  I.click({
    react: "ContextMenuButton",
    props: {
      className: "expandButton",
      title: "Show File Actions",
    },
  });
  I.wait(2);

  I.click("Version history");
  I.wait(1);

  I.click("Show version history");
  I.wait(1);
});
