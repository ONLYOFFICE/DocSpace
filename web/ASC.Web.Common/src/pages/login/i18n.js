import i18n from "i18next";
import en from "./locales/en/translation.json";
import ru from "./locales/ru/translation.json";
import { i18nBaseSettings } from "../../constants";

const newInstance = i18n.createInstance();

const resources = {
  en: {
    translation: en,
  },
  ru: {
    translation: ru,
  },
};

newInstance.init({ ...i18nBaseSettings, resources });

export default newInstance;
