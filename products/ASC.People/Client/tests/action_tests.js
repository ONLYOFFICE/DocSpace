const Endpoints = require('./mocking/endpoints.js');

const browser = process.env.profile || 'chromium';
const deviceType = process.env.DEVICE_TYPE || 'desktop';

const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `People actions on '${browser}' with '${deviceType}' dimension (model)`
  : `People actions on '${browser}' with '${deviceType}' dimension`;

const userForm = {
  comment: 'Hello, i am test user!',
  contacts: [],
  department: [],
  email: 'test123@mail.ru',
  firstName: 'TestName',
  groups: [],
  id: '',
  lastName: 'TestSurname',
  location: 'Moscow',
  notes: 'Hello, i am test user!',
  password: '123456789',
  passwordType: 'temp',
  sex: 'male',
  title: 'Test title',
  workFrom: '',
  worksFrom: '',
};

const groupForm = {
  groupName: 'Test group',
  groupManager: 'fcfa85e3-15fc-11ec-8f4f-80ce62334fc71',
  members: ['fcfa85e3-15fc-11ec-8f4f-80ce62334fc72'],
};

Feature(`${featureName}`);

Before(async ({ I }) => {
  I.mockData();
});

Scenario('Create new user test', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'many');
  I.mockEndpoint(Endpoints.filter, 'empty');
  I.openPage();

  if (deviceType !== 'desktop') I.openArticle();
  I.clickArticleMainButton();
  I.click('User');

  I.checkRequest(
    'http://localhost:8092/api/2.0/people',
    { ...userForm, isVisitor: false },
    'people',
    'createUser',
  );
  I.mockEndpoint(Endpoints.createUser, 'createUser');
  I.mockEndpoint(Endpoints.filter, 'createUser');

  if (deviceType !== 'desktop') I.forceClick('.backdrop-active');

  I.see('User (creation)');

  I.fillForm(userForm);

  I.checkText(userForm, 'User');
});

Scenario('Create new guest test', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'many');
  I.mockEndpoint(Endpoints.filter, 'empty');
  I.openPage();

  if (deviceType !== 'desktop') I.openArticle();
  I.clickArticleMainButton();
  I.click('Guest');

  I.checkRequest(
    'http://localhost:8092/api/2.0/people',
    { ...userForm, isVisitor: true },
    'people',
    'createGuest',
  );
  I.mockEndpoint(Endpoints.createUser, 'createGuest');
  I.mockEndpoint(Endpoints.filter, 'createGuest');

  if (deviceType !== 'desktop') I.forceClick('.backdrop-active');

  I.see('Guest (creation)');

  I.fillForm(userForm);

  I.checkText(userForm, 'Guest');
});

Scenario('Create new group test', async ({ I }) => {
  I.mockEndpoint(Endpoints.group, 'empty');
  I.mockEndpoint(Endpoints.filter, 'many');
  I.openPage();

  if (deviceType !== 'desktop') I.openArticle();
  I.clickArticleMainButton();
  I.click('Group');

  I.checkRequest('http://localhost:8092/api/2.0/group.json', groupForm, 'groups', 'createGroup');
  I.mockEndpoint(Endpoints.group, 'createGroup');

  if (deviceType !== 'desktop') I.forceClick('.backdrop-active');

  I.see('Group (creation)');

  I.fillField('group-name', groupForm.groupName);

  I.addHeadInNewGroup();

  if (deviceType !== 'desktop') I.forceClick('.backdrop-active');

  I.addUserInNewGroup();

  I.click('Add');

  I.waitForText('Done', 3);

  if (deviceType !== 'desktop') I.openArticle();
  I.see(groupForm.groupName);
});
