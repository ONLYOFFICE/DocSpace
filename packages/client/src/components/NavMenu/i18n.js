import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import Backend from "@docspace/common/utils/i18next-http-backend";
import { LANGUAGE } from "@docspace/common/constants";
import config from "PACKAGE_FILE";
import { loadLanguagePath } from "@docspace/common/utils";

const newInstance = i18n.createInstance();

newInstance
  .use(Backend)
  .use(initReactI18next)
  .init({
    lng: localStorage.getItem(LANGUAGE) || "en",
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
      loadPath: loadLanguagePath(config.homepage, "NavMenu"),
    },

    react: {
      useSuspense: false,
    },
  });

export default newInstance;
