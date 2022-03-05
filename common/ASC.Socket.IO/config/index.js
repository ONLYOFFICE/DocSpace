const nconf = require('nconf');
const path = require('path');
const fs = require("fs");

nconf.argv()
    .env()
    .file("config", path.join(__dirname, 'config.json'));

getAndSaveAppsettings();

module.exports = nconf;

function getAndSaveAppsettings(){
    var appsettings = nconf.get("appsettings");
    var env = nconf.get("environment");
    var valueEnv = nconf.get(env);
    var fileWithEnv = path.join(__dirname, appsettings, 'appsettings.' + valueEnv + '.json');

    if(fs.existsSync(fileWithEnv)){
        nconf.file("appsettings", fileWithEnv);
    }
    else{
        nconf.file("appsettings", path.join(__dirname, appsettings, 'appsettings.json'));
    }

    nconf.file("appsettingsServices", path.join(__dirname, appsettings, 'appsettings.services.json'));

    var redisWithEnv = path.join(__dirname, appsettings, 'redis.' + valueEnv + '.json');
    if(fs.existsSync(fileWithEnv)){
        nconf.file("redis", redisWithEnv);
    }
    else{
        nconf.file("redis", path.join(__dirname, appsettings, 'redis.json'));
    }
    var hosts = nconf.get("Redis").Hosts;
    var redis = nconf.get("redis");
    if(hosts && hosts.count > 0)
    {
        redis.host = hosts[0].Host; 
        redis.port = hosts[0].Port; 
        nconf.set("redis", redis);
    }
}