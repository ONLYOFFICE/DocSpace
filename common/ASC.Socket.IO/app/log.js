const winston = require("winston");
require("winston-daily-rotate-file");

const path = require("path");
const config = require("../config");
const fs = require("fs");

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
};

//const fileTransport = new winston.transports.DailyRotateFile(options.file);

const transports = [
  new winston.transports.Console(options.console),
  new winston.transports.DailyRotateFile(options.file),
];

//winston.exceptions.handle(fileTransport);

module.exports = new winston.createLogger({
  //defaultMeta: { component: "socket.io-server" },
  format: winston.format.combine(
    winston.format.timestamp({
      format: "YYYY-MM-DD HH:mm:ss",
    }),
    winston.format.json()
  ),
  transports: transports,
  exitOnError: false,
});
