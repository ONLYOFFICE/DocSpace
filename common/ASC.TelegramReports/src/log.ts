import * as winston from "winston";
import * as WinstonCloudWatch from "winston-cloudwatch";
import * as date from "date-and-time";
import * as os from "os";
import * as config from "../config";
import { randomUUID } from "crypto";
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
    ? path.join(logpath, "telegramreports.%DATE%.log")
    : path.join(__dirname, "..", "..", "..", "..", "Logs", "telegramreports.%DATE%.log");

const dirName = path.dirname(fileName);

if (!fs.existsSync(dirName)) {
    fs.mkdirSync(dirName);
}

const aws = config.default.get("aws").cloudWatch;

const accessKeyId = aws.accessKeyId;
const secretAccessKey = aws.secretAccessKey;
const awsRegion = aws.region;
const logGroupName = aws.logGroupName;
const logStreamName = aws.logStreamName.replace("${hostname}", os.hostname())
    .replace("${applicationContext}", "TelegramReports")
    .replace("${guid}", randomUUID())
    .replace("${date}", date.format(new Date(), 'YYYY/MM/DDTHH.mm.ss'));

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
    }
};

const transports: winston.transport[] = [
    new winston.transports.Console(options.console),
    new winston.transports.DailyRotateFile(options.file)
];

if (aws != null && aws.accessKeyId !== '') {
    transports.push(new WinstonCloudWatch(options.cloudWatch));
}

const customFormat = winston.format(info => {
    const now = new Date();

    info.date = date.format(now, 'YYYY-MM-DD HH:mm:ss');
    info.applicationContext = "TelegramReports";
    info.level = info.level.toUpperCase();

    const hostname = os.hostname();

    info["instance-id"] = hostname;

    return info;
})();

module.exports = winston.createLogger({
    format: winston.format.combine(
        customFormat,
        winston.format.json()
    ),
    transports: transports,
    exitOnError: false,
});