import winston from "winston";
import "winston-daily-rotate-file";
import path from "path";
import fs from "fs";

let logpath = process.env.logpath || null;

if (logpath != null) {
  if (!path.isAbsolute(logpath)) {
    logpath = path.join(__dirname, "..", logpath);
  }
}

const fileName = IS_DEVELOPMENT
  ? path.join(__dirname, "..", "..", "..", "Logs", "editor.%DATE%.log")
  : logpath
  ? path.join(logpath, "editor.%DATE%.log")
  : path.join(__dirname, "..", "..", "..", "Logs", "editor.%DATE%.log");
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

export default new winston.createLogger({
  format: winston.format.combine(
    winston.format.timestamp({
      format: "YYYY-MM-DD HH:mm:ss",
    }),
    winston.format.json()
  ),
  transports: transports,
  exitOnError: false,
});
