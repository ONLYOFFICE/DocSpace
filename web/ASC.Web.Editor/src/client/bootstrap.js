import React, { Suspense } from "react";
import { hydrate } from "react-dom";
import { registerSW } from "@appserver/common/sw/helper";
import { App } from "./App.js";
import { useSSR } from "react-i18next";
import useMfScripts from "../helpers/useMfScripts";
import initDesktop from "../helpers/initDesktop";
import { AppServerConfig } from "@appserver/common/constants";
import { combineUrl } from "@appserver/common/utils";
import ErrorBoundary from "../components/ErrorBoundary";
import pkg from "../../package.json";
import { isRetina, getCookie, setCookie } from "@appserver/common/utils";
import { fonts } from "@appserver/common/fonts.js";
import GlobalStyle from "../components/GlobalStyle.js";

const propsObj = window.__ASC_INITIAL_EDITOR_STATE__;
const initialI18nStoreASC = window.initialI18nStoreASC;
const initialLanguage = window.initialLanguage;

if (initialI18nStoreASC && !window.i18n) {
  const { homepage } = pkg;

  window.i18n = {};
  window.i18n.inLoad = [];
  window.i18n.loaded = {};

  for (let lng in initialI18nStoreASC) {
    for (let ns in initialI18nStoreASC[lng]) {
      if (ns === "Common") {
        window.i18n.loaded[`/static/locales/${lng}/${ns}.json`] = {
          namespaces: ns,
          data: initialI18nStoreASC[lng][ns],
          work: true,
        };
      } else {
        window.i18n.loaded[`${homepage}/locales/${lng}/${ns}.json`] = {
          namespaces: ns,
          data: initialI18nStoreASC[lng][ns],
          work: true,
        };
      }
    }
  }
}

delete window.__ASC_INITIAL_EDITOR_STATE__;
delete window.initialI18nStoreASC;
delete window.initialLanguage;

const stateInit = document.getElementById("__ASC_INITIAL_EDITOR_STATE__");
const i18nInit = document.getElementById("__ASC_INITIAL_EDITOR_I18N__");
stateInit.parentNode.removeChild(stateInit);
i18nInit.parentNode.removeChild(i18nInit);

const isDesktopEditor = window["AscDesktopEditor"] !== undefined;

const AppWrapper = () => {
  const [isInitialized, isErrorLoading] = useMfScripts();
  useSSR(initialI18nStoreASC, initialLanguage);

  React.useEffect(() => {
    const tempElm = document.getElementById("loader");
    if (tempElm) {
      tempElm.outerHTML = "";
    }

    if (isRetina() && getCookie("is_retina") == null) {
      setCookie("is_retina", true, { path: "/" });
    }
  }, []);
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
      {/* <Suspense fallback={<div />}> */}
      <GlobalStyle fonts={fonts} />
      <App
        {...propsObj}
        mfReady={isInitialized}
        mfFailed={isErrorLoading}
        isDesktopEditor={isDesktopEditor}
        initDesktop={initDesktop}
      />
      {/* </Suspense> */}
    </ErrorBoundary>
  );
};

hydrate(<AppWrapper />, document.getElementById("root"));

if (IS_DEVELOPMENT) {
  const port = PORT || 5013;
  const socketPath = pkg.socketPath;

  const ws = new WebSocket(`ws://localhost:${port}${socketPath}`);
  let isErrorConnection = false;

  ws.onopen = (event) => {
    console.log("[editor-dev] Socket is connected. Live reload enabled");
  };

  ws.onmessage = function (event) {
    if (event.data === "reload") {
      console.log("[editor-dev] App updated. Reloading...");
      location.reload();
    }
  };

  ws.onclose = function (event) {
    console.log("close");
    console.log("[editor-dev] Socket is disconnected! Reloading...");
    setTimeout(() => {
      !isErrorConnection && location.reload();
    }, 1500);
  };

  ws.onerror = (event) => {
    isErrorConnection = true;
    console.log("[editor-dev] Socket connect error!");
  };
}

registerSW();
