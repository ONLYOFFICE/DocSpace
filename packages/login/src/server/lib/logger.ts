import { createLogger, transports, format } from "winston";
import "winston-daily-rotate-file";
import path from "path";
import fs from "fs";
import config from "../config";

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
