import i18n from 'i18next';
import Backend from 'i18next-xhr-backend';
//import LanguageDetector from 'i18next-browser-languagedetector';
import { initReactI18next } from 'react-i18next';
import config from '../package.json';


i18n
  // load translation using xhr -> see /public/locales
  // learn more: https://github.com/i18next/i18next-xhr-backend
  .use(Backend)
  // detect user language
  // learn more: https://github.com/i18next/i18next-browser-languageDetector
  //.use(LanguageDetector)
  // pass the i18n instance to react-i18next.
  .use(initReactI18next)
  // init i18next
  // for all options read: https://www.i18next.com/overview/configuration-options
  .init({

    lng: 'en',
    fallbackLng: 'en',
    debug: true,
    ns: ['PeopleJSResource', 'PeopleResource', 'Resource', 'UserControlsCommonResource'],

    interpolation: {
      escapeValue: false, // not needed for react as it escapes by default
    },
    backend: {
      loadPath: `${config.homepage}/locales/{{lng}}/{{ns}}.json`,
    },
    react: {
      useSuspense: false
    }
  });

export default i18n;
