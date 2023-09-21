import { NestFactory } from "@nestjs/core";
import { AppModule } from "./src/app/app.module";

const port = 5016;
const winston = require("./src/log.js");

async function bootstrap() {
  try {
    const app = await NestFactory.create(AppModule);
    await app.listen(port, () => {
      winston.info(`Start TelegramReports Service  listening on port ${port} for http`);
    });
  } catch (e) {
    winston.error(e);
  }
}
bootstrap();
