const Endpoints = require("./mocking/endpoints.js");

const browser = process.env.profile || "chromium";
const deviceType = process.env.DEVICE_TYPE || "desktop";
const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Render people on '${browser}' with '${deviceType}' dimension (model)`
  : `Render people on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

// doing it before others scenario
Before(async ({ I }) => {
  I.mockData();
});

Scenario("Test empty people and group lists render", async ({ I }) => {
  I.mockEndpoint(Endpoints.group, "empty");
  I.mockEndpoint(Endpoints.filter, "empty");
  I.openPage();

  I.see("No teammates found");

  I.saveScreenshot(`1.empty-people-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`1.empty-people-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== "desktop") {
    I.openArticle();

    I.saveScreenshot(`2.empty-group-list.png`);
    if (!isModel) {
      I.seeVisualDiff(`2.empty-group-list.png`, {
        tolerance: 1,
        prepareBaseImage: false,
      });
    }
  }
});

Scenario("Test one person and group lists render", async ({ I }) => {
  I.mockEndpoint(Endpoints.filter, "one");
  I.mockEndpoint(Endpoints.group, "one");
  I.openPage();

  I.saveScreenshot(`3.one-person-people-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`3.one-person-people-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== "desktop") {
    I.openArticle();

    I.saveScreenshot(`4.one-group-list.png`);
    if (!isModel) {
      I.seeVisualDiff(`4.one-group-list.png`, {
        tolerance: 1,
        prepareBaseImage: false,
      });
    }
  }
});

Scenario("Test many people and groups lists render", async ({ I }) => {
  I.mockEndpoint(Endpoints.filter, "many");
  I.mockEndpoint(Endpoints.group, "many");
  I.openPage();

  I.saveScreenshot(`5.many-people-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`5.many-people-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== "desktop") {
    I.openArticle();

    I.saveScreenshot(`6.many-groups-list.png`);
    if (!isModel) {
      I.seeVisualDiff(`6.many-groups-list.png`, {
        tolerance: 1,
        prepareBaseImage: false,
      });
    }
  }
});

Scenario("Test profile menu render", async ({ I }) => {
  I.mockEndpoint(Endpoints.group, "empty");
  I.mockEndpoint(Endpoints.filter, "empty");
  I.openPage();

  I.openProfileMenu();

  I.seeElement({ react: "Backdrop", props: { visible: true } });
  I.seeElement({ react: "DropDown", props: { className: "profile-menu" } });

  I.saveScreenshot(`7.profile-menu.png`);
  if (!isModel) {
    I.seeVisualDiff(`7.profile-menu.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario("Test list of actions render", async ({ I }) => {
  I.mockEndpoint(Endpoints.group, "empty");
  I.mockEndpoint(Endpoints.filter, "empty");
  I.openPage();

  if (deviceType !== "desktop") I.openArticle();
  I.clickArticleMainButton();
  I.see("User");
  I.see("Guest");
  I.see("Group");
  I.see("Invitation link");

  I.saveScreenshot(`8.article-main-button-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`8.article-main-button-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario("Test context menu render", async ({ I }) => {
  I.mockEndpoint(Endpoints.filter, "one");
  I.mockEndpoint(Endpoints.group, "empty");
  I.openPage();

  I.openContextMenu();
  I.see("Send email");
  I.see("Edit");
  I.see("Change password");
  I.see("Change email");

  I.saveScreenshot(`9.context-menu.png`);
  if (!isModel) {
    I.seeVisualDiff(`9.context-menu.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

//  Modal Tests //

Scenario("Modal test - Create invitation link", async ({ I }) => {
  I.mockEndpoint(Endpoints.group, "empty");
  I.mockEndpoint(Endpoints.filter, "empty");

  I.openPage();

  I.click({
    react: "ContextMenuButton",
    props: { className: "action-button" },
  });
  I.wait(1);

  I.see("Create invitation link");
  I.click({
    react: "DropDownItem",
    props: { label: "Create invitation link" },
  });
  I.wait(1);

  I.saveScreenshot(`10.modal-create-invitation-link.png`);
  if (!isModel) {
    I.seeVisualDiff(`10.modal-create-invitation-link.png`, {
      tolerance: 1,
      prepareBaseImage: true,
    });
  }
});

Scenario("Modal test - Change email", async ({ I }) => {
  I.mockEndpoint(Endpoints.group, "empty");
  I.mockEndpoint(Endpoints.filter, "one");

  I.openPage();

  I.click({
    react: "ContextMenuButton",
    props: { className: "expandButton" },
  });
  I.wait(2);

  I.click("Change email");
  I.wait(1);

  I.saveScreenshot(`11.modal-email-change.png`);
  if (!isModel) {
    I.seeVisualDiff(`11.modal-email-change.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario("Modal test - Change password", async ({ I }) => {
  I.mockEndpoint(Endpoints.group, "empty");
  I.mockEndpoint(Endpoints.filter, "one");

  I.openPage();

  I.click({
    react: "ContextMenuButton",
    props: { className: "expandButton" },
  });
  I.wait(2);

  I.click("Change password");
  I.wait(1);

  I.saveScreenshot(`12.modal-password-change.png`);
  if (!isModel) {
    I.seeVisualDiff(`12.modal-password-change.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario("Modal test - Data loss warning", async ({ I }) => {
  I.mockEndpoint(Endpoints.group, "empty");
  I.mockEndpoint(Endpoints.filter, "one");

  I.openPage();

  I.click({
    react: "ContextMenuButton",
    props: { className: "expandButton" },
  });
  I.wait(2);

  I.click("Edit");
  I.wait(5);
});

Scenario("Modal test - Send invite link again", async ({ I }) => {
  I.mockEndpoint(Endpoints.group, "empty");
  I.mockEndpoint(Endpoints.filter, "many");

  I.openPage();
});

Scenario("Modal test - Backup codes", async ({ I }) => {
  I.mockEndpoint(Endpoints.group, "empty");
  I.mockEndpoint(Endpoints.filter, "one");

  I.openPage();

  I.click({
    react: "Avatar",
    role: "owner",
  });
  I.wait(2);

  I.click("Profile");
  I.wait(2);
});

Scenario("Modal test - Change", async ({ I }) => {
  I.mockEndpoint(Endpoints.group, "empty");
  I.mockEndpoint(Endpoints.filter, "one");

  I.openPage();

  I.click({
    react: "Checkbox",
    role: "owner",
  });
});
