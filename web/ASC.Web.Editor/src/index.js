import express from "express";
import template from "./server/template";
import render from "./server/render";
import i18nextMiddleware from "i18next-express-middleware";
import i18next from "i18next";
import Backend from "i18next-fs-backend";
import path, { join } from "path";
import compression from "compression";
// import webpack from "webpack";
// import WebpackDevMiddleware from "webpack-dev-middleware";
// import WebpackHotMiddleware from "webpack-hot-middleware";

const loadPath = (lng, ns) => {
  let resourcePath =
    path.resolve(process.cwd(), "dist/client") + `/locales/${lng}/${ns}.json`;

  if (ns === "Common")
    resourcePath = join(__dirname, `../../../public/locales/${lng}/${ns}.json`);

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

// if (process.env.NODE_ENV === "development") {
//   const webpackConfig = require("../webpack/dev/webpack.dev.client.js");
//   const compiler = webpack(webpackConfig);
//   console.log("server");
//   app.use(
//     WebpackDevMiddleware(compiler, {
//       publicPath: webpackConfig.output.publicPath,
//       serverSideRender: true,
//     })
//   );

//   app.use(WebpackHotMiddleware(compiler));
// }

app.use(i18nextMiddleware.handle(i18next));
app.use(compression());
app.use(
  "/products/files/doceditor/static/",
  express.static(path.resolve(__dirname, "../dist/client/static"))
);
app.use(express.static(path.resolve(__dirname, "../dist/client")));

app.get("/products/files/doceditor", async (req, res) => {
  const { props, content, styleTags, extractor } = await render(req);
  const userLng = props?.user?.cultureName || "en";
  const initialI18nStore = {};

  i18next.changeLanguage(userLng).then(() => {
    const initialLanguage = userLng;

    const usedNamespaces = req.i18n.reportNamespaces.getUsedNamespaces();

    initialI18nStore[initialLanguage] = {};

    usedNamespaces.forEach((namespace) => {
      initialI18nStore[initialLanguage][namespace] =
        req.i18n.services.resourceStore.data[initialLanguage][namespace];
    });

    const response = template(
      props,
      content,
      styleTags,
      initialI18nStore,
      initialLanguage,
      extractor
    );

    res.send(response);
  });
});

app.listen(port, () => {
  console.log(`Server is listening on port ${port}`);
});
