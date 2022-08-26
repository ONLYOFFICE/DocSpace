import * as nconf from "nconf";
import * as path from "path";
import * as fs from "fs";

import * as conf from "./config.json";

nconf.argv().env().file("config", path.join(__dirname, "config.json"));

getAndSaveAppsettings();

export default nconf;

function getAndSaveAppsettings() {
  var appsettings = nconf.get("app").appsettings;

  if (!path.isAbsolute(appsettings)) {
    appsettings = path.join(__dirname, appsettings);
  }

  var env = nconf.get("app").environment;

  var valueEnv = nconf.get(env);

  var fileWithEnv = path.join(appsettings, "appsettings." + valueEnv + ".json");

  if (fs.existsSync(fileWithEnv)) {
    nconf.file("appsettings", fileWithEnv);
  } else {
    nconf.file("appsettings", path.join(appsettings, "appsettings.json"));
  }

  nconf.file("pluginsConf", path.join(appsettings, "plugins.json"));
}
