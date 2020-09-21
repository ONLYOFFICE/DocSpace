import { AUTH_KEY, LANGUAGE } from "../constants";

export const toUrlParams = (obj, skipNull) => {
  let str = "";
  for (var key in obj) {
    if (skipNull && !obj[key]) continue;

    if (str !== "") {
      str += "&";
    }

    str += key + "=" + encodeURIComponent(obj[key]);
  }

  return str;
};

export function getObjectByLocation(location) {
  if (!location.search || !location.search.length) return null;

  const searchUrl = location.search.substring(1);
  const object = JSON.parse(
    '{"' +
      decodeURIComponent(searchUrl)
        .replace(/"/g, '\\"')
        .replace(/&/g, '","')
        .replace(/=/g, '":"') +
      '"}'
  );

  return object;
}

export function changeLanguage(i18n) {
  const currentLng = localStorage.getItem(LANGUAGE);
  return currentLng
    ? i18n.language !== currentLng
      ? i18n.changeLanguage(currentLng)
      : Promise.resolve((...args) => i18n.t(...args))
    : i18n.changeLanguage("en");
}

export function changeDocumentTitle(title) {
  if (title !== document.title) {
    document.title = title;
  }
}

export function redirectToDefaultPage() {
  if (
    (window.location.pathname === "/" || window.location.pathname === "") &&
    localStorage.getItem(AUTH_KEY) !== null
  ) {
    setTimeout(() => window.location.replace("/products/files"), 0);
    return true;
  }

  return false;
}
