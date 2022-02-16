import React from "react";
import { renderToString } from "react-dom/server";
import App from "../Editor.js";
import { initDocEditor } from "../helpers/utils";

export default async (req) => {
  const props = await initDocEditor(req);
  const content = renderToString(<App />);

  return { props, content };
};
