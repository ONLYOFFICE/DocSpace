import { saveToSessionStorage, getFromSessionStorage } from "../utils";

export const resetSessionStorage = () => {
  const portalNameFromSessionStorage = getFromSessionStorage("portalName");
  const portalNameDefaultFromSessionStorage = getFromSessionStorage("portalNameDefault");

  const greetingTitleFromSessionStorage = getFromSessionStorage("greetingTitle");
  const greetingTitleDefaultFromSessionStorage = getFromSessionStorage("greetingTitleDefault");

  const languageFromSessionStorage = getFromSessionStorage("language");
  const languageDefaultFromSessionStorage = getFromSessionStorage("languageDefault");
  const timezoneFromSessionStorage = getFromSessionStorage("timezone");
  const timezoneDefaultFromSessionStorage = getFromSessionStorage("timezoneDefault");

  if (portalNameFromSessionStorage !== portalNameDefaultFromSessionStorage) {
    saveToSessionStorage("portalName", "none");
    saveToSessionStorage("errorValue", null);
  }
  if (greetingTitleFromSessionStorage !== greetingTitleDefaultFromSessionStorage) {
    saveToSessionStorage("greetingTitle", "none");
    saveToSessionStorage("greetingTitleDefault", "none");
  }
  if (languageFromSessionStorage !== languageDefaultFromSessionStorage) {
    saveToSessionStorage("language", "");
  }
  if (timezoneFromSessionStorage !== timezoneDefaultFromSessionStorage) {
    saveToSessionStorage("timezone", "");
  }
};
