import i18n from "i18next";
import Backend from "i18next-http-backend";
import config from "../package.json";
import { LANGUAGE } from "@appserver/common/constants";
import { loadLanguagePath } from "@appserver/common/utils";

const newInstance = i18n.createInstance();

const lng = localStorage.getItem(LANGUAGE) || "en";

newInstance.use(Backend).init({
  lng: lng,
  fallbackLng: "en",
  load: "all",
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
    allowMultiLoading: false,
    crossDomain: true,
  },

  react: {
    useSuspense: false,
  },
});

export default newInstance;
