import express, { Response } from "express";
import template from "./lib/template";
import path from "path";
import compression from "compression";
import ws from "./lib/websocket";
import fs from "fs";
import logger from "morgan";
import winston, { stream } from "./lib/logger";
import { getAssets, getInitialState } from "./lib/helpers";
import renderApp from "./lib/helpers/render-app";
import i18nextMiddleware from "i18next-express-middleware";
import i18next from "./i18n";
import cookieParser from "cookie-parser";
import { LANGUAGE, COOKIE_EXPIRATION_YEAR } from "@docspace/common/constants";
import { getLanguage } from "@docspace/common/utils";
import { initSSR } from "@docspace/common/api/client";
import dns from "dns";

let port = PORT;

dns.setDefaultResultOrder("ipv4first");

const config = fs.readFileSync(path.join(__dirname, "config.json"), "utf-8");
const parsedConfig: IParsedConfig = JSON.parse(config);

if (parsedConfig.PORT) {
  port = parsedConfig.PORT;
}

const app = express();
app.use(i18nextMiddleware.handle(i18next));
app.use(compression());
app.use(cookieParser());
app.use(
  "/login",
  express.static(path.resolve(path.join(__dirname, "client")), {
    // don`t delete
    // https://github.com/pillarjs/send/issues/110
    cacheControl: false,
  })
);

app.use(logger("dev", { stream: stream }));

app.get("*", async (req: ILoginRequest, res: Response, next) => {
  const { i18n, cookies, headers, query, t, url } = req;
  let initialState: IInitialState;
  let assets: assetsType;

  initSSR(headers);

  try {
    initialState = await getInitialState(query);

    if (initialState.isAuth && url !== "/login/error") {
      res.redirect("/");
      next();
    }

    let currentLanguage: string = initialState.portalSettings.culture;

    if (cookies && cookies[LANGUAGE]) {
      currentLanguage = cookies[LANGUAGE];
    } else {
      res.cookie(LANGUAGE, currentLanguage, {
        maxAge: COOKIE_EXPIRATION_YEAR,
      });
    }

    currentLanguage = getLanguage(currentLanguage);

    if (i18n) await i18n.changeLanguage(currentLanguage);

    let initialI18nStore: {
      [key: string]: { [key: string]: {} };
    } = {};

    if (i18n && i18n?.services?.resourceStore?.data) {
      for (let key in i18n?.services?.resourceStore?.data) {
        if (key === "en" || key === currentLanguage) {
          initialI18nStore[key] = i18n.services.resourceStore.data[key];
        }
      }
    }

    assets = await getAssets();

    const { component, styleTags } = renderApp(i18n, initialState, url);

    const htmlString = template(
      initialState,
      component,
      styleTags,
      initialI18nStore,
      currentLanguage,
      assets,
      t
    );

    res.send(htmlString);
  } catch (e) {
    let message: string | unknown = e;
    if (e instanceof Error) {
      message = e.message;
    }
    winston.error(message);
  }
});

const server = app.listen(port, () => {
  winston.info(`Server is listening on port ${port}`);
});

if (IS_DEVELOPMENT) {
  const wss = ws(server);

  const manifestFile = path.resolve(
    path.join(__dirname, "client/manifest.json")
  );

  let fsWait = false;
  let waitTimeout: timeoutType;
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
