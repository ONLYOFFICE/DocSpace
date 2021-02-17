import { LANGUAGE } from "../constants";
import sjcl from "sjcl";
import { isMobile } from "react-device-detect";

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

export function changeLanguage(
  i18n,
  currentLng = localStorage.getItem(LANGUAGE)
) {
  return currentLng
    ? i18n.language !== currentLng
      ? i18n.changeLanguage(currentLng)
      : Promise.resolve((...args) => i18n.t(...args))
    : i18n.changeLanguage("en");
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

export function updateTempContent(isAuth = false) {
  if (isAuth) {
    const el = document.getElementById("burger-loader-svg");
    if (el) {
      el.style.display = "block";
    }

    const el1 = document.getElementById("logo-loader-svg");
    if (el1) {
      el1.style.display = "block";
    }

    const el2 = document.getElementById("avatar-loader-svg");
    if (el2) {
      el2.style.display = "block";
    }
  } else {
    const tempElm = document.getElementById("temp-content");
    if (tempElm) {
      tempElm.outerHTML = "";
    }
  }
}

export function hideLoader() {
  if (isMobile) return;

  if (window.loadingTimeout) {
    clearTimeout(window.loadingTimeout);
    window.loadingTimeout = null;
  }

  document.body.classList.remove("loading");
}

export function showLoader() {
  if (isMobile) return;

  window.loadingTimeout = setTimeout(() => {
    document.body.classList.add("loading");
  }, 1000);
}

export { withLayoutSize } from "./withLayoutSize";

export function tryRedirectTo(page) {
  if (
    window.location.pathname === page ||
    window.location.pathname.indexOf(page) !== -1
  ) {
    return false;
  }
  //TODO: check if we already on default page
  window.location.replace(page);

  return true;
}

export function isMe(user, userName) {
  return (
    user && user.id && (userName === "@self" || user.userName === userName)
  );
}

export function isAdmin(currentUser, currentProductId) {
  let productName = null;

  switch (currentProductId) {
    case "f4d98afd-d336-4332-8778-3c6945c81ea0":
      productName = "people";
      break;
    case "e67be73d-f9ae-4ce1-8fec-1880cb518cb4":
      productName = "documents";
      break;
    default:
      break;
  }

  const isProductAdmin =
    currentUser.listAdminModules && productName
      ? currentUser.listAdminModules.includes(productName)
      : false;

  return currentUser.isAdmin || currentUser.isOwner || isProductAdmin;
}
