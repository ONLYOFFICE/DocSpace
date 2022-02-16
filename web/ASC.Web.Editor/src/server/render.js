import React from "react";
import { renderToString } from "react-dom/server";
import App from "../Editor.js";
import { initDocEditor } from "./helpers/utils";

export default async (req) => {
  const props = await initDocEditor(req);

  let content = renderToString(<App />);

  // Get a copy of store data to create the same store on client side

  return { app, content };
};
