import winston from "winston";
import WinstonCloudWatch from "winston-cloudwatch"; 
import date from "date-and-time"; 
import os from "os"; 
import "winston-daily-rotate-file";
import path from "path";
import fs from "fs";
import config from "../config";
import { randomUUID } from "crypto";

let logPath: string = config.get("logPath");

if (logPath != null) {
  if (!path.isAbsolute(logPath)) {
    logPath = path.join(__dirname, "..", logPath);
  }
}

const fileName = logPath
  ? path.join(logPath, "login.%DATE%.log")
  : path.join(__dirname, "..", "..", "..", "Logs", "login.%DATE%.log");
const dirName = path.dirname(fileName);

if (!fs.existsSync(dirName)) {
  fs.mkdirSync(dirName);
}

const aws = config.get("aws").cloudWatch;

const accessKeyId = aws.accessKeyId; 
const secretAccessKey = aws.secretAccessKey; 
const awsRegion = aws.region; 
const logGroupName = aws.logGroupName;
const logStreamName = aws.logStreamName;

const options = {
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
    logStreamName: () => {
      const hostname = os.hostname();
      const now = new Date();
      const guid = randomUUID();
      const dateAsString = date.format(now, 'YYYY/MM/DDTHH.mm.ss');
      
      return logStreamName.replace("${hostname}", hostname)
                          .replace("${applicationContext}", "Login")                  
                          .replace("${guid}", guid)
                          .replace("${date}", dateAsString);      
    },
    logGroupName:logGroupName,
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

const transports: winston.transport[] = [
  new winston.transports.Console(options.console),
  new winston.transports.DailyRotateFile(options.file)
];

if (aws != null && aws.accessKeyId !== '')
{
  transports.push(new WinstonCloudWatch(options.cloudWatch));
}

const customFormat = winston.format(info => {
  const now = new Date();

  info.date = date.format(now, 'YYYY-MM-DD HH:mm:ss');
  info.applicationContext = "Login";
  info.level = info.level.toUpperCase();

  const hostname = os.hostname();

  info["instance-id"] = hostname;

  return info;
})();

const logger = winston.createLogger({
  format: winston.format.combine(
    customFormat,
    winston.format.json()    
  ),
  transports: transports,
  exitOnError: false,
});

export default logger;

export const stream = {
  write: (message: string) => logger.info(message),
};
