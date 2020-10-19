import i18n from "i18next";
import Backend from "i18next-xhr-backend";
import { constants } from "asc-web-common";
const { i18nBaseSettings } = constants;

/**
 * @description create i18n instance
 * @param {object} object with method,url,data etc.
 */
export const createI18N = function (options) {
  const { page, localesPath, forceBackend } = options;

  const newInstance = i18n.createInstance();

  if (process.env.NODE_ENV === "production" || forceBackend) {
    newInstance.use(Backend).init({
      ...i18nBaseSettings,
      backend: {
        loadPath: `/locales/${page}/{{lng}}/{{ns}}.json`,
      },
    });
  } else if (process.env.NODE_ENV === "development") {
    const resources = {};

    i18nBaseSettings.supportedLngs.forEach((name) => {
      resources[name] = {
        translation: require(`../components/${localesPath}/locales/${name}/translation.json`),
      };
    });

    newInstance.init({ ...i18nBaseSettings, resources });
  }

  return newInstance;
};
