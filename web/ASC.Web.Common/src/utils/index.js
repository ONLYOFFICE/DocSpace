import { AUTH_KEY, LANGUAGE } from "../constants";
import sjcl from "sjcl";

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

export function changeLanguage(i18n, currentLng = localStorage.getItem(LANGUAGE)) {
  return currentLng
    ? i18n.language !== currentLng
      ? i18n.changeLanguage(currentLng)
      : Promise.resolve((...args) => i18n.t(...args))
    : i18n.changeLanguage("en");
}

export function redirectToDefaultPage() {
  if (
    (window.location.pathname === "/" ||
      window.location.pathname === "" ||
      window.location.pathname === "/login") &&
    localStorage.getItem(AUTH_KEY) !== null
  ) {
    setTimeout(() => window.location.replace("/products/files"), 0);
    return true;
  }

  return false;
}

export function createPasswordHash(password, hashSettings) {
  if (
    !password ||
    !hashSettings ||
    typeof password !== "string" ||
    typeof hashSettings !== "object" ||
    !hashSettings.hasOwnProperty("salt") ||
    !hashSettings.hasOwnProperty("size") ||
    !hashSettings.hasOwnProperty("iterations") ||
    typeof hashSettings.size !== "number" ||
    typeof hashSettings.iterations !== "number" ||
    typeof hashSettings.salt !== "string"
  )
    throw new Error("Invalid params.");

  const { size, iterations, salt } = hashSettings;

  let bits = sjcl.misc.pbkdf2(password, salt, iterations);
  bits = bits.slice(0, size / 32);
  const hash = sjcl.codec.hex.fromBits(bits);

  return hash;
}

export function removeTempContent() {
  const tempElm = document.getElementById("temp-content");
  if (tempElm) {
    tempElm.outerHTML = "";
  }
}

export function hideLoader() {
  const ele = document.getElementById("ipl-progress-indicator");
  if (ele) {
    // fade out
    ele.classList.add("available");
    ele.style.display = "";
    // setTimeout(() => {
    //   // remove from DOM
    //   ele.outerHTML = "";
    // }, 2000);
  }
}

export function showLoader() {
  const ele = document.getElementById("ipl-progress-indicator");
  if (ele) {
    ele.classList.remove("available");
    ele.style.display = "block";
  }
}
