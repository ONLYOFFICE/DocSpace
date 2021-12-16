const Endpoints = require('./mocking/endpoints.js');
const json = require('./mocking/mock-data/settings/cultures.json');

const cultures = json.response.map((item) => item.split('-')[1]);

const browser = process.env.profile || 'chromium';
const deviceType = process.env.DEVICE_TYPE || 'desktop';
const isModel = !!process.env.MODEL;

for (const culture of cultures) {
  const featureName = isModel ? `Files translation(model)` : `${culture}`;

  Feature(featureName);

  Before(async ({ I }) => {
    I.mockData();
  });

  Scenario(`Main page tests ${culture}`, ({ I }) => {
    I.mockEndpoint(Endpoints.root, 'empty');
    I.mockEndpoint(Endpoints.my, 'default');

    if (!isModel && culture !== 'US') {
      I.mockEndpoint(Endpoints.self, `self${culture}`);
      I.mockEndpoint(Endpoints.settings, `settings${culture}`);
    }

    I.amOnPage('/products/files');
    I.wait(5);

    I.see('My documents');

    I.saveScreenshot(`${culture}-main-page.png`);
    I.seeVisualDiff(`${culture}-main-page.png`, {
      tolerance: 0.04,
      prepareBaseImage: false,
      // ignoredBox: { top: 0, left: 0, bottom: 0, right: 1720 },
    });
  });

  Scenario(`Profile menu tests ${culture}`, ({ I }) => {
    I.mockEndpoint(Endpoints.root, 'empty');
    I.mockEndpoint(Endpoints.my, 'default');

    if (!isModel && culture !== 'US') {
      I.mockEndpoint(Endpoints.self, `self${culture}`);
      I.mockEndpoint(Endpoints.settings, `settings${culture}`);
    }

    I.amOnPage('/products/files');
    I.wait(5);

    I.openProfileMenu();

    I.saveScreenshot(`${culture}-profile-menu.png`);
    I.seeVisualDiff(`${culture}-profile-menu.png`, {
      tolerance: 0.05,
      prepareBaseImage: false,
    });
  });

  Scenario(`Main button tests ${culture}`, ({ I }) => {
    I.mockEndpoint(Endpoints.root, 'empty');
    I.mockEndpoint(Endpoints.my, 'default');

    if (!isModel && culture !== 'US') {
      I.mockEndpoint(Endpoints.self, `self${culture}`);
      I.mockEndpoint(Endpoints.settings, `settings${culture}`);
    }

    I.amOnPage('/products/files');
    I.wait(5);

    I.seeElement({ react: 'ArticleMainButton' });
    I.click({ react: 'ArticleMainButton' });

    I.saveScreenshot(`${culture}-main-button.png`);
    I.seeVisualDiff(`${culture}-main-button.png`, {
      tolerance: 0,
      prepareBaseImage: false,
      ignoredBox: { top: 0, left: 256, bottom: 0, right: 0 },
    });
  });

  Scenario(`Select fields tests ${culture}`, ({ I }) => {
    I.mockEndpoint(Endpoints.root, 'empty');
    I.mockEndpoint(Endpoints.my, 'default');

    if (!isModel && culture !== 'US') {
      I.mockEndpoint(Endpoints.self, `self${culture}`);
      I.mockEndpoint(Endpoints.settings, `settings${culture}`);
    }

    I.amOnPage('/products/files');
    I.wait(5);

    I.seeElement({ react: 'TableSettings' });
    I.click({ react: 'TableSettings' });

    I.saveScreenshot(`${culture}-table-settings.png`);
    I.seeVisualDiff(`${culture}-table-settings.png`, {
      tolerance: 0.07,
      prepareBaseImage: false,
      ignoredBox: { top: 0, left: 0, bottom: 0, right: 1770 },
    });
  });
}
