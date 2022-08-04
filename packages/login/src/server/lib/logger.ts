import { createLogger, transports, format } from "winston";
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
  ? path.join(__dirname, "..", "..", "..", "Logs", "login.%DATE%.log")
  : logpath
  ? path.join(logpath, "login.%DATE%.log")
  : path.join(__dirname, "..", "..", "..", "Logs", "login.%DATE%.log");
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

const logger = createLogger({
  format: format.combine(
    format.timestamp({
      format: "YYYY-MM-DD HH:mm:ss",
    }),
    format.json()
  ),
  transports: [
    new transports.Console(options.console),
    new transports.DailyRotateFile(options.file),
  ],
  exitOnError: false,
});

export default logger;

export const stream = {
  write: (message: string) => logger.info(message),
};
