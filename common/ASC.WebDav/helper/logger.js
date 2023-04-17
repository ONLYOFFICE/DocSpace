/*
 *
 * (c) Copyright Ascensio System Limited 2010-2021
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * http://www.apache.org/licenses/LICENSE-2.0
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */


const winston = require("winston"),
      WinstonCloudWatch = require('winston-cloudwatch'),
      { randomUUID } = require('crypto'),
      date = require('date-and-time'),
      os = require("os");


const { format } = require("winston");
require("winston-daily-rotate-file");

const path = require("path");
const config = require("../server/config.js");
const fs = require("fs");
const logLevel = process.env.logLevel || config.logLevel || "info";
const fileName = process.env.logPath || path.join(__dirname, "..", "logs", "web.webdav.%DATE%.log");
const dirName = path.dirname(fileName);

if (!fs.existsSync(dirName)) {
    fs.mkdirSync(dirName);
}

const fileTransport = new (winston.transports.DailyRotateFile)({
    filename: fileName,
    datePattern: "MM-DD",
    handleExceptions: true,
    humanReadableUnhandledException: true,
    zippedArchive: true,
    maxSize: "50m",
    maxFiles: "30d"
});

const nconf = require("nconf");

nconf.argv()
     .env();

var appsettings = config.appsettings;

if(!path.isAbsolute(appsettings)){
    appsettings = path.join(__dirname, appsettings);
}

var fileWithEnv = path.join(appsettings, 'appsettings.' + config.environment + '.json');

if(fs.existsSync(fileWithEnv)){
    nconf.file("appsettings", fileWithEnv);
}
else{
    nconf.file("appsettings", path.join(appsettings, 'appsettings.json'));
}

const aws = nconf.get("aws").cloudWatch;

const accessKeyId = aws.accessKeyId; 
const secretAccessKey = aws.secretAccessKey; 
const awsRegion = aws.region; 
const logGroupName = aws.logGroupName;
const logStreamName = aws.logStreamName.replace("${hostname}", os.hostname())
                                      .replace("${applicationContext}", "WebDav")                  
                                      .replace("${guid}", randomUUID())
                                      .replace("${date}", date.format(new Date(), 'YYYY/MM/DDTHH.mm.ss'));      

let transports = [
    new (winston.transports.Console)(),
    fileTransport
];


if (aws != null && aws.accessKeyId !== '')
{
  transports.push(new WinstonCloudWatch({
    name: 'aws',
    level: "debug",
    logStreamName: logStreamName,
    logGroupName: logGroupName,
    awsRegion: awsRegion,
    jsonMessage: true,
    awsOptions: {
      credentials: {
        accessKeyId: accessKeyId,
        secretAccessKey: secretAccessKey
      }
    }
  }));
}

const customFormat = winston.format(info => {
    const now = new Date();
  
    info.date = date.format(now, 'YYYY-MM-DD HH:mm:ss');
    info.applicationContext = "WebDav";
    info.level = info.level.toUpperCase();
  
    const hostname = os.hostname();
  
    info["instance-id"] = hostname;
  
    return info;
  })();

winston.exceptions.handle(fileTransport);

module.exports = winston.createLogger({
    level: logLevel,
    transports: transports,
    exitOnError: false,
    format: format.combine(
        customFormat,
        winston.format.json()    
)});