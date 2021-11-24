const Endpoints = require('./mocking/endpoints.js');

const browser = process.env.profile || 'chromium';
const deviceType = process.env.DEVICE_TYPE || 'desktop';

const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Create screenshots actions model on '${browser}' with '${deviceType}' dimension (actions)`
  : `People actions on '${browser}' with '${deviceType}' dimension`;

const form = {
  firstName: 'TestName',
  lastName: 'TestSurname',
  email: 'test123@mail.ru',
  password: secret('123456789'),
};

const groupName = 'Test group';

Feature(`${featureName}`);

Before(async ({ I }) => {
  I.mockData();
});

Scenario('Create new user test', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'empty');
  I.mockEndpoint(Endpoints.filter, 'empty');
  I.openPage();

  if (deviceType !== 'desktop') I.openArticle();
  I.clickArticleMainButton();
  I.click({ react: 'DropDownItem', props: { label: 'User' } });

  I.checkRequest('http://localhost:8092/api/2.0/people', form, 'people', 'createUser');
  I.mockEndpoint(Endpoints.createUser, 'createUser');
  I.mockEndpoint(Endpoints.filter, 'createUser');

  if (deviceType !== 'desktop') I.forceClick('.backdrop-active');

  I.see('User (creation)');

  I.fillForm(form);

  I.checkText(form, 'User');

  I.saveScreenshot(`10.create-new-user.png`);
  if (!isModel) {
    I.seeVisualDiff(`10.create-new-user.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario('Create new guest test', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'empty');
  I.mockEndpoint(Endpoints.filter, 'empty');
  I.openPage();

  if (deviceType !== 'desktop') I.openArticle();
  I.clickArticleMainButton();
  I.click({ react: 'DropDownItem', props: { label: 'Guest' } });

  I.checkRequest('http://localhost:8092/api/2.0/people', form, 'people', 'createGuest');
  I.mockEndpoint(Endpoints.createUser, 'createGuest');
  I.mockEndpoint(Endpoints.filter, 'createGuest');

  if (deviceType !== 'desktop') I.forceClick('.backdrop-active');

  I.see('Guest (creation)');

  I.fillForm(form);

  I.checkText(form, 'Guest');

  I.saveScreenshot(`11.create-new-guest.png`);
  if (!isModel) {
    I.seeVisualDiff(`11.create-new-guest.png`, {
      tolerance: 1,
      prepareBaseImage: false,
    });
  }
});

Scenario('Create new group test', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'empty');
  I.mockEndpoint(Endpoints.filter, 'many');
  I.openPage();

  if (deviceType !== 'desktop') I.openArticle();
  I.clickArticleMainButton();
  I.click({ react: 'DropDownItem', props: { label: 'Group' } });

  I.checkRequest(
    'http://localhost:8092/api/2.0/group.json',
    { groupName: groupName },
    'groups',
    'createGroup',
  );
  I.mockEndpoint(Endpoints.group, 'createGroup');

  if (deviceType !== 'desktop') I.forceClick('.backdrop-active');

  I.see('Group (creation)');

  I.fillField('group-name', groupName);

  I.addHeadInNewGroup();

  if (deviceType !== 'desktop') I.forceClick('.backdrop-active');

  I.addUserInNewGroup();

  I.click('Add');

  I.waitForText('Done', 3);

  if (deviceType !== 'desktop') I.openArticle();
  I.see(groupName);
});
