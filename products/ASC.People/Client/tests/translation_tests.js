const Endpoints = require('./mocking/endpoints.js');
const changeCulture = require('./helpers/changeCulture.js');
const config = require('../../../../config/appsettings.json');
const ignoringCultures = require('./ignoringCultures.json');

const cultures = config.web.cultures.split(',');

const isModel = !!process.env.MODEL;

const featureName = isModel ? `People translation(model) ` : `People translation tests`;

Feature(featureName, { timeout: 90 });

Before(async ({ I }) => {
  I.mockData();
});
for (const culture of cultures) {
  Scenario(`Main page test ${culture}`, { timeout: 30 }, ({ I }) => {
    changeCulture(culture);
    const isExсeption = ignoringCultures.mainPage.indexOf(culture) != -1;

    I.mockEndpoint(Endpoints.filter, 'many');
    I.mockEndpoint(Endpoints.group, 'many');

    if (!isModel || isExсeption) {
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

  Scenario(`Main button test ${culture}`, { timeout: 30 }, ({ I }) => {
    changeCulture(culture);
    const isExсeption = ignoringCultures.mainButton.indexOf(culture) != -1;

    I.mockEndpoint(Endpoints.filter, 'many');
    I.mockEndpoint(Endpoints.group, 'many');

    if (!isModel || isExсeption) {
      I.mockEndpoint(Endpoints.self, `selfTranslation`);
      I.mockEndpoint(Endpoints.settings, `settingsTranslation`);
    }

    I.openPage();

    I.seeElement({ react: 'ArticleMainButton' });
    I.click({ react: 'ArticleMainButton' });

    I.wait(3);

    I.saveScreenshot(`${culture}-main-button.png`);
    I.seeVisualDiff(`${culture}-main-button.png`, {
      tolerance: 0.01,
      prepareBaseImage: false,
      ignoredBox: { top: 0, left: 256, bottom: 0, right: 0 },
    });
  });

  Scenario(`Table settings test ${culture}`, { timeout: 30 }, async ({ I }) => {
    changeCulture(culture);
    const isExсeption = ignoringCultures.tableSettings.indexOf(culture) != -1;

    await I.mockEndpoint(Endpoints.filter, 'many');
    await I.mockEndpoint(Endpoints.group, 'many');

    if (!isModel || isExсeption) {
      await I.mockEndpoint(Endpoints.self, `selfTranslation`);
      await I.mockEndpoint(Endpoints.settings, `settingsTranslation`);
    }

    I.openPage();

    I.seeElement({ react: 'TableSettings' });
    I.click({ react: 'TableSettings' });

    I.wait(3);

    I.saveScreenshot(`${culture}-table-settings.png`);
    await I.seeVisualDiff(`${culture}-table-settings.png`, {
      tolerance: 0.07,
      prepareBaseImage: false,
      ignoredBox: { top: 0, left: 0, bottom: 0, right: 1770 },
    });
  });
}
