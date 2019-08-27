import i18n from 'i18next';
import Backend from 'i18next-xhr-backend';

const newInstance = i18n.createInstance();

newInstance
  .use(Backend)
  .init({
    lng: 'ru',
    fallbackLng: 'en',
    debug: true,
    ns: ['Resource', 'translation'],
    backend: {
      loadPath: `/products/people/locales/Profile/{{lng}}/{{ns}}.json`,
    },

    interpolation: {
      escapeValue: false, // not needed for react as it escapes by default
    },

    react: {
      useSuspense: true
    }
  });

export default newInstance;
