const Endpoints = require("./mocking/endpoints.js");

const browser = process.env.profile || "chromium";
const deviceType = process.env.DEVICE_TYPE || "desktop";
const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Catalog test on '${browser}' with '${deviceType}' dimension (model)`
  : `Catalog test on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

Before(async ({ I }) => {
  I.mockEndpoint(Endpoints.self, "self");
  I.mockEndpoint(Endpoints.settings, "settings");
  I.mockEndpoint(Endpoints.build, "build");
  I.mockEndpoint(Endpoints.info, "info");
  I.mockEndpoint(Endpoints.common, "common");
  I.mockEndpoint(Endpoints.cultures, "cultures");
  I.mockEndpoint(Endpoints.fileSettings, "default");

  I.mockEndpoint(Endpoints.capabilities, "capabilities");
  I.mockEndpoint(Endpoints.thirdparty, "thirdparty");
  I.mockEndpoint(Endpoints.thumbnails, "thumbnails");
});

Scenario("Catalog with empty folders", ({ I }) => {
  I.mockEndpoint(Endpoints.root, "empty");
  I.mockEndpoint(Endpoints.my, "empty");
  I.mockEndpoint(Endpoints.getFolder(1), "1");
  I.mockEndpoint(Endpoints.getFolder(2), "emptyShared");
  I.mockEndpoint(Endpoints.getFileOperation(2), "2-empty");
  I.mockEndpoint(Endpoints.getFolder(3), "emptyFavorites");
  I.mockEndpoint(Endpoints.getFileOperation(3), "3-empty");
  I.mockEndpoint(Endpoints.getFolder(4), "emptyRecent");
  I.mockEndpoint(Endpoints.getFileOperation(4), "4-empty");
  I.mockEndpoint(Endpoints.getFolder(5), "emptyPrivate");
  I.mockEndpoint(Endpoints.getFileOperation(5), "5-empty");
  I.mockEndpoint(Endpoints.getFolder(6), "emptyCommon");
  I.mockEndpoint(Endpoints.getFileOperation(6), "6-empty");
  I.mockEndpoint(Endpoints.getFolder(7), "emptyTrash");
  I.mockEndpoint(Endpoints.getFileOperation(7), "7-empty");

  I.amOnPage("/products/files");

  I.wait(3);

  I.saveScreenshot(`catalog.documents-empty.png`);
  if (!isModel) {
    I.seeVisualDiff(`catalog.documents-empty.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }

  if (deviceType === "desktop") {
    I.click({ react: "CatalogItem", props: { id: 2 } });

    I.saveScreenshot(`catalog.shared-empty.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.shared-empty.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }

    I.click({ react: "CatalogItem", props: { id: 3 } });

    I.saveScreenshot(`catalog.favorites-empty.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.favorites-empty.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }

    I.click({ react: "CatalogItem", props: { id: 4 } });

    I.saveScreenshot(`catalog.recent-empty.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.recent-empty.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }

    I.click({ react: "CatalogItem", props: { id: 5 } });

    I.saveScreenshot(`catalog.private-empty.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.private-empty.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }

    I.click({ react: "CatalogItem", props: { id: 6 } });

    I.saveScreenshot(`catalog.common-empty.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.common-empty.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }

    I.click({ react: "CatalogItem", props: { id: 7 } });

    I.saveScreenshot(`catalog.trash-empty.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.trash-empty.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }
  }
});

Scenario("Documents with many files", ({ I }) => {
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "default");
  I.mockEndpoint(Endpoints.getFolder(1), "1");

  I.amOnPage("/products/files");

  I.wait(3);

  I.saveScreenshot(`catalog.documents-many.png`);
  if (!isModel) {
    I.seeVisualDiff(`catalog.documents-many.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }
});

Scenario("Catalog actions", ({ I }) => {
  I.mockEndpoint(Endpoints.root, "one");
  I.mockEndpoint(Endpoints.my, "default");
  I.mockEndpoint(Endpoints.news, "news");
  I.mockEndpoint(Endpoints.getFolder(1), "1");

  I.amOnPage("/products/files");

  I.wait(3);

  if (deviceType === "desktop") {
    I.click({ react: "Badge", props: { className: "catalog-item__badge" } });

    I.wait(1);

    I.saveScreenshot(`catalog.badge.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.badge.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }

    I.forceClick({ react: "Backdrop" });

    I.click({ react: "MainButton" });

    I.saveScreenshot(`catalog.main-button.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.main-button.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }
  }

  if (deviceType === "tablet" || deviceType === "smallTablet") {
    I.saveScreenshot(`catalog.close.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.close.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }

    // open catalog
    I.click({ name: "catalog-burger" });

    I.saveScreenshot(`catalog.open.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.open.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }

    // close catalog
    I.click({ name: "catalog-burger" });

    if (!isModel) {
      I.seeVisualDiff(`catalog.close.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }

    I.click({ react: "MainButtonMobile" });

    I.saveScreenshot(`catalog.main-button.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.main-button.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }
  }

  if (deviceType === "mobile") {
    // open catalog
    I.click({ name: "catalog-burger" });

    I.saveScreenshot(`catalog.open.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.open.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }

    // close catalog
    I.click({ react: "CatalogBackdrop" });

    I.click({ react: "MainButtonMobile" });

    I.saveScreenshot(`catalog.main-button.png`);
    if (!isModel) {
      I.seeVisualDiff(`catalog.main-button.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }
  }
});
