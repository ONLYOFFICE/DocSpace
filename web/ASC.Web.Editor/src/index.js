import express from "express";
import path from "path";
import template from "./server/template";
import render from "./server/render";
import i18nextMiddleware from "i18next-express-middleware";
import i18next from "i18next";
import Backend from "i18next-node-fs-backend";

const app = express();
const port = process.env.PORT || 5013;

i18next.use(Backend).init({
  backend: {
    loadPath:
      path.resolve(process.cwd(), "clientBuild") +
      "/locales/{{lng}}/{{ns}}.json",
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

app.use(i18nextMiddleware.handle(i18next));

app.use(
  "/products/files/doceditor/static/",
  express.static(path.resolve(__dirname, "../clientBuild/static"))
);
app.use(express.static(path.resolve(__dirname, "../clientBuild")));

app.get("/products/files/doceditor", async (req, res) => {
  const { props, content, styleTags, scriptTags } = await render(req);
  const userLng = props?.user?.cultureName || "en";

  i18next.changeLanguage(userLng).then(() => {
    const initialLanguage = userLng;
    const initialI18nStore = {};
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
      scriptTags,
      initialI18nStore,
      initialLanguage
    );
    res.setHeader("Cache-Control", "assets, max-age=604800");

    res.send(response);
  });
});

app.listen(port, (err) => {
  console.log(`Server is listening on port ${port}`);
});
