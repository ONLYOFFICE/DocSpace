import React from "react";
import { ServerStyleSheet } from "styled-components";
import GlobalStyle from "../../../client/components/GlobalStyle";
import Editor from "../../../client/components/Editor";
import { renderToString } from "react-dom/server";
import { I18nextProvider } from "react-i18next";

const renderApp = (i18n, initialState) => {
  const sheet = new ServerStyleSheet();
  const component = renderToString(
    sheet.collectStyles(
      <React.Suspense fallback={<div></div>}>
        <I18nextProvider i18n={i18n}>
          <GlobalStyle />
          <Editor {...initialState} />
        </I18nextProvider>
      </React.Suspense>
    )
  );

  const styleTags = sheet.getStyleTags();

  return { component, styleTags };
};

export default renderApp;
