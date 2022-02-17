import React from "react";
import { renderToString } from "react-dom/server";
import App from "../Editor.js";
import { initDocEditor } from "../helpers/utils";
import { ServerStyleSheet } from "styled-components";

const sheet = new ServerStyleSheet();
export default async (req) => {
  const props = await initDocEditor(req);
  const content = renderToString(sheet.collectStyles(<App />));
  const styleTags = sheet.getStyleTags();

  return { props, content, styleTags };
};
