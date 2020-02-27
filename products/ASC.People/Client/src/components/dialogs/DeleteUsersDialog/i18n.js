import i18n from "i18next";
import Backend from "i18next-xhr-backend";
import config from "../../../../package.json";
import { constants } from 'asc-web-common';
const { LANGUAGE } = constants;

const newInstance = i18n.createInstance();

if (process.env.NODE_ENV === "production") {
  newInstance
    .use(Backend)
    .init({
      lng: localStorage.getItem(LANGUAGE) || 'en',
      fallbackLng: "en",

      interpolation: {
        escapeValue: false, // not needed for react as it escapes by default
      },

      react: {
        useSuspense: true
      },
      backend: {
        loadPath: `${config.homepage}/locales/DeleteUsersDialog/{{lng}}/{{ns}}.json`
      }
    });
} else if (process.env.NODE_ENV === "development") {

  const resources = {
    en: {
      translation: require("./locales/en/translation.json")
    },
    ru: {
      translation: require("./locales/ru/translation.json")
    },
  };

  newInstance.init({
    resources: resources,
    lng: localStorage.getItem(LANGUAGE) || 'en',
    fallbackLng: "en",
    debug: true,

    interpolation: {
      escapeValue: false, // not needed for react as it escapes by default
    },

    react: {
      useSuspense: true
    }
  });
}

export default newInstance;