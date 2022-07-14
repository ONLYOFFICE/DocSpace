const express = require("express");
const fs = require("fs");
const path = require("path");
const React = require("react");
const ReactDOMServer = require("react-dom/server");
const compression = require("compression");
const i18nextMiddleware = require("i18next-express-middleware");
const i18next = require("i18next");
const Backend = require("i18next-fs-backend");
const process = require("process");
const { ServerStyleSheet } = require("styled-components");
const pkg = require("../../package.json");
const title = pkg.title;
const { App } = require("../client/App");
const initDocEditor = require("./lib/initDocEditor");

const loadPath = (lng, ns) => {
  let resourcePath = path.resolve(
    process.cwd(),
    `dist/locales/${lng}/${ns}.json`
  );
  if (ns === "Common")
    resourcePath = path.join(
      __dirname,
      `../../../../public/locales/${lng}/${ns}.json`
    );

  return resourcePath;
};

const app = express();
const port = process.env.PORT || 5013;

i18next.use(Backend).init({
  backend: {
    loadPath: loadPath,
    allowMultiLoading: true,
    crossDomain: false,
  },
  fallbackLng: "en",
  load: "currentOnly",

  saveMissing: true,
  ns: ["Editor", "Common"],
  defaultNS: "Editor",

  interpolation: {
    escapeValue: false, // not needed for react as it escapes by default
    format: function (value, format) {
      if (format === "lowercase") return value.toLowerCase();
      return value;
    },
  },
});

app.set("view engine", "ejs");
app.set("views", path.join(__dirname, "views"));

// app.get(
//   /\.(js|css|map|ico)$/,
//   express.static(path.resolve(process.cwd(), "dist"))
// );

app.use(i18nextMiddleware.handle(i18next));
app.use(compression());

app.use(
  "/products/files/doceditor/",
  express.static(path.resolve(process.cwd(), "dist"))
);
//app.use(express.static(path.resolve(process.cwd(), "dist")));

app.get("/products/files/doceditor", async (req, res) => {
  const manifest = fs.readFileSync(
    path.join(process.cwd(), "dist/manifest.json"),
    "utf-8"
  );
  const initialState = await initDocEditor(req);

  const assets = JSON.parse(manifest);
  const sheet = new ServerStyleSheet();
  const component = ReactDOMServer.renderToString(
    sheet.collectStyles(React.createElement(App))
  );
  const styleTags = sheet.getStyleTags();
  const userLng = initialState?.props?.user?.cultureName || "en";
  const initialI18nStore = {};

  i18next.changeLanguage(userLng).then(() => {
    const initialLanguage = userLng;
    const usedNamespaces = req.i18n.reportNamespaces
      ? req.i18n.reportNamespaces.getUsedNamespaces()
      : ["Common", "Editor"];

    initialI18nStore[initialLanguage] = {};
    usedNamespaces.forEach((namespace) => {
      initialI18nStore[initialLanguage][namespace] =
        req.i18n.services.resourceStore.data[initialLanguage][namespace];
    });

    const parsedI18nStore = JSON.stringify(initialI18nStore);
    const parsedInitialState = JSON.stringify(initialState);
    const docApiUrl = initialState?.props?.docApiUrl || "";

    res.render("editor", {
      assets,
      component,
      title,
      parsedI18nStore,
      initialLanguage,
      parsedInitialState,
      docApiUrl,
      styleTags,
    });
  });
});

const server = app.listen(port, () => {
  console.log(`Server is listening on port <http://localhost:${port}>`);
});

if (process.env.NODE_ENV === "development") {
  const ws = require("./websocket");

  const wss = ws(server);
  const manifestFile = path.resolve(process.cwd(), "dist/manifest.json");

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
