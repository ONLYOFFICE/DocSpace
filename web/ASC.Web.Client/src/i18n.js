import i18n from "i18next";
import Backend from "i18next-http-backend";
import config from "../package.json";
import { LANGUAGE } from "@appserver/common/constants";

const newInstance = i18n.createInstance();

newInstance.use(Backend).init({
  lng: localStorage.getItem(LANGUAGE) || "en",
  fallbackLng: "en",
  load: "languageOnly",
  //debug: true,

  interpolation: {
    escapeValue: false, // not needed for react as it escapes by default
    format: function (value, format) {
      if (format === "lowercase") return value.toLowerCase();
      return value;
    },
  },

  backend: {
    loadPath: `${config.homepage}/locales/{{lng}}/{{ns}}.json`,
  },

  react: {
    useSuspense: false,
  },
});

export default newInstance;
