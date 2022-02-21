module.exports = class Endpoints {
  static confirm = {
    url: ["http://localhost:8092/api/2.0/authentication/confirm.json"],
    method: "POST",
    baseDir: "user",
  };

  static settings = {
    url: ["http://localhost:8092/api/2.0/settings.json"],
    method: "GET",
    baseDir: "settings",
  };

  static build = {
    url: ["http://localhost:8092/api/2.0/settings/version/build.json"],
    method: "GET",
    baseDir: "settings",
  };

  static password = {
    url: ["http://localhost:8092/api/2.0/settings/security/password"],
    method: "GET",
    baseDir: "settings",
  };

  static providers = {
    url: ["http://localhost:8092/api/2.0/people/thirdparty/providers"],
    method: "GET",
    baseDir: "people",
  };
};
