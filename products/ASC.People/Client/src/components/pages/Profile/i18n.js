import i18n from 'i18next';
import Backend from 'i18next-xhr-backend';
import config from '../../../../package.json';

const newInstance = i18n.createInstance();

newInstance
  .use(Backend)
  .init({
    fallbackLng: 'en',
    debug: true,
    backend: {
      loadPath: `${config.homepage}/locales/Profile/{{lng}}/{{ns}}.json`,
    },

    interpolation: {
      escapeValue: false, // not needed for react as it escapes by default
    },

    react: {
      useSuspense: true
    }
  });

export default newInstance;
