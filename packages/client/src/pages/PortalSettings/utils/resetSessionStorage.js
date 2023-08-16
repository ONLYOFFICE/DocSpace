import { saveToSessionStorage, getFromSessionStorage } from "../utils";

export const resetSessionStorage = () => {
  const portalNameFromSessionStorage = getFromSessionStorage("portalName");
  const portalNameDefaultFromSessionStorage = getFromSessionStorage(
    "portalNameDefault"
  );
  const greetingTitleFromSessionStorage = getFromSessionStorage(
    "greetingTitle"
  );
  const greetingTitleDefaultFromSessionStorage = getFromSessionStorage(
    "greetingTitleDefault"
  );
  const languageFromSessionStorage = getFromSessionStorage("language");
  const languageDefaultFromSessionStorage = getFromSessionStorage(
    "languageDefault"
  );
  const timezoneFromSessionStorage = getFromSessionStorage("timezone");
  const timezoneDefaultFromSessionStorage = getFromSessionStorage(
    "timezoneDefault"
  );

  const selectColorId = getFromSessionStorage("selectColorId");
  const defaultColorId = getFromSessionStorage("defaultColorId");
  const selectColorAccent = getFromSessionStorage("selectColorAccent");
  const defaultColorAccent = getFromSessionStorage("defaultColorAccent");

  const currentPasswordSettings = getFromSessionStorage(
    "currentPasswordSettings"
  );
  const defaultPasswordSettings = getFromSessionStorage(
    "defaultPasswordSettings"
  );
  const currentTfaSettings = getFromSessionStorage("currentTfaSettings");
  const defaultTfaSettings = getFromSessionStorage("defaultTfaSettings");
  const currentTrustedMailSettings = getFromSessionStorage(
    "currentTrustedMailSettings"
  );
  const defaultTrustedMailSettings = getFromSessionStorage(
    "defaultTrustedMailSettings"
  );
  const currentIPSettings = getFromSessionStorage("currentIPSettings");
  const defaultIPSettings = getFromSessionStorage("defaultIPSettings");
  const currentAdminMessageSettings = getFromSessionStorage(
    "currentAdminMessageSettings"
  );
  const defaultAdminMessageSettings = getFromSessionStorage(
    "defaultAdminMessageSettings"
  );
  const currentSessionLifetimeSettings = getFromSessionStorage(
    "currentSessionLifetimeSettings"
  );
  const defaultSessionLifetimeSettings = getFromSessionStorage(
    "defaultSessionLifetimeSettings"
  );
  const storagePeriodSettings = getFromSessionStorage("storagePeriod");
  const defaultStoragePeriodSettings = getFromSessionStorage(
    "defaultStoragePeriod"
  );

  const companyNameFromeSessionStorage = getFromSessionStorage("companyName");
  const companySettingsFromSessionStorage = getFromSessionStorage(
    "companySettings"
  );
  const defaultCompanySettingsFromSessionStorage = getFromSessionStorage(
    "defaultCompanySettings"
  );
  const additionalSettings = getFromSessionStorage("additionalSettings");
  const defaultAdditionalSettings = getFromSessionStorage(
    "defaultAdditionalSettings"
  );

  if (portalNameFromSessionStorage !== portalNameDefaultFromSessionStorage) {
    saveToSessionStorage("portalName", "none");
    saveToSessionStorage("errorValue", null);
  }
  if (
    greetingTitleFromSessionStorage !== greetingTitleDefaultFromSessionStorage
  ) {
    saveToSessionStorage("greetingTitle", "none");
  }
  if (languageFromSessionStorage !== languageDefaultFromSessionStorage) {
    saveToSessionStorage("language", languageDefaultFromSessionStorage);
  }
  if (timezoneFromSessionStorage !== timezoneDefaultFromSessionStorage) {
    saveToSessionStorage("timezone", timezoneDefaultFromSessionStorage);
  }
  if (currentPasswordSettings !== defaultPasswordSettings) {
    saveToSessionStorage("currentPasswordSettings", defaultPasswordSettings);
  }
  if (currentTfaSettings !== defaultTfaSettings) {
    saveToSessionStorage("currentTfaSettings", defaultTfaSettings);
  }
  if (currentTrustedMailSettings !== defaultTrustedMailSettings) {
    saveToSessionStorage(
      "currentTrustedMailSettings",
      defaultTrustedMailSettings
    );
  }
  if (currentIPSettings !== defaultIPSettings) {
    saveToSessionStorage("currentIPSettings", defaultIPSettings);
  }
  if (currentAdminMessageSettings !== defaultAdminMessageSettings) {
    saveToSessionStorage(
      "currentAdminMessageSettings",
      defaultAdminMessageSettings
    );
  }
  if (currentSessionLifetimeSettings !== defaultSessionLifetimeSettings) {
    saveToSessionStorage(
      "currentSessionLifetimeSettings",
      defaultSessionLifetimeSettings
    );
  }
  if (storagePeriodSettings !== defaultStoragePeriodSettings) {
    saveToSessionStorage("storagePeriod", defaultStoragePeriodSettings);
  }

  sessionStorage.removeItem("companyName");

  if (
    companySettingsFromSessionStorage !==
    defaultCompanySettingsFromSessionStorage
  ) {
    saveToSessionStorage(
      "companySettings",
      defaultCompanySettingsFromSessionStorage
    );
  }
  if (additionalSettings !== defaultAdditionalSettings) {
    saveToSessionStorage("additionalSettings", defaultAdditionalSettings);
  }
  if (selectColorId !== defaultColorId) {
    saveToSessionStorage("selectColorId", defaultColorId);
  }
  if (selectColorAccent !== defaultColorAccent) {
    saveToSessionStorage("selectColorAccent", defaultColorAccent);
  }
};
