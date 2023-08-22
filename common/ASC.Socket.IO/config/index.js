const nconf = require('nconf');
const path = require('path');
const fs = require("fs");

nconf.argv()
    .env()
    .file("config", path.join(__dirname, 'config.json'));

getAndSaveAppsettings();

module.exports = nconf;

function getAndSaveAppsettings(){
    var appsettings = nconf.get("app").appsettings;
    if(!path.isAbsolute(appsettings)){
        appsettings = path.join(__dirname, appsettings);
    }

    var env = nconf.get("app").environment;

    console.log('environment: ' + env);
    nconf.file("appsettingsWithEnv", path.join(appsettings, 'appsettings.' + env + '.json'));
    nconf.file("appsettings", path.join(appsettings, 'appsettings.json'));

    nconf.file("appsettingsServices", path.join(appsettings, 'appsettings.services.json'));

    nconf.file("redisWithEnv", path.join(appsettings, 'redis.' + env + '.json'));
    nconf.file("redis", path.join(appsettings, 'redis.json'));

    var redis = nconf.get("Redis");
    if(redis != null)
    {
        redis.host = redis.Hosts[0].Host; 
        redis.port = redis.Hosts[0].Port; 
        redis.connect_timeout = redis.ConnectTimeout;
        redis.db = redis.Database;
        redis.user = redis.User;
        redis.password = redis.Password;
        nconf.set("Redis", redis);
    }
}