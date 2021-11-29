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
  static common = {
    url: ['http://localhost:8092/api/2.0/settings/customschemas/Common.json'],
    method: 'GET',
    baseDir: 'settings',
  };
  static cultures = {
    url: ['http://localhost:8092/api/2.0/settings/cultures.json'],
    method: 'GET',
    baseDir: 'settings',
  };
  static root = {
    url: [
      'http://localhost:8092/api/2.0/files/@root',
      'http://localhost:8092/api/2.0/files/%40root',
    ],
    method: 'GET',
    baseDir: 'files/root',
  };
  static fileSettings = {
    url: ['http://localhost:8092/api/2.0/files/settings'],
    method: 'GET',
    baseDir: 'files/settings',
  };
  static my = {
    url: ['http://localhost:8092/api/2.0/files/@my', 'http://localhost:8092/api/2.0/files/%40my'],
    method: 'GET',
    baseDir: 'files/my',
  };
  static capabilities = {
    url: ['http://localhost:8092/api/2.0/files/thirdparty/capabilities'],
    method: 'GET',
    baseDir: 'files/settings',
  };
  static thirdparty = {
    url: ['http://localhost:8092/api/2.0/files/thirdparty'],
    method: 'GET',
    baseDir: 'files/settings',
  };
  static thumbnails = {
    url: ['http://localhost:8092/api/2.0/files/thumbnails'],
    method: 'POST',
    baseDir: 'files/settings',
  };
  static getFolder = (folderId) => {
    return {
      url: [`http://localhost:8092/api/2.0/files/${folderId}`],
      method: 'GET',
      baseDir: 'files/folder',
    };
  };

  static getSubfolder = (folderId) => {
    return {
      url: [`http://localhost:8092/api/2.0/files/${folderId}/subfolders`],
      method: 'GET',
      baseDir: 'files/subfolder',
    };
  };

  static getFile = (fileId) => {
    return {
      url: [`http://localhost:8092/api/2.0/files/file/${fileId}`],
      method: 'GET',
      baseDir: 'files/file',
    };
  };
};
