import { initReactI18next } from "react-i18next";
import HttpBackend from "i18next-http-backend";
import i18n from "i18next";

const newInstance = i18n.createInstance();

newInstance
  .use(HttpBackend)
  .use(initReactI18next)
  .init({
    load: "currentOnly",
    ns: ["Common"],
    defaultNS: "Common",
    backend: {
      backendOptions: [
        {
          loadPath: "../../client/public/locales/{{lng}}/{{ns}}.json",
        },
        {
          loadPath: "../../../public/locales/{{lng}}/{{ns}}.json",
        },
      ],
    },
    lng: "en",
    fallbackLng: "en",
    interpolation: {
      escapeValue: false,
    },
  });

export default newInstance;
