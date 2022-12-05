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
import { AppServerConfig, ThemeKeys } from "@docspace/common/constants";
import initDesktop from "./helpers/initDesktop";
import ErrorBoundary from "./components/ErrorBoundary";
import store from "client/store";
import i18n from "./i18n";
import { I18nextProvider } from "react-i18next";
import { fonts } from "@docspace/common/fonts.js";
import GlobalStyle from "./components/GlobalStyle.js";
import { Provider as MobxProvider } from "mobx-react";
import ThemeProvider from "@docspace/components/theme-provider";
import DeepLink from "./components/DeepLink.js";

const isDesktopEditor = window["AscDesktopEditor"] !== undefined;

const App = ({ initialLanguage, initialI18nStoreASC, ...rest }) => {
  const [isInitialized, isErrorLoading] = useMfScripts();
  useSSR(initialI18nStoreASC, initialLanguage);

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
        AppServerConfig.proxyURL,
        rest.personal ? "sign-in" : "/login"
      ),
      "_self"
    );
  };

  const showDeepLink = !isDesktopEditor && rest?.config?.file?.encrypted;

  return (
    <ErrorBoundary onError={onError}>
      <MobxProvider {...store}>
        <I18nextProvider i18n={i18n}>
          <ThemeProvider theme={rest?.theme}>
            <GlobalStyle fonts={fonts} />
            {showDeepLink ? (
              <DeepLink
                currentColorScheme={rest?.currentColorScheme}
                whiteLabelLogoUrls={rest?.whiteLabelLogoUrls}
              />
            ) : (
              <Editor
                mfReady={isInitialized}
                mfFailed={isErrorLoading}
                isDesktopEditor={isDesktopEditor}
                initDesktop={initDesktop}
                {...rest}
              />
            )}
          </ThemeProvider>
        </I18nextProvider>
      </MobxProvider>
    </ErrorBoundary>
  );
};

export default App;
