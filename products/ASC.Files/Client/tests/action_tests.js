const Endpoints = require('./mocking/endpoints.js');

const browser = process.env.profile || 'chromium';
const deviceType = process.env.DEVICE_TYPE || 'desktop';
const isModel = !!process.env.MODEL;

const featureName = isModel
  ? `Files actions on '${browser}' with '${deviceType}' dimension (model)`
  : `Files actions on '${browser}' with '${deviceType}' dimension`;

const renamedFolderTitle = 'Renamed folder';

Feature(featureName);

Before(async ({ I }) => {
  I.mockData();
});

Scenario('Rename file test', ({ I }) => {
  I.mockEndpoint(Endpoints.root, 'one');
  I.mockEndpoint(Endpoints.my, 'many');
  I.mockEndpoint(Endpoints.getFolder(9), '9');
  I.mockEndpoint(Endpoints.getSubfolder(9), '9');
  I.checkRequest(
    `http://localhost:8092/api/2.0/files/folder/16`,
    { title: renamedFolderTitle },
    'files/folder',
    '16',
  );
  I.amOnPage('/products/files');
  I.wait(5);

  I.click('New folder');
  I.see('New folder');

  I.openContextMenu();
  I.see('Rename');
  I.click('Rename');
  I.type(renamedFolderTitle);
  I.pressKey('Enter');
  I.see('Done');
});

Scenario('Add and remove from favorites file test', ({ I }) => {
  I.mockEndpoint(Endpoints.root, 'empty');
  I.mockEndpoint(Endpoints.my, 'default');
  I.mockEndpoint(Endpoints.getFile(5), 'addFavorite');
  I.checkRequest(
    `http://localhost:8092/api/2.0/files/favorites`,
    { id: '5' },
    'files/favorites',
    'default',
  );
  I.amOnPage('/products/files');
  I.wait(5);

  I.openContextMenu();
  I.see('Mark as favorite');
  I.click('Mark as favorite');

  I.mockEndpoint(Endpoints.getFile(5), 'removeFavorite');

  I.openContextMenu();
  I.see('Remove from favorites');
  I.click('Remove from favorites');
});
