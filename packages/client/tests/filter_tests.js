const Endpoints = require("./mocking/endpoints.js");

const browser = process.env.profile || "chromium";
const deviceType = process.env.DEVICE_TYPE || "desktop";
const isModel = !!process.env.MODEL;

const sizes = {
  mobile: { width: 375, height: 667 },
  smallTablet: { width: 600, height: 667 },
  tablet: { width: 1023, height: 667 },
  desktop: { width: 1920, height: 1080 },
};

const featureName = isModel
  ? `Filter test on '${browser}' with '${deviceType}' dimension (model)`
  : `Filter test on '${browser}' with '${deviceType}' dimension`;

const boundingBox =
  deviceType === "mobile"
    ? { left: 0, top: 0, right: 0, bottom: 0 }
    : { left: sizes[deviceType] - 480, top: 0, right: 0, bottom: 0 };

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

Scenario("Filter block ", ({ I }) => {
  I.mockEndpoint(Endpoints.group, "group");
  I.mockEndpoint(Endpoints.people, "people");
  I.mockEndpoint(Endpoints.admin, "admin");
  I.mockEndpoint(Endpoints.getFile(1), "1");
  I.amOnPage("/products/files");

  I.wait(3);

  // open filter block
  I.click({ react: "FilterButton" });

  I.saveScreenshot(`filter.filter-block.png`);
  if (!isModel) {
    I.seeVisualDiff(`filter.filter-block.png`, {
      tolerance: 3,
      prepareBaseImage: false,
      boundingBox: boundingBox,
    });
  }
  if (deviceType === "desktop") {
    // open author selector
    I.click({ react: "SelectorAddButton" });

    I.saveScreenshot(`filter.author-selector.png`);
    if (!isModel) {
      I.seeVisualDiff(`filter.author-selector.png`, {
        tolerance: 3,
        prepareBaseImage: false,
        boundingBox: boundingBox,
      });
    }

    // open "All users" in author selector
    I.forceClick({ name: "selector-row-option-1" });

    I.saveScreenshot(`filter.all-users.png`);
    if (!isModel) {
      I.seeVisualDiff(`filter.all-users.png`, {
        tolerance: 3,
        prepareBaseImage: false,
        boundingBox: boundingBox,
      });
    }

    // click arrow button with "All users" group selected
    I.click({ react: "IconButton", props: { className: "arrow-button" } });

    if (!isModel) {
      I.seeVisualDiff(`filter.author-selector.png`, {
        tolerance: 3,
        prepareBaseImage: false,
        boundingBox: boundingBox,
      });
    }

    // click arrow button without group selected
    I.click({ react: "IconButton", props: { className: "arrow-button" } });

    if (!isModel) {
      I.seeVisualDiff(`filter.filter-block.png`, {
        tolerance: 3,
        prepareBaseImage: false,
        boundingBox: boundingBox,
      });
    }

    I.selectAuthor();

    I.saveScreenshot(`filter.selected-author.png`);
    if (!isModel) {
      I.seeVisualDiff(`filter.selected-author.png`, {
        tolerance: 3,
        prepareBaseImage: false,

        boundingBox: boundingBox,
      });
    }

    // select "documents" type
    I.click({ name: "documents-3" });

    I.saveScreenshot(`filter.documents-type.png`);
    if (!isModel) {
      I.seeVisualDiff(`filter.documents-type.png`, {
        tolerance: 3,
        prepareBaseImage: false,
        boundingBox: boundingBox,
      });
    }

    // deselect "documents" type
    I.click({ name: "documents-3" });

    if (!isModel) {
      I.seeVisualDiff(`filter.selected-author.png`, {
        tolerance: 3,
        prepareBaseImage: false,
        boundingBox: boundingBox,
      });
    }

    // select "documents" type
    I.click({ name: "documents-3" });

    if (!isModel) {
      I.seeVisualDiff(`filter.documents-type.png`, {
        tolerance: 3,
        prepareBaseImage: false,
        boundingBox: boundingBox,
      });
    }

    // select "no subfolders" filter
    I.click({ react: "ToggleButton", props: { isChecked: false } });

    I.saveScreenshot(`filter.no-subfolders.png`);
    if (!isModel) {
      I.seeVisualDiff(`filter.no-subfolders.png`, {
        tolerance: 3,
        prepareBaseImage: false,
        boundingBox: boundingBox,
      });
    }

    // deselect "no subfolders" filter
    I.click({ react: "ToggleButton", props: { isChecked: true } });

    if (!isModel) {
      I.seeVisualDiff(`filter.documents-type.png`, {
        tolerance: 3,
        prepareBaseImage: false,
        boundingBox: boundingBox,
      });
    }

    // select "no subfolders" filter
    I.click({ react: "ToggleButton", props: { isChecked: false } });

    if (!isModel) {
      I.seeVisualDiff(`filter.no-subfolders.png`, {
        tolerance: 3,
        prepareBaseImage: false,
        boundingBox: boundingBox,
      });
    }

    // clear filter
    I.click({
      react: "IconButton",
      props: { iconName: "/static/images/clear.react.svg" },
    });

    if (!isModel) {
      I.seeVisualDiff(`filter.filter-block.png`, {
        tolerance: 3,
        prepareBaseImage: false,
        boundingBox: boundingBox,
      });
    }

    I.selectAuthor();
    I.click({ name: "documents-3" });
    I.click({ react: "ToggleButton", props: { isChecked: false } });

    if (!isModel) {
      I.seeVisualDiff(`filter.no-subfolders.png`, {
        tolerance: 3,
        prepareBaseImage: false,
        boundingBox: boundingBox,
      });
    }
  }
});

if (deviceType !== "desktop") {
  Scenario("Sort block ", ({ I }) => {
    I.amOnPage("/products/files");

    I.wait(3);

    I.click({ react: "SortButton" });

    I.saveScreenshot(`filter.sort-block.png`);
    if (!isModel) {
      I.seeVisualDiff(`filter.sort-block.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }
  });
}
