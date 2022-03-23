import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import Backend from "i18next-http-backend";
import { LANGUAGE } from "../../constants";
import { loadLanguagePath } from "../../utils";

i18n
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
      loadPath: loadLanguagePath(""),
    },

    react: {
      useSuspense: false,
    },
  });

export default i18n;
