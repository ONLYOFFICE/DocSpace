import React from "react";
import { renderToString } from "react-dom/server";
import App from "../client/Editor.js";
import { initDocEditor } from "../helpers/utils";
import { ServerStyleSheet, createGlobalStyle } from "styled-components";
import { ChunkExtractor, ChunkExtractorManager } from "@loadable/server";
import path from "path";
import { I18nextProvider } from "react-i18next";

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
//const statsFile = path.resolve("clientBuild/stats.json");
const loadableJson = path.resolve(__dirname, "./loadable-stats.json");
export default async (req) => {
  const props = await initDocEditor(req);

  //const extractor = new ChunkExtractor({ statsFile });
  const extractor = new ChunkExtractor({
    statsFile: loadableJson,
    entrypoints: ["client"],
  });
  const scriptTags = extractor.getScriptTags();

  const content = renderToString(
    sheet.collectStyles(
      <ChunkExtractorManager extractor={extractor}>
        <GlobalStyle />
        <I18nextProvider i18n={req.i18n}>
          <App />
        </I18nextProvider>
      </ChunkExtractorManager>
    )
  );
  const styleTags = sheet.getStyleTags();

  return { ...props, content, styleTags, extractor };
};
