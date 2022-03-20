import React, { Suspense, useState, useEffect } from "react";
import { hydrate } from "react-dom";
import { registerSW } from "@appserver/common/sw/helper";
import App from "../App.js";
import { useSSR } from "react-i18next";
import "../i18n";
import { FILES_SCOPE } from "../helpers/constants.js";

const propsObj = window.__ASC_INITIAL_STATE__;
const initialI18nStore = window.initialI18nStore;
const initialLanguage = window.initialLanguage;
const remoteScript = document.getElementById(FILES_SCOPE);

delete window.__ASC_INITIAL_STATE__;
delete window.initialI18nStore;
delete window.initialLanguage;

const stateJS = document.getElementById("__ASC_INITIAL_STATE__");
stateJS.parentNode.removeChild(stateJS);

const AppWrapper = () => {
  const [isReadyFilesRemote, setIsReadyFilesRemote] = useState(false);
  useSSR(initialI18nStore, initialLanguage);

  useEffect(() => {
    remoteScript.onload = () => setIsReadyFilesRemote(true);
  }, []);

  return (
    <Suspense fallback={<div />}>
      <App {...propsObj} isReadyFilesRemote={isReadyFilesRemote} />
    </Suspense>
  );
};

hydrate(<AppWrapper />, document.getElementById("root"));

registerSW();
