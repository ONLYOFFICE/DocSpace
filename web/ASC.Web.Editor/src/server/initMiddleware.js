import React from "react";
import { renderToString } from "react-dom/server";
import App from "../client/Editor.js";
import { initDocEditor } from "../helpers/utils";
import { ServerStyleSheet } from "styled-components";
import path from "path";
import { I18nextProvider } from "react-i18next";
import fs from "fs";
import GlobalStyle from "../components/GlobalStyle.js";
import winston from "./logger.js";

winston.stream = {
  write: (message) => winston.info(message),
};

const sheet = new ServerStyleSheet();

export default async (req, res, next) => {
  const manifest = fs.readFileSync(
    path.join(__dirname, "client/manifest.json"),
    "utf-8"
  );
  try {
    req.initialEditorState = await initDocEditor(req);
    req.assets = JSON.parse(manifest);

    req.appComponent = renderToString(
      sheet.collectStyles(
        <I18nextProvider i18n={req.i18n}>
          <GlobalStyle />
          <App {...req.initialEditorState} />
        </I18nextProvider>
      )
    );
    req.styleTags = sheet.getStyleTags();
  } catch (e) {
    winston.error(e.message);
  }
  next();
};
