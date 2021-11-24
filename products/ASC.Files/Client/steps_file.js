const Endpoints = require('./tests/mocking/endpoints.js');
// in this file you can append custom step methods to 'I' object

module.exports = function () {
  return actor({
    // Define custom steps here, use 'this' to access default methods of I.
    // It is recommended to place a general 'login' function here.

    mockData: function () {
      this.mockEndpoint(Endpoints.self, 'self');
      this.mockEndpoint(Endpoints.settings, 'settings');
      this.mockEndpoint(Endpoints.build, 'build');
      this.mockEndpoint(Endpoints.info, 'info');
      this.mockEndpoint(Endpoints.common, 'common');
      this.mockEndpoint(Endpoints.cultures, 'cultures');
      this.mockEndpoint(Endpoints.fileSettings, 'default');
      this.mockEndpoint(Endpoints.capabilities, 'capabilities');
      this.mockEndpoint(Endpoints.thirdparty, 'thirdparty');
      this.mockEndpoint(Endpoints.thumbnails, 'thumbnails');
    },
    openArticle: function () {
      this.seeElement({ react: 'SectionToggler', props: { visible: true } });
      this.click({ react: 'SectionToggler', props: { visible: true } });
    },

    openProfileMenu: function () {
      this.seeElement({ react: 'Avatar', props: { className: 'icon-profile-menu' } });
      this.click({ react: 'Avatar', props: { className: 'icon-profile-menu' } });
    },

    openContextMenu: function () {
      this.seeElement({ react: 'ContextMenuButton', props: { className: 'expandButton' } });
      this.click({ react: 'ContextMenuButton', props: { className: 'expandButton' } });
    },

    switchView: function (type) {
      this.click({ react: 'styled.div', key: type, props: { isChecked: false } });
      this.wait(2);
    },
  });
};
