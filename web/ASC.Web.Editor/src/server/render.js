import React from "react";
import { renderToString } from "react-dom/server";
import App from "../Editor.js";
import { initDocEditor } from "../helpers/utils";
import { ServerStyleSheet } from "styled-components";
import { ChunkExtractor } from "@loadable/server";
import path from "path";
const sheet = new ServerStyleSheet();
const statsFile = path.resolve("./dist/stats.json");
export default async (req) => {
  const props = await initDocEditor(req);

  const extractor = new ChunkExtractor({ statsFile });
  const scriptTags = extractor.getScriptTags();

  const content = renderToString(sheet.collectStyles(<App />));
  const styleTags = sheet.getStyleTags();

  return { props, content, styleTags, scriptTags };
};
