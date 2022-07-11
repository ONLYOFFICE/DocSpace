import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import Backend from "@appserver/common/utils/i18next-http-backend";
import config from "../package.json";
import { LANGUAGE } from "@appserver/common/constants";
import { loadLanguagePath } from "@appserver/common/utils";

const newInstance = i18n.createInstance();

const lng = localStorage.getItem(LANGUAGE) || "en";

newInstance
  .use(Backend)
  .use(initReactI18next)
  .init({
    lng: lng,
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

    ns: [
      "DownloadDialog",
      "DeleteDialog",
      "EmptyTrashDialog",
      "ConvertDialog",
      "ConnectDialog",
      "ConflictResolveDialog",
      "DeleteThirdPartyDialog",
      "ThirdPartyMoveDialog",
    ],

    backend: {
      loadPath: loadLanguagePath(config.homepage),
      allowMultiLoading: false,
      crossDomain: true,
    },

    react: {
      useSuspense: false,
    },
  });

export default newInstance;
