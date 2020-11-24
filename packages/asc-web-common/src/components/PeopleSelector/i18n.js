import i18n from "i18next";
import en from "./locales/en/translation.json";
import ru from "./locales/ru/translation.json";
import { i18nBaseSettings } from "../../constants";

const newInstance = i18n.createInstance();

const resources = {
  en: {
    translation: en,
  },
  ru: {
    translation: ru,
  },
};

newInstance.init({ ...i18nBaseSettings, resources });

export default newInstance;

// newInstance.init({
//   resources: resources,
//   lng: localStorage.getItem(LANGUAGE) || 'en',
//   fallbackLng: "en",

//   interpolation: {
//     escapeValue: false, // not needed for react as it escapes by default
//     format: function (value, format) {
//       if (format === 'lowercase') return value.toLowerCase();
//       return value;
//     }
//   },

//   react: {
//     useSuspense: false
//   }
// });
