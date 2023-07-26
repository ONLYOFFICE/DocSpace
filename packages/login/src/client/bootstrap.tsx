import React from "react";
import { hydrateRoot } from "react-dom/client";
// import { registerSW } from "@docspace/common/sw/helper";
import pkg from "../../package.json";
import { initI18n } from "./helpers/utils";
import ClientApp from "./components/ClientApp";

const propsObj: IInitialState = window.__ASC_INITIAL_LOGIN_STATE__;
const initialI18nStoreASC: IInitialI18nStoreASC = window.initialI18nStoreASC;
const initialLanguage = window.initialLanguage;

const isDesktopEditor = window["AscDesktopEditor"] !== undefined;
if (isDesktopEditor) typeof window["AscDesktopEditor"];

initI18n(initialI18nStoreASC);

const container = document.getElementById("root");
if (container) {
  hydrateRoot(
    container,
    <ClientApp
      initialLanguage={initialLanguage}
      initialI18nStoreASC={initialI18nStoreASC}
      isDesktopEditor={isDesktopEditor}
      {...propsObj}
    />
  );
}

if (IS_DEVELOPMENT) {
  const port = PORT || 5011;
  const socketPath = pkg.socketPath;

  const ws = new WebSocket(`ws://localhost:${port}${socketPath}`);
  let isErrorConnection = false;

  ws.onopen = (event) => {
    console.log("[login-dev] Socket is connected. Live reload enabled");
  };

  ws.onmessage = function (event) {
    if (event.data === "reload") {
      console.log("[login-dev] App updated. Reloading...");
      location.reload();
    }
  };

  ws.onclose = function (event) {
    console.log("[login-dev] Socket is disconnected! Reloading...");
    setTimeout(() => {
      !isErrorConnection && location.reload();
    }, 1500);
  };

  ws.onerror = (event) => {
    isErrorConnection = true;
    console.log("[login-dev] Socket connect error!");
  };
}

// registerSW();
