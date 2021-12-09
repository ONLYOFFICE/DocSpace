const Helper = require('@codeceptjs/helper');

const path = require('path');
const assert = require('assert');

class PlaywrightHelper extends Helper {
  // before/after hooks
  /**
   * @protected
   */
  _before() {
    const { page } = this.helpers.Playwright;

    // clear all routes between tests
    page._routes = [];
  }

  // add custom methods here
  // If you need to access other helpers
  // use: this.helpers['helperName']
  async mockEndpoint(endpoint, scenario) {
    const { page } = this.helpers.Playwright;
    const rootDir = 'tests/mocking/mock-data/';
    endpoint.url.forEach(async (url, index) => {
      await page.route(new RegExp(url), (route) => {
        if (scenario !== '') {
          route.fulfill({
            path: path.resolve(rootDir, endpoint.baseDir, `${scenario}.json`),
            headers: {
              'content-type': 'application/json',
              'access-control-allow-origin': '*',
            },
          });
        } else {
          route.fulfill();
        }
      });
    });
  }

  async checkRequest(url, form, baseDir, scenario) {
    const { page } = this.helpers.Playwright;
    const rootDir = 'tests/mocking/mock-data/';
    await page.route(new RegExp(url), (route) => {
      for (let key in form) {
        if (typeof form[key] === 'string') {
          assert(route.request().postDataJSON()[key] === form[key]);
        } else {
          assert(JSON.stringify(route.request().postDataJSON()[key]) === JSON.stringify(form[key]));
        }
      }

      return route.fulfill({
        path: path.resolve(rootDir, baseDir, `${scenario}.json`),
        headers: {
          'content-type': 'application/json',
          'access-control-allow-origin': '*',
        },
      });
    });
  }
}

module.exports = PlaywrightHelper;
