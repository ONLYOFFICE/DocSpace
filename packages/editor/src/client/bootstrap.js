import React from "react";
import { hydrateRoot } from "react-dom/client";
// import { registerSW } from "@docspace/common/sw/helper";
import App from "./App.js";
import pkg from "../../package.json";
import { initI18n } from "./helpers/utils.js";

const propsObj = window.__ASC_INITIAL_EDITOR_STATE__;
const initialI18nStoreASC = window.initialI18nStoreASC;
const initialLanguage = window.initialLanguage;

initI18n(initialI18nStoreASC);

const container = document.getElementById("root");
if (container) {
  hydrateRoot(
    container,
    <React.Suspense fallback={<div></div>}>
      <App
        initialLanguage={initialLanguage}
        initialI18nStoreASC={initialI18nStoreASC}
        {...propsObj}
      />
    </React.Suspense>
  );
}
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

// registerSW();
