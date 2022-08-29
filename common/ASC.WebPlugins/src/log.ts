import * as winston from "winston";
import "winston-daily-rotate-file";
import * as path from "path";
import * as fs from "fs";

let logpath = process.env.logpath || null;

if (logpath != null) {
  if (!path.isAbsolute(logpath)) {
    logpath = path.join(__dirname, "..", logpath);
  }
}

const fileName = logpath
  ? path.join(logpath, "plugins.%DATE%.log")
  : path.join(__dirname, "..", "..", "..", "..", "Logs", "plugins.%DATE%.log");

const dirName = path.dirname(fileName);

if (!fs.existsSync(dirName)) {
  fs.mkdirSync(dirName);
}

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
};

const transports = [
  new winston.transports.Console(options.console),
  new winston.transports.DailyRotateFile(options.file),
];

export default winston.createLogger({
  format: winston.format.combine(
    winston.format.timestamp({
      format: "YYYY-MM-DD HH:mm:ss",
    }),
    winston.format.json()
  ),
  transports: transports,
  exitOnError: false,
});
