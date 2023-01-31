import pkg from "../../../package.json";
import { translations } from "./translations";

export function getLanguage(lng) {
  try {
    let language = lng == "en-US" || lng == "en-GB" ? "en" : lng;

    const splitted = lng.split("-");

    if (splitted.length == 2 && splitted[0] == splitted[1].toLowerCase()) {
      language = splitted[0];
    }

    return language;
  } catch (error) {
    console.error(error);
  }

  return lng;
}

export function loadLanguagePath(homepage, fixedNS = null) {
  return (lng, ns) => {
    const language = getLanguage(lng instanceof Array ? lng[0] : lng);

    const lngCollection = translations.get(language);

    let path = "";

    if (ns.length > 0 && ns[0] === "Common") {
      path = lngCollection?.get("Common");
    }
    path = lngCollection?.get(`${fixedNS || ns}`);

    return path;
  };
}

export const initI18n = (initialI18nStoreASC) => {
  if (!initialI18nStoreASC || window.i18n) return;

  window.i18n = {};
  window.i18n.inLoad = [];
  window.i18n.loaded = {};

  for (let lng in initialI18nStoreASC) {
    const collection = translations.get(lng);

    for (let ns in initialI18nStoreASC[lng]) {
      window.i18n.loaded[`${collection?.get(ns)}`] = {
        namespaces: ns,
        data: initialI18nStoreASC[lng][ns],
      };
    }
  }
};
