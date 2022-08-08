import express, { Request, Response } from "express";
import template from "./lib/template";
import path from "path";
import compression from "compression";
import ws from "./lib/websocket";
import fs from "fs";
import logger from "morgan";
import winston, { stream } from "./lib/logger";
import { getAssets } from "./lib/helpers";
import renderApp, { getStyleTags } from "./lib/helpers/render-app";
import i18nextMiddleware, { I18next } from "i18next-express-middleware";
import i18next from "./i18n";
import cookieParser from "cookie-parser";
import { LANGUAGE } from "@docspace/common/constants";
import parser from "accept-language-parser";
import { getPortalCultures } from "@docspace/common/api/settings";
import { initSSR } from "@docspace/common/api/client";
interface IParsedConfig extends Object {
  PORT: number;
}
interface ILoginRequest extends Request {
  i18n?: I18next;
}
type Timeout = ReturnType<typeof setTimeout>;
interface IAcceptLanguage extends Object {
  code?: string;
  quality?: number;
}
let port = PORT;

const config = fs.readFileSync(path.join(__dirname, "config.json"), "utf-8");
const parsedConfig: IParsedConfig = JSON.parse(config);

if (parsedConfig.PORT) {
  port = parsedConfig.PORT;
}

const app = express();
app.use(i18nextMiddleware.handle(i18next));
app.use(compression());
app.use(cookieParser());
app.use("/login", express.static(path.resolve(path.join(__dirname, "client"))));

app.use(logger("dev", { stream: stream }));

if (IS_DEVELOPMENT) {
  app.get("/login", async (req: ILoginRequest, res: Response) => {
    const { i18n, cookies, headers } = req;
    initSSR(headers);

    let currentLanguage = "en";

    if (cookies && cookies[LANGUAGE]) {
      currentLanguage = cookies[LANGUAGE];
    } else {
      const availableLanguages: string[] = await getPortalCultures();
      const parsedAcceptLanguages: object[] = parser.parse(
        headers["accept-language"]
      );

      const detectedLanguage:
        | IAcceptLanguage
        | any = parsedAcceptLanguages.find(
        (acceptLang: IAcceptLanguage) =>
          typeof acceptLang === "object" &&
          acceptLang?.code &&
          availableLanguages.includes(acceptLang.code)
      );

      if (typeof detectedLanguage === "object")
        currentLanguage = detectedLanguage.code;
    }

    await i18next.changeLanguage(currentLanguage);
    let initialI18nStore = {};
    if (i18n) initialI18nStore = i18n.services.resourceStore.data;

    let assets: assetsType;

    try {
      assets = await getAssets();
    } catch (e) {
      let message: string | unknown = e;
      if (e instanceof Error) {
        message = e.message;
      }
      winston.error(message);
    }

    const appComponent = renderApp();
    const styleTags = getStyleTags();
    const htmlString = template(
      {},
      appComponent,
      styleTags,
      initialI18nStore,
      currentLanguage,
      assets
    );

    res.send(htmlString);
  });

  const server = app.listen(port, () => {
    winston.info(`Server is listening on port ${port}`);
  });

  const wss = ws(server);

  const manifestFile = path.resolve(
    path.join(__dirname, "client/manifest.json")
  );

  let fsWait = false;
  let waitTimeout: Timeout;
  fs.watch(manifestFile, (event, filename) => {
    if (filename && event === "change") {
      if (fsWait) return;
      fsWait = true;
      waitTimeout = setTimeout(() => {
        fsWait = false;
        clearTimeout(waitTimeout);

        wss.broadcast && wss.broadcast("reload");
      }, 100);
    }
  });
}
