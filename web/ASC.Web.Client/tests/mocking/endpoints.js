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

  static timeandlanguage = {
    url: ["http://localhost:8092/api/2.0/settings/timeandlanguage.json"],
    method: "PUT",
    baseDir: "settings",
  };

  static common = {
    url: ["http://localhost:8092/api/2.0/settings/customschemas/Common.json"],
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

  static code = {
    url: ["http://localhost:8092/api/2.0/authentication/123456"],
    method: "POST",
    baseDir: "auth",
  };

  static self = {
    url: [
      "http://localhost:8092/api/2.0/people/@self.json",
      "http://localhost:8092/api/2.0/people/%40self.json",
    ],
    method: "GET",
    baseDir: "people",
  };

  static info = {
    url: ["http://localhost:8092/api/2.0/modules/info"],
    method: "GET",
    baseDir: "modules",
  };

  static setup = {
    url: ["http://localhost:8092/api/2.0/settings/tfaapp/setup"],
    method: "GET",
    baseDir: "settings",
  };

  static validation = {
    url: ["http://localhost:8092/api/2.0/settings/tfaapp/validate"],
    method: "POST",
    baseDir: "settings",
  };

  static cultures = {
    url: ["http://localhost:8092/api/2.0/settings/cultures.json"],
    method: "GET",
    baseDir: "settings",
  };

  static timezones = {
    url: ["http://localhost:8092/api/2.0/settings/timezones.json"],
    method: "GET",
    baseDir: "settings",
  };

  static capabilities = {
    url: ["http://localhost:8092/api/2.0/capabilities"],
    method: "GET",
    baseDir: "settings",
  };

  static user = {
    url: ["http://localhost:8092/api/2.0/people/user.json"],
    method: "GET",
    baseDir: "people",
  };

  static tfaapp = {
    url: ["http://localhost:8092/api/2.0/settings/tfaapp"],
    method: "GET",
    baseDir: "settings",
  };

  static settfaapp = {
    url: ["http://localhost:8092/api/2.0/settings/tfaapp"],
    method: "PUT",
    baseDir: "settings",
  };

  static tfaconfirm = {
    url: ["http://localhost:8092/api/2.0/settings/tfaapp/confirm"],
    method: "GET",
    baseDir: "settings",
  };
};
