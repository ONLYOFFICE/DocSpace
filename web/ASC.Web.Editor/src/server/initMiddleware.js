import React from "react";
import { renderToString } from "react-dom/server";
import App from "../client/Editor.js";
import { initDocEditor } from "../helpers/utils";
import { ServerStyleSheet, createGlobalStyle } from "styled-components";
import path from "path";
import { I18nextProvider } from "react-i18next";
import fs from "fs";

const GlobalStyle = createGlobalStyle`
  html,
  body {
    height: 100%;
  }

  #root {
    min-height: 100%;

    .pageLoader {
      position: fixed;
      left: calc(50% - 20px);
      top: 35%;
    }
  }
  body {
    margin: 0;
  }

  body.loading * {
    cursor: wait !important;
  }
`;

const sheet = new ServerStyleSheet();

export default async (req, res, next) => {
  const manifest = fs.readFileSync(
    path.join(process.cwd(), "dist/client/manifest.json"),
    "utf-8"
  );

  req.initialState = await initDocEditor(req);
  req.assets = JSON.parse(manifest);

  req.appComponent = renderToString(
    sheet.collectStyles(
      <I18nextProvider i18n={req.i18n}>
        <GlobalStyle />
        <App {...req.initialState} />
      </I18nextProvider>
    )
  );
  req.styleTags = sheet.getStyleTags();

  next();
};
