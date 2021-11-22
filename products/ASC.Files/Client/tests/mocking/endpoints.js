module.exports = class Endpoints {
  static self = {
    url: 'http://localhost:8092/api/2.0/people/@self.json',
    method: 'GET',
    baseDir: 'people',
  };
  static info = {
    url: 'http://localhost:8092/api/2.0/modules/info',
    method: 'GET',
    baseDir: 'modules',
  };
  static build = {
    url: 'http://localhost:8092/api/2.0/settings/version/build.json',
    method: 'GET',
    baseDir: 'settings',
  };
  static settings = {
    url: 'http://localhost:8092/api/2.0/settings.json',
    method: 'GET',
    baseDir: 'settings',
  };
  static common = {
    url: 'http://localhost:8092/api/2.0/settings/customschemas/Common.json',
    method: 'GET',
    baseDir: 'settings',
  };
  static cultures = {
    url: 'http://localhost:8092/api/2.0/settings/cultures.json',
    method: 'GET',
    baseDir: 'settings',
  };
  static root = {
    url: 'http://localhost:8092/api/2.0/files/@root',
    method: 'GET',
    baseDir: 'files/root',
  };
  static fileSettings = {
    url: 'http://localhost:8092/api/2.0/files/settings',
    method: 'GET',
    baseDir: 'files/settings',
  };
  static my = {
    url: 'http://localhost:8092/api/2.0/files/@my',
    method: 'GET',
    baseDir: 'files/my',
  };
  static capabilities = {
    url: 'http://localhost:8092/api/2.0/files/thirdparty/capabilities',
    method: 'GET',
    baseDir: 'files/settings',
  };
  static thirdparty = {
    url: 'http://localhost:8092/api/2.0/files/thirdparty',
    method: 'GET',
    baseDir: 'files/settings',
  };
  static thumbnails = {
    url: 'http://localhost:8092/api/2.0/files/thumbnails',
    method: 'POST',
    baseDir: 'files/settings',
  };
  static folder = {
    url: 'http://localhost:8092/api/2.0/files/',
    method: 'GET',
    baseDir: 'files/folder',
  };
};
