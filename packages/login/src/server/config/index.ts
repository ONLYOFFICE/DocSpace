import nconf from "nconf";
import path from "path";
import fs from "fs";

nconf.argv().env().file("config", path.join(__dirname, "config.json"));

getAndSaveAppsettings();

function getAndSaveAppsettings() {
  let appsettings: string = nconf.get("app").appsettings;

  if (!path.isAbsolute(appsettings)) {
    appsettings = path.join(__dirname, appsettings);
  }

  const env: string = nconf.get("app").environment;
  console.log('environment: ' + env);
  nconf.file("appsettingsWithEnv", path.join(appsettings, 'appsettings.' + env + '.json'));
  nconf.file("appsettings", path.join(appsettings, 'appsettings.json'));

  nconf.file(
    "appsettingsServices",
    path.join(appsettings, "appsettings.services.json")
  );
}

export default nconf;
