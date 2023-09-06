import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import Backend from "@docspace/common/utils/i18next-http-backend";
import config from "../../package.json";
import { LANGUAGE } from "@docspace/common/constants";
import { getLtrLanguageForEditor, getCookie } from "@docspace/common/utils";
import { loadLanguagePath } from "./helpers/utils";

const newInstance = i18n.createInstance();

const userLng = getCookie(LANGUAGE) || "en";
const portalLng = window?.__ASC_INITIAL_EDITOR_STATE__?.portalSettings.culture;

newInstance
  .use(Backend)
  .use(initReactI18next)
  .init({
    lng: getLtrLanguageForEditor(userLng, portalLng, true),
    fallbackLng: "en",
    load: "currentOnly",
    //debug: true,

    interpolation: {
      escapeValue: false, // not needed for react as it escapes by default
      format: function (value, format) {
        if (format === "lowercase") return value.toLowerCase();
        return value;
      },
    },

    backend: {
      loadPath: loadLanguagePath(config.homepage),
      allowMultiLoading: true,
      crossDomain: false,
    },

    ns: ["Editor", "Common", "DeepLink"],
    defaultNS: "Editor",

    react: {
      useSuspense: true,
    },
  });

export default newInstance;
