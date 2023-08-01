const winston = require("winston"),
  WinstonCloudWatch = require("winston-cloudwatch");

require("winston-daily-rotate-file");

const path = require("path");
const config = require("../config");
const fs = require("fs");
const os = require("os");
const { randomUUID } = require("crypto");
const date = require("date-and-time");

let logpath = config.get("logPath");
let logLevel = config.get("logLevel") || "debug";
if (logpath != null) {
  if (!path.isAbsolute(logpath)) {
    logpath = path.join(__dirname, "..", logpath);
  }
}

const fileName = logpath
  ? path.join(logpath, "web.sso.%DATE%.log")
  : path.join(__dirname, "..", "..", "..", "Logs", "web.sso.%DATE%.log");
const dirName = path.dirname(fileName);

const aws = config.get("aws").cloudWatch;

const accessKeyId = aws.accessKeyId;
const secretAccessKey = aws.secretAccessKey;
const awsRegion = aws.region;
const logGroupName = aws.logGroupName;
const logStreamName = aws.logStreamName
  .replace("${hostname}", os.hostname())
  .replace("${applicationContext}", "SsoAuth")
  .replace("${guid}", randomUUID())
  .replace("${date}", date.format(new Date(), "YYYY/MM/DDTHH.mm.ss"));

if (!fs.existsSync(dirName)) {
  fs.mkdirSync(dirName);
}

var options = {
  file: {
    filename: fileName,
    level: logLevel,
    datePattern: "MM-DD",
    handleExceptions: true,
    humanReadableUnhandledException: true,
    zippedArchive: true,
    maxSize: "50m",
    maxFiles: "30d",
    json: true,
  },
  console: {
    level: logLevel,
    handleExceptions: true,
    json: false,
    colorize: true,
  },
  cloudWatch: {
    name: "aws",
    level: logLevel,
    logStreamName: logStreamName,
    logGroupName: logGroupName,
    awsRegion: awsRegion,
    jsonMessage: true,
    awsOptions: {
      credentials: {
        accessKeyId: accessKeyId,
        secretAccessKey: secretAccessKey,
      },
    },
  },
};

let transports = [
  new winston.transports.Console(options.console),
  new winston.transports.DailyRotateFile(options.file),
];

if (aws != null && aws.accessKeyId !== "") {
  transports.push(new WinstonCloudWatch(options.cloudWatch));
}

const customFormat = winston.format((info) => {
  const now = new Date();

  info.date = date.format(now, "YYYY-MM-DD HH:mm:ss");
  info.applicationContext = "SsoAuth";
  info.level = info.level.toUpperCase();

  const hostname = os.hostname();

  info["instance-id"] = hostname;

  return info;
})();

module.exports = new winston.createLogger({
  format: winston.format.combine(customFormat, winston.format.json()),
  transports: transports,
  exitOnError: false,
});
