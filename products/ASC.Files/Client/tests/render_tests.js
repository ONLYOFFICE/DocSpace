const Endpoints = require('./mocking/endpoints.js');

const browser = process.env.profile || 'chromium';
const deviceType = process.env.DEVICE_TYPE || 'desktop';
const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Files render on '${browser}' with '${deviceType}' dimension (model)`
  : `Files render on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

Before(async ({ I }) => {
  I.mockData();
});

Scenario('Default root folders render test', ({ I }) => {
  I.mockEndpoint(Endpoints.root, 'empty');
  I.mockEndpoint(Endpoints.my, 'default');
  I.mockEndpoint(Endpoints.getFolder(3), 'emptyShared');
  I.mockEndpoint(Endpoints.getFolder(4), 'emptyFavorites');
  I.mockEndpoint(Endpoints.getFolder(5), 'emptyRecent');
  I.mockEndpoint(Endpoints.getFolder(6), 'emptyPrivate');
  I.mockEndpoint(Endpoints.getFolder(7), 'emptyCommon');
  I.mockEndpoint(Endpoints.getFolder(8), 'emptyTrash');

  I.amOnPage('/products/files');
  I.wait(5);

  I.see('My documents');
  I.saveScreenshot(`1.row-default-my-documents-tab.png`);
  if (!isModel) {
    I.seeVisualDiff(`1.row-default-my-documents-tab.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== 'mobile') {
    I.switchView('tile');
    I.saveScreenshot(`1.tile-default-my-documents-tab.png`);
    if (!isModel) {
      I.seeVisualDiff(`1.tile-default-my-documents-tab.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }
  }

  if (deviceType !== 'desktop') I.openArticle();
  I.click('Shared with me');
  I.wait(2);
  I.see('Shared with me');
  I.saveScreenshot(`2.empty-shared-with-me-tab.png`);
  if (!isModel) {
    I.seeVisualDiff(`2.empty-shared-with-me-tab.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== 'desktop') I.openArticle();
  I.click('Favorites');
  I.wait(2);
  I.see('Favorites');
  I.saveScreenshot(`3.empty-favorites-tab.png`);
  if (!isModel) {
    I.seeVisualDiff(`3.empty-favorites-tab.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== 'desktop') I.openArticle();
  I.click('Recent');
  I.wait(2);
  I.see('Recent');
  I.saveScreenshot(`4.empty-recent-tab.png`);
  if (!isModel) {
    I.seeVisualDiff(`4.empty-recent-tab.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== 'desktop') I.openArticle();
  I.click('Private Room');
  I.wait(2);
  I.see('Private Room');
  I.saveScreenshot(`5.empty-private-room-tab.png`);
  if (!isModel) {
    I.seeVisualDiff(`5.empty-private-room-tab.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== 'desktop') I.openArticle();
  I.click('Common');
  I.wait(2);
  I.see('Common');
  I.saveScreenshot(`6.empty-common-tab.png`);
  if (!isModel) {
    I.seeVisualDiff(`6.empty-common-tab.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== 'desktop') I.openArticle();
  I.click('Trash');
  I.wait(2);
  I.see('Trash');
  I.saveScreenshot(`7.empty-trash-tab.png`);
  if (!isModel) {
    I.seeVisualDiff(`7.empty-trash-tab.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }
});

Scenario('Default settings render test', ({ I }) => {
  I.mockEndpoint(Endpoints.root, 'empty');
  I.mockEndpoint(Endpoints.my, 'default');
  I.amOnPage('/products/files');
  I.wait(5);

  if (deviceType !== 'desktop') I.openArticle();
  I.click({ react: 'ContextTreeNode', props: { eventKey: 'settings' } });
  I.see('Common settings');
  I.see('Admin settings');
  I.see('Connected clouds');

  I.click('Common settings');
  if (deviceType !== 'desktop') I.forceClick('.backdrop-active');
  I.see('Common settings');
  I.saveScreenshot(`8.default-common-settings-tab.png`);
  if (!isModel) {
    I.seeVisualDiff(`8.default-common-settings-tab.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== 'desktop') I.openArticle();
  I.click('Admin settings');
  if (deviceType !== 'desktop') I.forceClick('.backdrop-active');
  I.see('Admin settings');
  I.saveScreenshot(`9.default-admin-settings-tab.png`);
  if (!isModel) {
    I.seeVisualDiff(`9.default-admin-settings-tab.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== 'desktop') I.openArticle();
  I.click('Connected clouds');
  if (deviceType !== 'desktop') I.forceClick('.backdrop-active');
  I.see('Connected clouds');
  I.saveScreenshot(`10.default-connected-clouds-tab.png`);
  if (!isModel) {
    I.seeVisualDiff(`10.default-connected-clouds-tab.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }
});

Scenario('Many documents and folders render test', ({ I }) => {
  I.mockEndpoint(Endpoints.root, 'one');
  I.mockEndpoint(Endpoints.my, 'many');
  I.mockEndpoint(Endpoints.getFolder(9), '9');
  I.mockEndpoint(Endpoints.getSubfolder(9), '9');
  I.amOnPage('/products/files');
  I.wait(5);

  I.click('New folder');
  I.see('New folder');

  I.saveScreenshot(`11.row-many-documents.png`);
  if (!isModel) {
    I.seeVisualDiff(`11.row-many-documents.png`, {
      tolerance: 3,
      prepareBaseImage: false,
    });
  }

  if (deviceType !== 'mobile') {
    I.switchView('tile');
    I.saveScreenshot(`11.tile-many-documents.png`);
    if (!isModel) {
      I.seeVisualDiff(`11.tile-many-documents.png`, {
        tolerance: 3,
        prepareBaseImage: false,
      });
    }
  }
});
