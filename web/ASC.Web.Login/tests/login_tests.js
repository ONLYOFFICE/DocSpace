const Endpoints = require('./mocking/endpoints.js');

const browser = process.env.profile || 'chromium';
const deviceType = process.env.DEVICE_TYPE || 'desktop';

Feature(`Login Page on '${browser}' with '${deviceType}' dimension`);

// doing it before others scenario
Before(async ({ I }) => {
  I.mockEndpoint(Endpoints.settings, 'settings');
  I.mockEndpoint(Endpoints.build, 'build');
  I.mockEndpoint(Endpoints.providers, 'providers');
  I.amOnPage('/login');
});

Scenario('Test login page components render', async ({ I }) => {
  I.seeElement({
    react: 'Text',
  });
  I.seeElement({
    react: 'TextInput',
    props: {
      id: 'login',
      name: 'login',
      type: 'email',
      autoComplete: 'username',
    },
  });
  I.seeElement({
    react: 'PasswordInput',
    props: {
      id: 'password',
      inputName: 'password',
      type: 'password',
      autoComplete: 'current-password',
    },
  });
  I.seeElement({
    react: 'Checkbox',
    props: {
      className: 'login-checkbox',
      isChecked: false,
    },
  });
  I.seeElement({
    react: 'HelpButton',
    props: {
      className: 'login-tooltip',
    },
  });
  I.seeElement({
    react: 'Link',
    props: {
      className: 'login-link',
      type: 'page',
    },
  });
  I.seeElement({
    react: 'Button',
    props: {
      className: 'login-button',
      type: 'page',
    },
  });
  I.seeElement({
    react: 'FacebookButton',
    props: {
      className: 'socialButton',
      iconName: '/static/images/share.facebook.react.svg',
    },
  });
  I.see('Sign in with Google');
  I.see('Sign in with LinkedIn');
  I.seeElement({ react: 'Register' });
  I.saveScreenshot(`1.login.${browser}.${deviceType}.png`);
  I.seeVisualDiff(`1.login.${browser}.${deviceType}.png`, {
    tolerance: 1,
    prepareBaseImage: false,
  });
}).retry(1);

Scenario('Test checkbox click', async ({ I }) => {
  I.seeElement({
    react: 'Checkbox',
    props: {
      className: 'login-checkbox',
      isChecked: false,
    },
  });
  I.click({ react: 'Checkbox' });
  I.seeElement({
    react: 'Checkbox',
    props: {
      className: 'login-checkbox',
      isChecked: true,
    },
  });
  I.saveScreenshot(`2.login.${browser}.${deviceType}.png`);
  I.seeVisualDiff(`2.login.${browser}.${deviceType}.png`, {
    tolerance: 1,
    prepareBaseImage: false,
  });
}).retry(1);

Scenario('Test reset mail send', async ({ I }) => {
  I.seeElement({
    react: 'Link',
    props: {
      className: 'login-link',
      type: 'page',
    },
  });
  I.click({ react: 'Link' });
  I.see('Password recovery');
  I.see(
    'Please, enter the email you used for registration. The password recovery instructions will be sent to it.',
  );
  I.seeElement({
    react: 'TextInput',
  });
  I.see('Send');
  I.saveScreenshot(`3.login.${browser}.${deviceType}.png`);
  I.seeVisualDiff(`3.login.${browser}.${deviceType}.png`, {
    tolerance: 1,
    prepareBaseImage: false,
  });
  I.fillField('e-mail', 'test@mail.ru');
}).retry(1);

Scenario('Test registration', async ({ I }) => {
  I.click({ react: 'Register' });
  I.see('Registration request');
  I.see(
    'To register, enter your email and click Send request. An activation link will be sent to you.',
  );
  I.seeElement({
    react: 'TextInput',
  });
  I.see('Send request');
  I.saveScreenshot(`4.login.${browser}.${deviceType}.png`);
  I.seeVisualDiff(`4.login.${browser}.${deviceType}.png`, {
    tolerance: 1,
    prepareBaseImage: false,
  });
  I.fillField('e-mail', 'test@mail.ru');
}).retry(1);

Scenario('Test help button click', async ({ I }) => {
  I.seeElement({
    react: 'HelpButton',
    props: {
      className: 'login-tooltip',
    },
  });
  I.click({
    react: 'HelpButton',
    props: {
      className: 'login-tooltip',
    },
  });
  I.see(
    'The default session lifetime is 20 minutes. Check this option to set it to 1 year. To set your own value, go to Settings',
  );
  I.saveScreenshot(`5.login.${browser}.${deviceType}.png`);
  I.seeVisualDiff(`5.login.${browser}.${deviceType}.png`, {
    tolerance: 1,
    prepareBaseImage: false,
  });
}).retry(1);

Scenario('Test login error', async ({ I }) => {
  I.mockEndpoint(Endpoints.auth, 'authError');
  I.click({
    react: 'Button',
    props: {
      className: 'login-button',
      type: 'page',
    },
  });
  I.see('Required field');
  I.fillField('login', 'test@mail.ru');
  I.click({
    react: 'Button',
    props: {
      className: 'login-button',
      type: 'page',
    },
  });
  I.see('Required field');
  I.fillField('password', secret('0000000'));
  I.click({
    react: 'Button',
    props: {
      className: 'login-button',
      type: 'page',
    },
  });
  I.see('User authentication failed');
}).retry(1);

Scenario('Test login success', async ({ I }) => {
  I.mockEndpoint(Endpoints.people, 'self');
  I.mockEndpoint(Endpoints.modules, 'info');
  I.mockEndpoint(Endpoints.auth, 'authSuccess');
  I.fillField('login', 'test@mail.ru');
  I.fillField('password', secret('12345678'));
  I.click({
    react: 'Button',
    props: {
      className: 'login-button',
      type: 'page',
    },
  });
  I.seeElement({
    react: 'Home',
  });
}).retry(1);
