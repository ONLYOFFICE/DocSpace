import React, { Suspense } from "react";
import { hydrate } from "react-dom";
import { registerSW } from "@appserver/common/sw/helper";
import App from "../App.js";
import { useSSR } from "react-i18next";
import useMfScripts from "../helpers/useMfScripts";
import initDesktop from "../helpers/initDesktop";

const propsObj = window.__ASC_INITIAL_STATE__;
const initialI18nStore = window.initialI18nStore;
const initialLanguage = window.initialLanguage;

delete window.__ASC_INITIAL_STATE__;
delete window.initialI18nStore;
delete window.initialLanguage;

const stateInit = document.getElementById("__ASC_INITIAL_STATE__");
const i18nInit = document.getElementById("__ASC_I18N_INIT__");
stateInit.parentNode.removeChild(stateInit);
i18nInit.parentNode.removeChild(i18nInit);
const isDesktopEditor = window["AscDesktopEditor"] !== undefined;

const AppWrapper = () => {
  const [isInitialized, isErrorLoading] = useMfScripts();
  useSSR(initialI18nStore, initialLanguage);

  return (
    <Suspense fallback={<div />}>
      <App
        {...propsObj}
        mfReady={isInitialized}
        mfFailed={isErrorLoading}
        isDesktopEditor={isDesktopEditor}
        initDesktop={initDesktop}
      />
    </Suspense>
  );
};

hydrate(<AppWrapper />, document.getElementById("root"));

registerSW();
