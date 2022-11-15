import express from "express";
import template from "./lib/template";
import devMiddleware from "./lib/middleware/devMiddleware";
import i18nextMiddleware from "i18next-express-middleware";
import i18next from "i18next";
import Backend from "i18next-fs-backend";
import path from "path";
import compression from "compression";
import ws from "./lib/websocket";
import fs from "fs";
import logger from "morgan";
import winston from "./lib/logger.js";
import { getAssets, initDocEditor } from "./lib/helpers";
import renderApp from "./lib/helpers/render-app";
import dns from "dns";

dns.setDefaultResultOrder("ipv4first");

const fallbackLng = "en";

let port = PORT;

const config = fs.readFileSync(path.join(__dirname, "config.json"), "utf-8");
const parsedCOnfig = JSON.parse(config);

if (parsedCOnfig?.PORT) {
  port = parsedCOnfig.PORT;
}

winston.stream = {
  write: (message) => winston.info(message),
};

const loadPath = (lng, ns) => {
  let resourcePath = path.resolve(
    path.join(__dirname, "client", `locales/${lng}/${ns}.json`)
  );
  if (ns === "Common")
    resourcePath = path.resolve(
      path.join(__dirname, `../../../public/locales/${lng}/${ns}.json`)
    );

  return resourcePath;
};

const app = express();

i18next.use(Backend).init({
  backend: {
    loadPath: loadPath,
    allowMultiLoading: true,
    crossDomain: false,
  },
  fallbackLng: fallbackLng,
  load: "currentOnly",

  saveMissing: true,
  ns: ["Editor", "Common"],
  defaultNS: "Editor",

  interpolation: {
    escapeValue: false,
    format: function (value, format) {
      if (format === "lowercase") return value.toLowerCase();
      return value;
    },
  },
});

app.use(i18nextMiddleware.handle(i18next));
app.use(compression());
app.use(
  "/doceditor/",
  express.static(path.resolve(path.join(__dirname, "client")))
);
app.use(logger("dev", { stream: winston.stream }));

if (IS_DEVELOPMENT) {
  app.use(devMiddleware);

  app.get("/doceditor", async (req, res) => {
    const { i18n, initialEditorState, assets } = req;
    const userLng = initialEditorState?.user?.cultureName || "en";

    await i18next.changeLanguage(userLng);
    const initialI18nStoreASC = i18n.services.resourceStore.data;

    if (initialEditorState?.error) {
      winston.error(initialEditorState.error.errorMessage);
    }

    const { component, styleTags } = renderApp(i18n, initialEditorState);

    const htmlString = template(
      initialEditorState,
      component,
      styleTags,
      initialI18nStoreASC,
      userLng,
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
  let waitTimeout = null;
  fs.watch(manifestFile, (event, filename) => {
    if (filename && event === "change") {
      if (fsWait) return;
      fsWait = true;
      waitTimeout = setTimeout(() => {
        fsWait = false;
        clearTimeout(waitTimeout);
        wss.broadcast("reload");
      }, 100);
    }
  });
} else {
  let assets;

  try {
    assets = getAssets();
  } catch (e) {
    winston.error(e.message);
  }

  app.get("/doceditor", async (req, res) => {
    const { i18n } = req;
    let initialEditorState;

    try {
      initialEditorState = await initDocEditor(req);
    } catch (e) {
      winston.error(e.message);
    }

    const userLng = initialEditorState?.user?.cultureName || "en";

    await i18next.changeLanguage(userLng);
    const initialI18nStoreASC = i18n.services.resourceStore.data;

    if (initialEditorState?.error) {
      winston.error(initialEditorState.error.errorMessage);
    }

    const { component, styleTags } = renderApp(i18n, initialEditorState);

    const htmlString = template(
      initialEditorState,
      component,
      styleTags,
      initialI18nStoreASC,
      userLng,
      assets
    );

    res.send(htmlString);
  });

  app.listen(port, () => {
    winston.info(`Server is listening on port ${port}`);
  });
}
