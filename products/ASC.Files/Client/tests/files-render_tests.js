const Endpoints = require('./mocking/endpoints.js');

const browser = process.env.profile || 'chromium';
const deviceType = process.env.DEVICE_TYPE || 'desktop';
const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Create screenshots model on '${browser}' with '${deviceType}' dimension`
  : `Render files on '${browser}' with '${deviceType}' dimension`;

Feature(featureName);

Before(async ({ I }) => {
  I.mockData();
});

Scenario('My documents render', ({ I }) => {
  I.mockEndpoint(Endpoints.root, 'empty');
  I.mockEndpoint(Endpoints.my, 'default');
  I.amOnPage('/products/files');
  I.wait(5);
  I.see('My documents');
  pause();
  I.saveScreenshot(`1.default-files-list.png`);
});
