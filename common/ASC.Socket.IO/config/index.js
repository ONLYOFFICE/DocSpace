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
    var valueEnv = nconf.get(env);
    var fileWithEnv = path.join(appsettings, 'appsettings.' + valueEnv + '.json');

    if(fs.existsSync(fileWithEnv)){
        nconf.file("appsettings", fileWithEnv);
    }
    else{
        nconf.file("appsettings", path.join(appsettings, 'appsettings.json'));
    }

    nconf.file("appsettingsServices", path.join(appsettings, 'appsettings.services.json'));

    var redisWithEnv = path.join(appsettings, 'redis.' + valueEnv + '.json');
    if(fs.existsSync(fileWithEnv)){
        nconf.file("redis", redisWithEnv);
    }
    else{
        nconf.file("redis", path.join(__dirname, appsettings, 'redis.json'));
    }

    var redis = nconf.get("Redis");
    if(redis != null)
    {
        redis.host = redis.Hosts[0].Host; 
        redis.port = redis.Hosts[0].Port; 
        redis.connect_timeout = redis.ConnectTimeout;
        redis.db = redis.Database;
        nconf.set("Redis", redis);
    }

    


}