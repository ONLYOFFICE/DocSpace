import React from "react";
import { ServerStyleSheet } from "styled-components";
import App from "../../../client/App";
import { renderToString } from "react-dom/server";
import { I18nextProvider, I18nextProviderProps } from "react-i18next";
import { StaticRouter } from "react-router-dom/server";
import GlobalStyle from "../../../client/components/GlobalStyle";
const renderApp = (
  i18n: I18nextProviderProps,
  initialState: IInitialState,
  url: string
): { component: string; styleTags: string } => {
  const sheet = new ServerStyleSheet();

  const component = renderToString(
    sheet.collectStyles(
      <StaticRouter location={url} context={{}}>
        <I18nextProvider i18n={i18n}>
          <GlobalStyle />
          <App {...initialState} />
        </I18nextProvider>
      </StaticRouter>
    )
  );

  const styleTags = sheet.getStyleTags();

  return { component, styleTags };
};

export default renderApp;
