import { LANGUAGE } from "../constants";
import sjcl from "sjcl";
import { isMobile } from "react-device-detect";
import history from "../history";
import TopLoaderService from "@appserver/components/top-loading-indicator";
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

let timer = null;

export function hideLoader() {
  if (isMobile) return;
  if (timer) {
    clearTimeout(timer);
    timer = null;
  }
  TopLoaderService.end();
}

export function showLoader() {
  if (isMobile) return;

  hideLoader();

  timer = setTimeout(() => TopLoaderService.start(), 500);
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
  //window.location.replace(page);
  history.push(page);

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
      productName = "files";
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

import combineUrlFunc from "./combineUrl";

export const combineUrl = combineUrlFunc;

export function getCookie(name) {
  let matches = document.cookie.match(
    new RegExp(
      "(?:^|; )" +
        name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, "\\$1") +
        "=([^;]*)"
    )
  );
  return matches ? decodeURIComponent(matches[1]) : undefined;
}

export function setCookie(name, value, options = {}) {
  options = {
    path: "/",
    ...options,
  };

  if (options.expires instanceof Date) {
    options.expires = options.expires.toUTCString();
  }

  let updatedCookie =
    encodeURIComponent(name) + "=" + encodeURIComponent(value);

  for (let optionKey in options) {
    updatedCookie += "; " + optionKey;
    let optionValue = options[optionKey];
    if (optionValue !== true) {
      updatedCookie += "=" + optionValue;
    }
  }

  document.cookie = updatedCookie;
}

export function deleteCookie(name) {
  setCookie(name, "", {
    "max-age": -1,
  });
}

export function clickBackdrop() {
  var elms = document.getElementsByClassName("backdrop-active");
  if (elms && elms.length > 0) {
    elms[0].click();
  }
}

export function objectToGetParams(object) {
  const params = Object.entries(object)
    .filter(([, value]) => value !== undefined && value !== null)
    .map(
      ([key, value]) =>
        `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`
    );

  return params.length > 0 ? `?${params.join("&")}` : "";
}

export function toCommunityHostname(hostname) {
  let communityHostname;
  try {
    communityHostname =
      hostname.indexOf("m.") > -1
        ? hostname.substring(2, hostname.length)
        : hostname;
  } catch (e) {
    console.error(e);
    communityHostname = hostname;
  }

  return communityHostname;
}

export function getProviderTranslation(provider, t) {
  switch (provider) {
    case "google":
      return t("Common:SignInWithGoogle");
    case "facebook":
      return t("Common:SignInWithFacebook");
    case "twitter":
      return t("Common:SignInWithTwitter");
    case "linkedin":
      return t("Common:SignInWithLinkedIn");
  }
}

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

    if (ns.length > 0 && ns[0] === "Common") {
      return `/static/locales/${language}/Common.json`;
    }
    return `${homepage}/locales/${language}/${fixedNS || ns}.json`;
  };
}

export function loadScript(url, id, onLoad, onError) {
  try {
    const script = document.createElement("script");
    script.setAttribute("type", "text/javascript");
    script.setAttribute("id", id);

    if (onLoad) script.onload = onLoad;
    if (onError) script.onerror = onError;

    script.src = url;
    script.async = true;

    document.body.appendChild(script);
  } catch (e) {
    console.error(e);
  }
}
