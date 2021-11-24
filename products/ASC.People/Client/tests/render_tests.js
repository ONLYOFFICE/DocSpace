const Endpoints = require('./mocking/endpoints.js');

const browser = process.env.profile || 'chromium';
const deviceType = process.env.DEVICE_TYPE || 'desktop';
const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Create screenshots model on '${browser}' with '${deviceType}' dimension`
  : `Render people on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

// doing it before others scenario
Before(async ({ I }) => {
  I.mockData();
});

Scenario('Test empty people list render', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'empty');
  I.mockEndpoint(Endpoints.filter, 'empty');
  I.amOnPage('/products/people');
  I.refreshPage();
  I.waitForText('No teammates found', 5);
  I.see('No teammates found');
  I.saveScreenshot(`1.empty-people-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`1.empty-people-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario('Test one person list render', async ({ I }) => {
  I.mockEndpoint(Endpoints.filter, 'one');
  I.mockEndpoint(Endpoints.group, 'empty');
  I.amOnPage('/products/people');
  I.refreshPage();
  I.wait(5);
  I.saveScreenshot(`2.one-person-people-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`2.one-person-people-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario('Test many people list render', async ({ I }) => {
  I.mockEndpoint(Endpoints.filter, 'many');
  I.mockEndpoint(Endpoints.group, 'empty');
  I.amOnPage('/products/people');
  I.refreshPage();
  I.wait(5);
  I.saveScreenshot(`3.many-people-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`3.many-people-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario('Test empty group list render', async ({ I }) => {
  I.mockEndpoint(Endpoints.filter, 'empty');
  I.mockEndpoint(Endpoints.group, 'empty');
  I.amOnPage('/products/people');
  I.refreshPage();
  I.wait(5);
  if (deviceType !== 'desktop') I.openArticle();
  I.saveScreenshot(`4.empty-group-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`4.empty-group-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario('Test one group list render', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'one');
  I.mockEndpoint(Endpoints.filter, 'empty');
  I.amOnPage('/products/people');
  I.refreshPage();
  I.wait(5);
  if (deviceType !== 'desktop') I.openArticle();
  I.saveScreenshot(`5.one-group-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`5.one-group-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario('Test many groups list render', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'many');
  I.mockEndpoint(Endpoints.filter, 'empty');
  I.amOnPage('/products/people');
  I.refreshPage();
  I.wait(5);
  if (deviceType !== 'desktop') I.openArticle();
  I.saveScreenshot(`6.many-groups-list.png`);
  if (!isModel) {
    I.seeVisualDiff(`6.many-groups-list.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario('Test profile menu render', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'empty');
  I.mockEndpoint(Endpoints.filter, 'empty');
  I.amOnPage('/products/people');
  I.refreshPage();
  I.wait(5);
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
  I.amOnPage('/products/people');
  I.refreshPage();
  I.wait(5);
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
  I.amOnPage('/products/people');
  I.refreshPage();
  I.wait(5);
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
