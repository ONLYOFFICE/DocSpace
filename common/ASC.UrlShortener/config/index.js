/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


const nconf = require("nconf");
const path = require("path");
const fs = require("fs");

nconf.argv()
    .env()
    .file("config", path.join(__dirname, 'config.json') );

getAndSaveAppsettings();

getAndSaveSql();

module.exports = nconf;

function getAndSaveAppsettings(){
    var appsettings = nconf.get("app").appsettings;
    if(!path.isAbsolute(appsettings)){
        appsettings =path.join(__dirname, appsettings);
    }

    var env = nconf.get("app").environment;
    console.log('environment: ' + env);
    
    nconf.file("appsettingsWithEnv", path.join(appsettings, 'appsettings.' + env + '.json'));
    nconf.file("appsettings", path.join(appsettings, 'appsettings.json'));
    nconf.file("appsettingsServices", path.join(appsettings, 'appsettings.services.json'));
}

function getAndSaveSql(){
    var sql = new Map();
    var connetionString = nconf.get("ConnectionStrings").default.connectionString;

    var conf = connetionString.split(';');
    
    for(let i = 0; i < conf.length; i++){
        var splited = conf[i].split('=');
        if(splited.Length < 2) continue;
        if(splited[0] != null){
            sql[splited[0]] = splited[1];
        }
    }
    nconf.set("sql", sql);
}