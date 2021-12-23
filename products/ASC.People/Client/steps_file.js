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
      this.mockEndpoint(Endpoints.cultures, 'cultures');
    },

    openPage: function () {
      this.amOnPage('/products/people');
      this.refreshPage();
      this.wait(5);
    },

    openArticle: function () {
      this.seeElement({
        react: 'styled.div',
        props: { className: 'not-selectable', visible: true },
      });
      this.click({ react: 'styled.div', props: { className: 'not-selectable', visible: true } });
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
      this.wait(3);
    },

    addHeadInNewGroup: function () {
      this.click('Select');
      this.click('Administrator1');
    },

    addUserInNewGroup: function () {
      this.click('#users-selector_button');
      this.click({ react: 'Checkbox', props: { value: '2' } });
      this.click('Add members');
    },

    fillForm: function (form) {
      this.click({ react: 'RadioButton', props: { name: 'passwordType', isChecked: false } });
      this.fillField('firstName', form.firstName);
      this.fillField('lastName', form.lastName);
      this.fillField('email', form.email);
      this.fillField('password', form.password);
      this.click({ react: 'div', props: { className: 'append' } });
      this.click({ react: 'ComboButton', props: { optionsLength: 12 } });
      this.click({ react: 'DropDownItem', props: { label: 'January' } });
      this.click({ react: 'ComboButton', props: { optionsLength: 122 } });
      this.click({ react: 'DropDownItem', props: { label: '2020' } });
      this.click('.calendar-month_weekend');
      this.seeElement({ react: 'InputBlock', props: { value: '01/04/2020' } });
      this.fillField('location', form.location);
      this.fillField('title', 'Test title');
      this.fillField('notes', form.notes);
      this.click({ react: 'Button', props: { label: 'Save' } });
    },

    checkText: function (form, type) {
      this.wait(10);
      this.see(`${form.firstName} ${form.lastName}`);
      this.see('Edit profile');
      this.see(type);
      this.see(form.email);
      this.see('Male');
      this.see('1/16/2020');
      this.see('1/15/2021');
    },
  });
};
