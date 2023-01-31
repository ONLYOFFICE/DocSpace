import { translations } from "../../translations";
import pkg from "../../../package.json";
import { thirdPartyLogin } from "@docspace/common/utils/loginUtils";

export function getLanguage(lng: string) {
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

export function loadLanguagePath(homepage: string, fixedNS = null) {
  return (lng: string | [string], ns: string) => {
    const language = getLanguage(lng instanceof Array ? lng[0] : lng);

    const lngCollection = translations.get(language);

    let path = "";

    if (ns.length > 0 && ns[0] === "Common") {
      return (path = lngCollection?.get("Common").replace("/login/", "/"));
    }
    path = lngCollection?.get(`${fixedNS || ns}`);

    return path;
  };
}

export function initI18n(initialI18nStoreASC: IInitialI18nStoreASC): void {
  if (!initialI18nStoreASC || window.i18n) return;

  const i18n = {
    inLoad: [],
    loaded: {},
  };
  window.i18n = i18n;

  for (let lng in initialI18nStoreASC) {
    const collection = translations.get(lng);

    for (let ns in initialI18nStoreASC[lng]) {
      if (ns === "Common") {
        window.i18n.loaded[
          `${collection?.get(ns)?.replace("/login/", "/")}`
        ] = {
          namespaces: ns,
          data: initialI18nStoreASC[lng][ns],
        };
      } else {
        window.i18n.loaded[`${collection?.get(ns)}`] = {
          namespaces: ns,
          data: initialI18nStoreASC[lng][ns],
        };
      }
    }
  }
}

export async function oAuthLogin(profile: string) {
  let isSuccess = false;
  try {
    await thirdPartyLogin(profile);
    isSuccess = true;
    const redirectPath = localStorage.getItem("redirectPath");

    if (redirectPath) {
      localStorage.removeItem("redirectPath");
      window.location.href = redirectPath;
    }
  } catch (e) {
    isSuccess = false;
    return isSuccess;
  }

  localStorage.removeItem("profile");
  localStorage.removeItem("code");

  return isSuccess;
}
