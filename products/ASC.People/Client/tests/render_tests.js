const Endpoints = require('./mocking/endpoints.js');

const browser = process.env.profile || 'chromium';
const deviceType = process.env.DEVICE_TYPE || 'desktop';
const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Render people on '${browser}' with '${deviceType}' dimension (model)`
  : `Render people on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

// doing it before others scenario
Before(async ({ I }) => {
  I.mockData();
});

Scenario('Test empty people and group lists render', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'empty');
  I.mockEndpoint(Endpoints.filter, 'empty');
  I.openPage();

  I.see('No teammates found');

  I.saveScreenshot(`1.empty-people-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`1.empty-people-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== 'desktop') {
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

Scenario('Test one person and group lists render', async ({ I }) => {
  I.mockEndpoint(Endpoints.filter, 'one');
  I.mockEndpoint(Endpoints.group, 'one');
  I.openPage();

  I.saveScreenshot(`3.one-person-people-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`3.one-person-people-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== 'desktop') {
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

Scenario('Test many people and groups lists render', async ({ I }) => {
  I.mockEndpoint(Endpoints.filter, 'many');
  I.mockEndpoint(Endpoints.group, 'many');
  I.openPage();

  I.saveScreenshot(`5.many-people-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`5.many-people-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== 'desktop') {
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

Scenario('Test profile menu render', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'empty');
  I.mockEndpoint(Endpoints.filter, 'empty');
  I.openPage();

  I.openProfileMenu();

  I.seeElement({ react: 'Backdrop', props: { visible: true } });
  I.seeElement({ react: 'DropDown', props: { className: 'profile-menu' } });

  I.saveScreenshot(`7.profile-menu.png`);
  if (!isModel) {
    I.seeVisualDiff(`7.profile-menu.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario('Test list of actions render', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'empty');
  I.mockEndpoint(Endpoints.filter, 'empty');
  I.openPage();

  if (deviceType !== 'desktop') I.openArticle();
  I.clickArticleMainButton();
  I.see('User');
  I.see('Guest');
  I.see('Group');
  I.see('Invitation link');

  I.saveScreenshot(`8.article-main-button-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`8.article-main-button-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario('Test context menu render', async ({ I }) => {
  I.mockEndpoint(Endpoints.filter, 'one');
  I.mockEndpoint(Endpoints.group, 'empty');
  I.openPage();

  I.openContextMenu();
  I.see('Send email');
  I.see('Edit');
  I.see('Change password');
  I.see('Change email');

  I.saveScreenshot(`9.context-menu.png`);
  if (!isModel) {
    I.seeVisualDiff(`9.context-menu.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});
