import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import Backend from "i18next-http-backend";
import config from "../package.json";
import { constants } from "asc-web-common";
const { LANGUAGE /*i18nBaseSettings*/ } = constants;

//import LanguageDetector from "i18next-browser-languagedetector";
// not like to use this?
// have a look at the Quick start guide
// for passing in lng and translations on init

const languages = ["en", "ru"];

i18n
  /*
     load translation using http -> see /public/locales (i.e. https://github.com/i18next/react-i18next/tree/master/example/react/public/locales)
     learn more: https://github.com/i18next/i18next-http-backend
    */
  .use(Backend)
  /*
     detect user language
     learn more: https://github.com/i18next/i18next-browser-languageDetector
    */
  //.use(LanguageDetector)
  /*
     pass the i18n instance to react-i18next.
    */
  .use(initReactI18next)
  /*
     init i18next
     for all options read: https://www.i18next.com/overview/configuration-options
    */
  .init({
    lng: localStorage.getItem(LANGUAGE) || "en",
    supportedLngs: languages,
    whitelist: languages,
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

export default i18n;
