import { LANGUAGE } from '../constants';

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
}

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

export function changeLanguage(i18nInstance) {
  return localStorage.getItem(LANGUAGE) 
  ? (i18nInstance.language !== localStorage.getItem(LANGUAGE) 
    ? i18nInstance.changeLanguage(localStorage.getItem(LANGUAGE))
    : Promise.resolve((...args) => i18nInstance.t(...args)))
  : i18nInstance.changeLanguage('en');
}
