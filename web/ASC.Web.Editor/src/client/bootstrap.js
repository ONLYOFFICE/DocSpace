import React, { Suspense } from "react";
import { hydrate, render } from "react-dom";
import { registerSW } from "@appserver/common/sw/helper";
import App from "./App.js";
import { useSSR } from "react-i18next";
import useMfScripts from "../helpers/useMfScripts";
import initDesktop from "../helpers/initDesktop";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import ErrorBoundary from "../components/ErrorBoundary";
import { loadableReady } from "@loadable/component";

const propsObj = window.__ASC_INITIAL_STATE__;
const initialI18nStore = window.initialI18nStore;
const initialLanguage = window.initialLanguage;

const isDesktopEditor = window["AscDesktopEditor"] !== undefined;

const AppWrapper = () => {
  const [isInitialized, isErrorLoading] = useMfScripts();
  useSSR(initialI18nStore, initialLanguage);

  const onError = () =>
    window.open(
      combineUrl(
        AppServerConfig.proxyURL,
        propsObj.personal ? "sign-in" : "/login"
      ),
      "_self"
    );

  return (
    <ErrorBoundary onError={onError}>
      <Suspense fallback={<div />}>
        <App
          {...propsObj}
          mfReady={isInitialized}
          mfFailed={isErrorLoading}
          isDesktopEditor={isDesktopEditor}
          initDesktop={initDesktop}
        />
      </Suspense>
    </ErrorBoundary>
  );
};

//hydrate(<AppWrapper />, document.getElementById("root"));

const renderApp = () => {
  const rootContent = document.getElementById("root");
  const renderMethod = module.hot ? render : hydrate;

  if (typeof window !== undefined) {
    window.testWork = true;
  }
  renderMethod(
    <AppWrapper />,

    rootContent
  );
};

loadableReady(() => {
  renderApp();
});

if (module.hot) {
  module.hot.accept();
}

registerSW();
