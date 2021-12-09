module.exports = class Endpoints {
  static self = {
    url: [
      'http://localhost:8092/api/2.0/people/@self.json',
      'http://localhost:8092/api/2.0/people/%40self.json',
    ],
    method: 'GET',
    baseDir: 'people',
  };
  static info = {
    url: ['http://localhost:8092/api/2.0/modules/info'],
    method: 'GET',
    baseDir: 'modules',
  };
  static build = {
    url: ['http://localhost:8092/api/2.0/settings/version/build.json'],
    method: 'GET',
    baseDir: 'settings',
  };
  static settings = {
    url: ['http://localhost:8092/api/2.0/settings.json'],
    method: 'GET',
    baseDir: 'settings',
  };
  static group = {
    url: ['http://localhost:8092/api/2.0/group'],
    method: 'GET',
    baseDir: 'groups',
  };
  static filter = {
    url: ['http://localhost:8092/api/2.0/people/filter.json'],
    method: 'GET',
    baseDir: 'filters',
  };
  static password = {
    url: ['http://localhost:8092/api/2.0/settings/security/password'],
    method: 'GET',
    baseDir: 'settings',
  };
  static common = {
    url: ['http://localhost:8092/api/2.0/settings/customschemas/Common.json'],
    method: 'GET',
    baseDir: 'settings',
  };
  static providers = {
    url: ['http://localhost:8092/api/2.0/people/thirdparty/providers'],
    method: 'GET',
    baseDir: 'people',
  };
  static createUser = {
    url: ['http://localhost:8092/api/2.0/people/TestName.json'],
    method: 'GET',
    baseDir: 'people',
  };
  static cultures = {
    url: ['http://localhost:8092/api/2.0/settings/cultures.json'],
    method: 'GET',
    baseDir: 'settings',
  };
};
