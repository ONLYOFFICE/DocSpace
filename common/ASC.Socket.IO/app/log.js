const winston = require("winston"),
      WinstonCloudWatch = require('winston-cloudwatch');

require("winston-daily-rotate-file");

const path = require("path");
const config = require("../config");
const fs = require("fs");
const os = require("os");
const { randomUUID } = require('crypto');
const date = require('date-and-time');

let logpath = config.get("logPath");
if(logpath != null)
{
    if(!path.isAbsolute(logpath))
    {
        logpath = path.join(__dirname, "..", logpath);
    }
}

const fileName = logpath ? path.join(logpath, "socket-io.%DATE%.log") : path.join(__dirname, "..", "..", "..", "Logs", "socket-io.%DATE%.log");
const dirName = path.dirname(fileName);

const aws = config.get("aws");

const accessKeyId = aws.accessKeyId; 
const secretAccessKey = aws.secretAccessKey; 
const awsRegion = aws.region; 
const logGroupName = aws.logGroupName;
const logStreamName = aws.logStreamName;

if (!fs.existsSync(dirName)) {
  fs.mkdirSync(dirName);
}

var options = {
  file: {
    filename: fileName,
    datePattern: "MM-DD",
    handleExceptions: true,
    humanReadableUnhandledException: true,
    zippedArchive: true,
    maxSize: "50m",
    maxFiles: "30d",
    json: true,
  },
  console: {
    level: "debug",
    handleExceptions: true,
    json: false,
    colorize: true,
  },
  cloudWatch: {
    name: 'aws',
    level: "debug",
    logGroupName: () => {
      const hostname = os.hostname();

      return logGroupName.replace("${instance-id}", hostname);      
    },       
    logStreamName: () => {
      const now = new Date();
      const guid = randomUUID();
      const dateAsString = date.format(now, 'YYYY/MM/DDTHH.mm.ss');
      
      return logStreamName.replace("${guid}", guid)
                          .replace("${date}", dateAsString);      
    },
    awsRegion: awsRegion,
    jsonMessage: true,
    awsOptions: {
      credentials: {
        accessKeyId: accessKeyId,
        secretAccessKey: secretAccessKey
      }
    }
  }
};

//const fileTransport = new winston.transports.DailyRotateFile(options.file);

var transports = [
  new winston.transports.Console(options.console),
  new winston.transports.DailyRotateFile(options.file)  
];

if (aws != null && aws.accessKeyId !== '')
{
  transports.push(new WinstonCloudWatch(options.cloudWatch));
}

//winston.exceptions.handle(fileTransport);

const customFormat = winston.format(info => {
  const now = new Date();

  info.date = date.format(now, 'YYYY-MM-DD HH:mm:ss');
  info.applicationContext = "SocketIO";
  info.level = info.level.toUpperCase();

  const hostname = os.hostname();

  info["instance-id"] = hostname;

  return info;
})();

module.exports = new winston.createLogger({
  //defaultMeta: { component: "socket.io-server" },
  format: winston.format.combine(
    customFormat,
    winston.format.json()    
  ),
  transports: transports,
  exitOnError: false,
});