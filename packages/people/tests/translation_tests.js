const Endpoints = require("./mocking/endpoints.js");
const changeCulture = require("./helpers/changeCulture.js");
const config = require("../../../../config/appsettings.json");
const ignoringCultures = require("./ignoringCultures.json");

const isModel = !!process.env.MODEL;

const cultures = isModel ? ["en"] : config.web.cultures.split(",");

const isPersonal = !!process.env.PERSONAL;

const settingsFile = isPersonal ? `settingsPersonal` : `settings`;

const settingsTranslationFile = isPersonal
  ? `settingsTranslationPersonal`
  : `settingsTranslation`;

const featureName = isModel
  ? `People translation(model) `
  : `People translation tests`;

Feature(featureName, { timeout: 90 });

Before(async ({ I }) => {
  I.mockData();
  if (isPersonal) {
    I.mockEndpoint(Endpoints.settings, settingsFile);
    I.mockEndpoint(Endpoints.subscription, "subscription");
    I.mockEndpoint(Endpoints.tfaapp, "tfaapp");
  }
});

for (const culture of cultures) {
  if (!isPersonal) {
    Scenario(`Main page test ${culture}`, { timeout: 30 }, ({ I }) => {
      changeCulture(culture, isPersonal);
      const isException = ignoringCultures.mainPage.indexOf(culture) != -1;

      I.mockEndpoint(Endpoints.filter, "many");
      I.mockEndpoint(Endpoints.group, "many");

      if (!isException) {
        I.mockEndpoint(Endpoints.self, `selfTranslation`);
        I.mockEndpoint(Endpoints.settings, settingsTranslationFile);
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
      changeCulture(culture, isPersonal);
      const isException = ignoringCultures.mainButton.indexOf(culture) != -1;

      I.mockEndpoint(Endpoints.filter, "many");
      I.mockEndpoint(Endpoints.group, "many");

      if (!isException) {
        I.mockEndpoint(Endpoints.self, `selfTranslation`);
        I.mockEndpoint(Endpoints.settings, settingsTranslationFile);
      }

      I.openPage();

      I.seeElement({ react: "MainButton" });
      I.click({ react: "MainButton" });

      I.wait(3);

      I.saveScreenshot(`${culture}-main-button.png`);
      I.seeVisualDiff(`${culture}-main-button.png`, {
        tolerance: 0.32,
        prepareBaseImage: false,
        ignoredBox: { top: 0, left: 256, bottom: 0, right: 0 },
      });
    });

    Scenario(
      `Table settings test ${culture}`,
      { timeout: 30 },
      async ({ I }) => {
        changeCulture(culture, isPersonal);
        const isException =
          ignoringCultures.tableSettings.indexOf(culture) != -1;

        await I.mockEndpoint(Endpoints.filter, "many");
        await I.mockEndpoint(Endpoints.group, "many");

        if (!isException) {
          await I.mockEndpoint(Endpoints.self, `selfTranslation`);
          await I.mockEndpoint(Endpoints.settings, settingsTranslationFile);
        }

        I.openPage();

        I.seeElement({ react: "TableSettings" });
        I.click({ react: "TableSettings" });

        I.wait(3);

        I.saveScreenshot(`${culture}-table-settings.png`);
        await I.seeVisualDiff(`${culture}-table-settings.png`, {
          tolerance: 0.07,
          prepareBaseImage: false,
          ignoredBox: { top: 0, left: 0, bottom: 0, right: 1770 },
        });
      }
    );
  }

  if (isPersonal) {
    Scenario(`Profile page test ${culture}`, { timeout: 30 }, async ({ I }) => {
      changeCulture(culture, isPersonal);
      const isException = ignoringCultures.profilePage.indexOf(culture) != -1;

      await I.mockEndpoint(Endpoints.filter, "many");
      await I.mockEndpoint(Endpoints.group, "many");

      if (!isException) {
        await I.mockEndpoint(Endpoints.self, `selfTranslation`);
        await I.mockEndpoint(Endpoints.settings, settingsTranslationFile);
      }

      I.amOnPage("/my");

      I.wait(3);

      I.saveScreenshot(`${culture}-profile-page.png`);
      await I.seeVisualDiff(`${culture}-profile-page.png`, {
        tolerance: 0.07,
        prepareBaseImage: false,
      });
    });
  }
}
