import React from "react";
import { ServerStyleSheet } from "styled-components";
import GlobalStyle from "../../../client/components/GlobalStyle";
import Login from "../../../client/components/Login";
import { renderToString } from "react-dom/server";
import { I18nextProvider } from "react-i18next";
import i18next from "i18next";

const sheet = new ServerStyleSheet();

const renderApp = (
  i18n: typeof i18next,
  initialState: IInitialState
): string => {
  return renderToString(
    sheet.collectStyles(
      <I18nextProvider i18n={i18n}>
        <GlobalStyle />
        <Login {...initialState} />
      </I18nextProvider>
    )
  );
};

export default renderApp;

export const getStyleTags = sheet.getStyleTags;
