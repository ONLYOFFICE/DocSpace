import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import Backend from "i18next-http-backend";
import config from "../package.json";
import { LANGUAGE } from "@appserver/common/constants";
import { loadLanguagePath } from "@appserver/common/utils";

const newInstance = i18n.createInstance();

const lng = localStorage.getItem(LANGUAGE) || "en";

newInstance
  .use(initReactI18next)
  .use(Backend)
  .init({
    lng: lng,
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
      loadPath: loadLanguagePath(config.homepage),
      allowMultiLoading: true,
      crossDomain: false,
    },

    ns: ["Editor", "Common"],
    defaultNS: "Editor",

    react: {
      useSuspense: true,
    },
  });

export default newInstance;
