import React from "react";
import { ServerStyleSheet } from "styled-components";
import GlobalStyle from "../../../client/components/GlobalStyle";
import App from "../../../client/App";
import { renderToString } from "react-dom/server";

const sheet = new ServerStyleSheet();

const renderApp = () => {
  return renderToString(
    sheet.collectStyles(
      <>
        <GlobalStyle />
        <App />
      </>
    )
  );
};

export default renderApp;

export const getStyleTags = sheet.getStyleTags;
