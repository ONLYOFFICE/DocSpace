import { NestFactory } from "@nestjs/core";
import { AppModule } from "./app.module";

import * as config from "../config";

const winston = require("./log.js");

const port = config.default.get("app").port || 5014;

async function bootstrap() {
  try {
    const app = await NestFactory.create(AppModule, { cors: false });
    // app.enableCors();
    await app.listen(port, () => {
      winston.info(`Plugins service start at port: ${port}`);
    });
  } catch (e) {
    winston.error(e);
  }
}
bootstrap();
