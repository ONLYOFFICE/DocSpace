import i18next from "i18next";
import Backend from "i18next-fs-backend";
import { loadPath } from "./lib/helpers";

const fallbackLng = "en";

i18next.use(Backend).init({
  backend: {
    loadPath: loadPath,
    allowMultiLoading: true,
    crossDomain: false,
  },
  fallbackLng: fallbackLng,
  load: "currentOnly",

  saveMissing: true,
  ns: ["Login", "Common"],
  defaultNS: "Login",

  interpolation: {
    escapeValue: false,
    format: function (value, format) {
      if (format === "lowercase") return value.toLowerCase();
      return value;
    },
  },
});

export default i18next;
