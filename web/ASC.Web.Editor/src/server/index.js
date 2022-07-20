import express from "express";
import template from "./render";
import initMiddleware from "./initMiddleware";
import i18nextMiddleware from "i18next-express-middleware";
import i18next from "i18next";
import Backend from "i18next-fs-backend";
import path from "path";
import compression from "compression";
import ws from "./websocket";
import fs from "fs";
import logger from "morgan";
import winston from "./logger.js";

winston.stream = {
  write: (message) => winston.info(message),
};

const loadPath = (lng, ns) => {
  let resourcePath =
    path.resolve(process.cwd(), "dist/client") + `/locales/${lng}/${ns}.json`;
  if (ns === "Common")
    resourcePath = path.resolve(
      process.cwd(),
      `../../public/locales/${lng}/${ns}.json`
    );

  return resourcePath;
};

const port = PORT || 5013;
const app = express();

const fallbackLng = "en";

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
  "/products/files/doceditor/",
  express.static(path.resolve(process.cwd(), "dist/client"))
);
//app.use(express.static(path.resolve(process.cwd(), "dist/client")));
app.use(initMiddleware);
app.use(logger("dev", { stream: winston.stream }));
app.get("/products/files/doceditor", async (req, res) => {
  const { i18n, initialEditorState, appComponent, styleTags, assets } = req;
  const userLng = initialEditorState?.user?.cultureName || "en";

  await i18next.changeLanguage(userLng);
  const initialI18nStoreASC = i18n.services.resourceStore.data;

  const htmlString = template(
    initialEditorState,
    appComponent,
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

if (IS_DEVELOPMENT) {
  const wss = ws(server);

  const manifestFile = path.resolve(process.cwd(), "dist/client/manifest.json");

  let fsWait = false;
  fs.watch(manifestFile, (event, filename) => {
    if (filename && event === "change") {
      if (fsWait) return;
      fsWait = true;
      fsWait = setTimeout(() => {
        fsWait = false;
      }, 100);

      wss.broadcast("reload");
    }
  });
}
