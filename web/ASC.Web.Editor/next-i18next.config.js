const localesFolder = "./public/locales";
const fs = require("fs");

const availableLocales = fs.readdirSync(localesFolder);

module.exports = {
  i18n: {
    defaultLocale: "en",
    locales: availableLocales,
  },
};
