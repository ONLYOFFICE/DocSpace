const Endpoints = require('./tests/mocking/endpoints.js');

// in this file you can append custom step methods to 'I' object

module.exports = function () {
  return actor({
    // Define custom steps here, use 'this' to access default methods of I.
    // It is recommended to place a general 'login' function here.

    mockData: function () {
      this.mockEndpoint(Endpoints.settings, 'settings');
      this.mockEndpoint(Endpoints.build, 'build');
      this.mockEndpoint(Endpoints.common, 'common');
      this.mockEndpoint(Endpoints.password, 'password');
      this.mockEndpoint(Endpoints.info, 'info');
      this.mockEndpoint(Endpoints.self, 'self');
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

    clickArticleMainButton: function () {
      this.seeElement({ react: 'ArticleMainButton' });
      this.click({ react: 'ArticleMainButton' });
    },

    fillForm: function (form) {
      this.click({ react: 'RadioButton', props: { name: 'passwordType', isChecked: false } });
      this.fillField('firstName', form.firstName);
      this.fillField('lastName', form.lastName);
      this.fillField('email', form.email);
      this.fillField('password', form.password);
      this.click({ react: 'Button', props: { label: 'Save' } });
    },

    checkText: function (form, type) {
      this.wait(3);
      this.see('Done');
      this.see('Changes saved successfully');
      this.see(`${form.firstName} ${form.lastName}`);
      this.see('Edit profile');
      this.see(type);
      this.see(form.email);
      this.see('Male');
      this.see('11/17/2021');
    },
  });
};
