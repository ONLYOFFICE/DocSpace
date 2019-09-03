import i18n from "i18next";
import Backend from "i18next-xhr-backend";

const newInstance = i18n.createInstance();

if (process.env.NODE_ENV === "production") {
  newInstance
    .use(Backend)
    .init({
      lng: 'en',
      fallbackLng: "en",
      debug: false,

      interpolation: {
        escapeValue: false, // not needed for react as it escapes by default
        format: function (value, format) {
          if (format === 'lowercase') return value.toLowerCase();
          return value;
        }
      },

      react: {
        useSuspense: false
      },
      backend: {
        loadPath: `/locales/Home/{{lng}}/{{ns}}.json`
      }
    });
} else if (process.env.NODE_ENV === "development") {

  const resources = {
    en: {
      translation: require("./locales/en/translation.json")
    }
  };

  newInstance.init({
    resources: resources,
    lng: 'en',
    fallbackLng: "en",
    debug: true,

    interpolation: {
      escapeValue: false, // not needed for react as it escapes by default
      format: function (value, format) {
        if (format === 'lowercase') return value.toLowerCase();
        return value;
      }
    },

    react: {
      useSuspense: false
    }
  });
}

export default newInstance;