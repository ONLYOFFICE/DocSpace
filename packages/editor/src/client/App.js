import React, { useEffect } from "react";
import Editor from "./components/Editor.js";
import { useSSR } from "react-i18next";
import useMfScripts from "./helpers/useMfScripts";
import {
  combineUrl,
  isRetina,
  getCookie,
  setCookie,
} from "@docspace/common/utils";
import initDesktop from "./helpers/initDesktop";
import ErrorBoundary from "./components/ErrorBoundary";
import store from "client/store";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import { fonts } from "@docspace/common/fonts.js";
import GlobalStyle from "./components/GlobalStyle.js";
import { inject, observer, Provider as MobxProvider } from "mobx-react";
import ThemeProvider from "@docspace/components/theme-provider";

const isDesktopEditor = window["AscDesktopEditor"] !== undefined;

import PresentationIcoUrl from "../../../../public/images/presentation.ico";
import SpreadSheetIcoUrl from "../../../../public/images/spreadsheet.ico";
import TextIcoUrl from "../../../../public/images/text.ico";

const ThemeProviderWrapper = inject(({ auth }) => {
  const { settingsStore } = auth;
  return { theme: settingsStore.theme };
})(observer(ThemeProvider));

const App = ({ initialLanguage, initialI18nStoreASC, ...rest }) => {
  const [isInitialized, isErrorLoading] = useMfScripts();
  useSSR(initialI18nStoreASC, initialLanguage);

  console.log(rest);

  useEffect(() => {
    let icon = "";

    switch (rest.config.documentType) {
      case "word":
        icon = TextIcoUrl;
        break;
      case "slide":
        icon = PresentationIcoUrl;
        break;
      case "cell":
        icon = SpreadSheetIcoUrl;
        break;
      default:
        break;
    }

    if (icon) {
      const el = document.getElementById("favicon");

      el.href = icon;
    }

    console.log(icon);
  }, [rest.config.documentType]);

  useEffect(() => {
    const tempElm = document.getElementById("loader");

    if (tempElm && !rest.error && !rest.needLoader && rest?.config?.editorUrl) {
      tempElm.outerHTML = "";
    }

    if (isRetina() && getCookie("is_retina") == null) {
      setCookie("is_retina", true, { path: "/" });
    }
  }, []);

  const onError = () => {
    window.open(
      combineUrl(
        window.DocSpaceConfig?.proxy?.url,
        rest.personal ? "sign-in" : "/login"
      ),
      "_self"
    );
  };

  return (
    <ErrorBoundary onError={onError}>
      <MobxProvider {...store}>
        <I18nextProvider i18n={i18n}>
          <ThemeProviderWrapper>
            <GlobalStyle fonts={fonts} />
            <Editor
              mfReady={isInitialized}
              mfFailed={isErrorLoading}
              isDesktopEditor={isDesktopEditor}
              initDesktop={initDesktop}
              {...rest}
            />
          </ThemeProviderWrapper>
        </I18nextProvider>
      </MobxProvider>
    </ErrorBoundary>
  );
};

export default App;
