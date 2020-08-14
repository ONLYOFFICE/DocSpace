import i18n from "i18next";
import Backend from "i18next-xhr-backend";
import { constants } from "asc-web-common";
const { i18nBaseSettings } = constants;
//const fs = require("fs");
const path = require("path");

// function getFolderList(dir) {
//   return fs
//     .readdirSync(dir)
//     .filter(file => fs.lstatSync(path.join(dir, file)).isDirectory());
// }

/**
 * @description create i18n instance
 * @param {object} object with method,url,data etc.
 */
export const createI18N = function(options) {
  const { page, localesPath } = options;

  const newInstance = i18n.createInstance();

  if (process.env.NODE_ENV === "production") {
    newInstance.use(Backend).init({
      ...i18nBaseSettings,
      backend: {
        loadPath: `/locales/${page}/{{lng}}/{{ns}}.json`
      }
    });
  } else if (process.env.NODE_ENV === "development") {
    const resources = {};

    const path = require("path");

    //const folders = getFolderList(localesPath);

    /* 


        translation: require("../components/pages/About/locales/en/translation.json")
                              ../components/pages/About/locales/en/translation.json
*/

    i18nBaseSettings.supportedLngs.forEach(name => {
      resources[name] = {
        translation: require(`../components/${localesPath}/locales/${name}/translation.json`) //`${localesPath}/${name}/translation.json`)
      };
    });

    // const resources = {
    //   en: {
    //     translation: require("./locales/en/translation.json")
    //   },
    //   ru: {
    //     translation: require("./locales/ru/translation.json")
    //   }
    // };

    newInstance.init({ ...i18nBaseSettings, resources });
  }

  return newInstance;
};
