import React from "react";
import ReactDOM from "react-dom";

import { Workbox } from "workbox-window";
import SnackBar from "@docspace/components/snackbar";
import i18n from "i18next";
import { useTranslation, initReactI18next } from "react-i18next";
import Backend from "@docspace/common/utils/i18next-http-backend";
import { LANGUAGE } from "../constants";
import { getCookie } from "../utils";

i18n
  .use(Backend)
  .use(initReactI18next)
  .init({
    lng: getCookie(LANGUAGE) || "en",
    fallbackLng: "en",
    load: "currentOnly",
    //debug: true,

    interpolation: {
      escapeValue: false, // not needed for react as it escapes by default
      format: function (value, format) {
        if (format === "lowercase") return value.toLowerCase();
        return value;
      },
    },

    backend: {
      loadPath: loadLanguagePath(""),
    },

    react: {
      useSuspense: false,
    },
  });

const SnackBarWrapper = (props) => {
  const { t, ready } = useTranslation("Common", { i18n });

  if (ready) {
    const barConfig = {
      parentElementId: "snackbar",
      text: t("Common:NewVersionAvailable"),
      btnText: t("Common:Load"),
      onAction: () => props.onButtonClick(),
      opacity: 1,
      countDownTime: 5 * 60 * 1000,
    };

    return <SnackBar {...barConfig} />;
  }
  return <></>;
};

export default function () {
  if (
    process.env.NODE_ENV !== "production" &&
    !("serviceWorker" in navigator)
  ) {
    console.log("SKIP registerSW because of DEV mode");
    return;
  }

  const wb = new Workbox(`/sw.js`);

  const showSkipWaitingPrompt = (event) => {
    console.log(
      `A new service worker has installed, but it can't activate` +
        `until all tabs running the current version have fully unloaded.`
    );

    function refresh() {
      wb.addEventListener("controlling", () => {
        localStorage.removeItem("sw_need_activation");
        window.location.reload();
      });

      // This will postMessage() to the waiting service worker.
      wb.messageSkipWaiting();
    }

    try {
      const snackbarNode = document.createElement("div");
      snackbarNode.id = "snackbar";
      document.body.appendChild(snackbarNode);

      ReactDOM.render(
        <SnackBarWrapper
          onButtonClick={() => {
            snackbarNode.remove();
            refresh();
          }}
        />,
        document.getElementById("snackbar")
      );

      localStorage.setItem("sw_need_activation", true);
    } catch (e) {
      console.error("showSkipWaitingPrompt", e);
      refresh();
    }
  };

  window.addEventListener("beforeunload", async () => {
    if (localStorage.getItem("sw_need_activation")) {
      localStorage.removeItem("sw_need_activation");
      wb.messageSkipWaiting();
    }
  });

  // Add an event listener to detect when the registered
  // service worker has installed but is waiting to activate.
  wb.addEventListener("waiting", showSkipWaitingPrompt);

  wb.register()
    .then((reg) => {
      console.log("Successful service worker registration", reg);

      if (!window.swUpdateTimer) {
        console.log("SW timer checks for updates every hour");
        window.swUpdateTimer = setInterval(() => {
          console.log("SW update timer check");
          reg.update().catch((e) => {
            console.error("SW update timer FAILED", e);
          });
        }, 60 * 60 * 1000);
      }
    })
    .catch((err) => console.error("Service worker registration failed", err));
}
