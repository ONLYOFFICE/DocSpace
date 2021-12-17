const Endpoints = require('./mocking/endpoints.js');
const json = require('./mocking/mock-data/settings/cultures.json');
const changeCulture = require('./helpers/changeCulture.js');

const cultures = json.response;

const isModel = !!process.env.MODEL;

for (const culture of cultures) {
  const featureName = isModel ? `People translation(model)` : `${culture}`;

  Feature(featureName);

  Before(async ({ I }) => {
    I.mockData();
    changeCulture(culture);
  });

  Scenario(`Main page tests ${culture}`, ({ I }) => {
    I.mockEndpoint(Endpoints.filter, 'many');
    I.mockEndpoint(Endpoints.group, 'many');

    if (!isModel) {
      I.mockEndpoint(Endpoints.self, `selfTranslation`);
      I.mockEndpoint(Endpoints.settings, `settingsTranslation`);
    }

    I.openPage();

    I.saveScreenshot(`${culture}-main-page.png`);
    I.seeVisualDiff(`${culture}-main-page.png`, {
      tolerance: 0.04,
      prepareBaseImage: false,
      // ignoredBox: { top: 0, left: 0, bottom: 0, right: 1720 },
    });
  });

  Scenario(`Main button tests ${culture}`, ({ I }) => {
    I.mockEndpoint(Endpoints.filter, 'many');
    I.mockEndpoint(Endpoints.group, 'many');

    if (!isModel) {
      I.mockEndpoint(Endpoints.self, `self`);
      I.mockEndpoint(Endpoints.settings, `settings`);
    }

    I.openPage();

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
    I.mockEndpoint(Endpoints.filter, 'many');
    I.mockEndpoint(Endpoints.group, 'many');

    if (!isModel) {
      I.mockEndpoint(Endpoints.self, `self`);
      I.mockEndpoint(Endpoints.settings, `settings`);
    }

    I.openPage();

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
