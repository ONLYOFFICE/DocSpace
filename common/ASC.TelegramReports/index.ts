import firebase from "firebase/compat/app";
import "firebase/compat/database";

import { NestFactory } from "@nestjs/core";
import { AppModule } from "./src/app/app.module";
import { AppService } from "./src/app/app.service";

import * as config from "./config";

const winston = require("./src/log.js");

const firebaseConfig = config.default.get("firebase");
firebase.initializeApp(firebaseConfig);

async function bootstrap() {
  try {
    const app = await NestFactory.create(AppModule);
    const appService = app.get(AppService);

    winston.info(`Start TelegramReports Service  listening`);

    const ref = firebase.database().ref("reports").limitToLast(1);

    ref.on("child_added", (data) => {
      appService.sendMessage(data.val())
    });
  } catch (e) {
    winston.error(e);
  }
}
bootstrap();
