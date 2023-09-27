import LoginPageSvgUrl from "PUBLIC_DIR/images/logo/loginpage.svg?url";
import DarkLoginPageSvgUrl from "PUBLIC_DIR/images/logo/dark_loginpage.svg?url";
import LeftMenuSvgUrl from "PUBLIC_DIR/images/logo/leftmenu.svg?url";
import DocseditorSvgUrl from "PUBLIC_DIR/images/logo/docseditor.svg?url";
import LightSmallSvgUrl from "PUBLIC_DIR/images/logo/lightsmall.svg?url";
import DocsEditoRembedSvgUrl from "PUBLIC_DIR/images/logo/docseditorembed.svg?url";
import DarkLightSmallSvgUrl from "PUBLIC_DIR/images/logo/dark_lightsmall.svg?url";
import FaviconIco from "PUBLIC_DIR/favicon.ico";

import BackgroundPatternReactSvgUrl from "PUBLIC_DIR/images/background.pattern.react.svg?url";
import BackgroundPatternOrangeReactSvgUrl from "PUBLIC_DIR/images/background.pattern.orange.react.svg?url";
import BackgroundPatternGreenReactSvgUrl from "PUBLIC_DIR/images/background.pattern.green.react.svg?url";
import BackgroundPatternRedReactSvgUrl from "PUBLIC_DIR/images/background.pattern.red.react.svg?url";
import BackgroundPatternPurpleReactSvgUrl from "PUBLIC_DIR/images/background.pattern.purple.react.svg?url";
import BackgroundPatternLightBlueReactSvgUrl from "PUBLIC_DIR/images/background.pattern.lightBlue.react.svg?url";
import BackgroundPatternBlackReactSvgUrl from "PUBLIC_DIR/images/background.pattern.black.react.svg?url";

import moment from "moment";

import { LANGUAGE, ThemeKeys, RtlLanguages } from "../constants";
import sjcl from "sjcl";
import { isMobile } from "react-device-detect";
import TopLoaderService from "@docspace/components/top-loading-indicator";
import { Encoder } from "./encoder";
import FilesFilter from "../api/files/filter";
import combineUrlFunc from "./combineUrl";
// import { translations } from "./i18next-http-backend/lib/translations";
export const toUrlParams = (obj, skipNull) => {
  let str = "";
  for (var key in obj) {
    if (skipNull && !obj[key]) continue;

    if (str !== "") {
      str += "&";
    }

    // added for double employeetype
    if (Array.isArray(obj[key]) && key === "employeetypes") {
      for (let i = 0; i < obj[key].length; i++) {
        str += key + "=" + encodeURIComponent(obj[key][i]);
        if (i !== obj[key].length - 1) {
          str += "&";
        }
      }
    } else if (typeof obj[key] === "object") {
      str += key + "=" + encodeURIComponent(JSON.stringify(obj[key]));
    } else {
      str += key + "=" + encodeURIComponent(obj[key]);
    }
  }

  return str;
};

export const decodeDisplayName = (items) => {
  return items.map((item) => {
    if (!item) return item;

    if (item.updatedBy?.displayName) {
      item.updatedBy.displayName = Encoder.htmlDecode(
        item.updatedBy.displayName
      );
    }
    if (item.createdBy?.displayName) {
      item.createdBy.displayName = Encoder.htmlDecode(
        item.createdBy.displayName
      );
    }
    return item;
  });
};

export function getObjectByLocation(location) {
  if (!location.search || !location.search.length) return null;

  const searchUrl = location.search.substring(1);
  const decodedString = decodeURIComponent(searchUrl)
    .replace(/\["/g, '["')
    .replace(/"\]/g, '"]')
    .replace(/"/g, '\\"')
    .replace(/&/g, '","')
    .replace(/=/g, '":"')
    .replace(/\\/g, "\\\\")
    .replace(/\[\\\\"/g, '["')
    .replace(/\\\\"\]/g, '"]')
    .replace(/"\[/g, "[")
    .replace(/\]"/g, "]")
    .replace(/\\\\",\\\\"/g, '","')
    .replace(/\\\\\\\\"/g, '\\"');

  try {
    const object = JSON.parse(`{"${decodedString}"}`);

    return object;
  } catch (e) {
    return {};
  }
}

export function changeLanguage(i18n, currentLng = getCookie(LANGUAGE)) {
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

export function isMe(user, userName) {
  return (
    user && user.id && (userName === "@self" || user.userName === userName)
  );
}

export function isAdmin(currentUser) {
  return (
    currentUser.isAdmin ||
    currentUser.isOwner ||
    currentUser?.listAdminModules?.length > 0
  );
}

export const getUserRole = (user) => {
  if (user.isOwner) return "owner";
  else if (isAdmin(user))
    //TODO: Change to People Product Id const
    return "admin";
  //TODO: Need refactoring
  else if (user.isVisitor) return "user";
  else if (user.isCollaborator) return "collaborator";
  else return "manager";
};

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

export function getProviderTranslation(provider, t, linked = false, signUp = false) {
  const capitalizeProvider =
    provider.charAt(0).toUpperCase() + provider.slice(1);
  if (linked) {
    return `${t("Common:Disconnect")} ${capitalizeProvider}`;
  }

  switch (provider) {
    case "apple":
      return signUp ? t("Common:SignUpWithApple") : t("Common:SignInWithApple");
    case "google":
      return signUp ? t("Common:SignUpWithGoogle") : t("Common:SignInWithGoogle");
    case "facebook":
      return signUp ? t("Common:SignUpWithFacebook") : t("Common:SignInWithFacebook");
    case "twitter":
      return signUp ? t("Common:SignUpWithTwitter") : t("Common:SignInWithTwitter");
    case "linkedin":
      return signUp ? t("Common:SignUpWithLinkedIn") : t("Common:SignInWithLinkedIn");
    case "microsoft":
      return signUp ? t("Common:SignUpWithMicrosoft") : t("Common:SignInWithMicrosoft");
    case "sso":
      return signUp ? t("Common:SignUpWithSso") : t("Common:SignInWithSso");
    case "zoom":
      return signUp ? t("Common:SignUpWithZoom") : t("Common:SignInWithZoom");
  }
}

export function getLanguage(lng) {
  try {
    if (!lng) return lng;

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

export const isLanguageRtl = (lng: string) => {
  if (!lng) return;

  const splittedLng = lng.split("-");
  return RtlLanguages.includes(splittedLng[0]);
};

// temporary function needed to replace rtl language in Editor to ltr
export const getLtrLanguageForEditor = (
  userLng: string,
  portalLng: string,
  isEditor: boolean = false
): string => {
  let isEditorPath;
  if (typeof window !== "undefined") {
    isEditorPath = window?.location.pathname.indexOf("doceditor") !== -1;
  }
  const isUserLngRtl = isLanguageRtl(userLng);
  const isPortalLngRtl = isLanguageRtl(portalLng);

  if ((!isEditor && !isEditorPath) || (userLng && !isUserLngRtl))
    return userLng;
  if (portalLng && !isPortalLngRtl) return portalLng;

  return "en";
};

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

export function isRetina() {
  if (window.devicePixelRatio > 1) return true;

  var mediaQuery =
    "(-webkit-min-device-pixel-ratio: 1.5),\
      (min--moz-device-pixel-ratio: 1.5),\
      (-o-min-device-pixel-ratio: 3/2),\
      (min-resolution: 1.5dppx),\
      (min-device-pixel-ratio: 1.5)";

  if (window.matchMedia && window.matchMedia(mediaQuery).matches) return true;
  return false;
}

export function convertLanguage(key) {
  switch (key) {
    case "en":
      return "en-GB";
    case "ru-RU":
      return "ru";
    case "de-DE":
      return "de";
    case "it-IT":
      return "it";
    case "fr-FR":
      return "fr";
  }

  return key;
}

export function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export function isElementInViewport(el) {
  if (!el) return;

  const rect = el.getBoundingClientRect();

  return (
    rect.top >= 0 &&
    rect.left >= 0 &&
    rect.bottom <=
    (window.innerHeight || document.documentElement.clientHeight) &&
    rect.right <= (window.innerWidth || document.documentElement.clientWidth)
  );
}

export function assign(obj, keyPath, value) {
  const lastKeyIndex = keyPath.length - 1;
  for (let i = 0; i < lastKeyIndex; ++i) {
    const key = keyPath[i];
    if (!(key in obj)) {
      obj[key] = {};
    }
    obj = obj[key];
  }
  obj[keyPath[lastKeyIndex]] = value;
}

export function getOAuthToken(
  tokenGetterWin: Window | string | null
): Promise<string> {
  return new Promise((resolve, reject) => {
    localStorage.removeItem("code");
    let interval: ReturnType<typeof setInterval>;
    interval = setInterval(() => {
      try {
        const code = localStorage.getItem("code");
        if (typeof tokenGetterWin !== "string") {
          if (code) {
            localStorage.removeItem("code");
            clearInterval(interval);
            resolve(code);
          } else if (tokenGetterWin && tokenGetterWin.closed) {
            clearInterval(interval);
            reject();
          }
        }
      } catch (e) {
        clearInterval(interval);
        reject(e);
      }
    }, 500);
  });
}

export function getLoginLink(token: string, code: string) {
  return combineUrl(
    window.DocSpaceConfig?.proxy?.url,
    `/login.ashx?p=${token}&code=${code}`
  );
}

export function checkIsSSR() {
  return typeof window === "undefined";
}

export const frameCallbackData = (methodReturnData: any) => {
  window.parent.postMessage(
    JSON.stringify({
      type: "onMethodReturn",
      methodReturnData,
    }),
    "*"
  );
};

export const frameCallEvent = (eventReturnData: any) => {
  window.parent.postMessage(
    JSON.stringify({
      type: "onEventReturn",
      eventReturnData,
    }),
    "*"
  );
};

export const frameCallCommand = (commandName: string, commandData: any) => {
  window.parent.postMessage(
    JSON.stringify({
      type: "onCallCommand",
      commandName,
      commandData,
    }),
    "*"
  );
};

export const getConvertedSize = (t, bytes) => {
  let power = 0,
    resultSize = bytes;

  const sizeNames = [
    t("Common:Bytes"),
    t("Common:Kilobyte"),
    t("Common:Megabyte"),
    t("Common:Gigabyte"),
    t("Common:Terabyte"),
    t("Common:Petabyte"),
    t("Common:Exabyte"),
  ];

  if (bytes <= 0) return `${"0" + " " + t("Common:Bytes")}`;

  if (bytes >= 1024) {
    power = Math.floor(Math.log(bytes) / Math.log(1024));
    power = power < sizeNames.length ? power : sizeNames.length - 1;
    resultSize = parseFloat((bytes / Math.pow(1024, power)).toFixed(2));
  }

  return resultSize + " " + sizeNames[power];
};

export const getBgPattern = (colorSchemeId: number | undefined) => {
  switch (colorSchemeId) {
    case 1:
      return `url('${BackgroundPatternReactSvgUrl}')`;
    case 2:
      return `url('${BackgroundPatternOrangeReactSvgUrl}')`;
    case 3:
      return `url('${BackgroundPatternGreenReactSvgUrl}')`;
    case 4:
      return `url('${BackgroundPatternRedReactSvgUrl}')`;
    case 5:
      return `url('${BackgroundPatternPurpleReactSvgUrl}')`;
    case 6:
      return `url('${BackgroundPatternLightBlueReactSvgUrl}')`;
    case 7:
      return `url('${BackgroundPatternBlackReactSvgUrl}')`;
    default:
      return `url('${BackgroundPatternReactSvgUrl}')`;
  }
};

export const getLogoFromPath = (path) => {
  if (!path || path.indexOf("images/logo/") === -1) return path;

  const name = path.split("/").pop();

  switch (name) {
    case "aboutpage.svg":
    case "loginpage.svg":
      return LoginPageSvgUrl;
    case "dark_loginpage.svg":
      return DarkLoginPageSvgUrl;
    case "leftmenu.svg":
    case "dark_leftmenu.svg":
      return LeftMenuSvgUrl;
    case "dark_aboutpage.svg":
    case "dark_lightsmall.svg":
      return DarkLightSmallSvgUrl;
    case "docseditor.svg":
      return DocseditorSvgUrl;
    case "lightsmall.svg":
      return LightSmallSvgUrl;
    case "docseditorembed.svg":
      return DocsEditoRembedSvgUrl;
    case "favicon.ico":
      return FaviconIco;
    default:
      break;
  }

  return path;
};

export const getDaysLeft = (date) => {
  return moment(date).startOf("day").diff(moment().startOf("day"), "days");
};
export const getDaysRemaining = (autoDelete) => {
  let daysRemaining = getDaysLeft(autoDelete);

  if (daysRemaining <= 0) return "<1";
  return "" + daysRemaining;
};

export const checkFilterInstance = (filterObject, certainClass) => {
  const isInstance =
    filterObject.constructor.name === certainClass.prototype.constructor.name;

  if (!isInstance)
    throw new Error(
      `Filter ${filterObject.constructor.name} isn't an instance of   ${certainClass.prototype.constructor.name}`
    );

  return isInstance;
};

export const getFileExtension = (fileTitle: string) => {
  if (!fileTitle) {
    return "";
  }
  fileTitle = fileTitle.trim();
  const posExt = fileTitle.lastIndexOf(".");
  return 0 <= posExt ? fileTitle.substring(posExt).trim().toLowerCase() : "";
};

export const getSystemTheme = () => {
  const isDesktopClient = window["AscDesktopEditor"] !== undefined;
  const desktopClientTheme = window?.RendererProcessVariable?.theme;
  const isDark =
    desktopClientTheme?.id === "theme-dark" ||
    desktopClientTheme?.id === "theme-contrast-dark" ||
    (desktopClientTheme?.id === "theme-system" &&
      desktopClientTheme?.system === "dark");

  return isDesktopClient
    ? isDark
      ? ThemeKeys.DarkStr
      : ThemeKeys.BaseStr
    : window.matchMedia &&
      window.matchMedia("(prefers-color-scheme: dark)").matches
      ? ThemeKeys.DarkStr
      : ThemeKeys.BaseStr;
};
