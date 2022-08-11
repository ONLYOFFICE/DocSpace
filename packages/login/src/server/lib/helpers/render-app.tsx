import React from "react";
import { ServerStyleSheet } from "styled-components";
import GlobalStyle from "../../../client/components/GlobalStyle";
import Login from "../../../client/components/Login";
import { renderToString } from "react-dom/server";
import { I18nextProvider } from "react-i18next";
import i18next from "i18next";

const renderApp = (
  i18n: typeof i18next,
  initialState: IInitialState
): { component: string; styleTags: string } => {
  const sheet = new ServerStyleSheet();
  const component = renderToString(
    sheet.collectStyles(
      <I18nextProvider i18n={i18n}>
        <GlobalStyle />
        <Login {...initialState} />
      </I18nextProvider>
    )
  );
  const styleTags = sheet.getStyleTags();

  return { component, styleTags };
};

export default renderApp;
