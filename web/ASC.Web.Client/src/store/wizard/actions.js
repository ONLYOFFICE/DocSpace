import { store, api } from "asc-web-common";
const {
  setPasswordSettings,
  setTimezones,
  setPortalCultures,
  getPortalSettings,
  setWizardComplete,
} = store.auth.actions;

export const SET_IS_WIZARD_LOADED = "SET_IS_WIZARD_LOADED";
export const SET_IS_MACHINE_NAME = "SET_IS_MACHINE_NAME";
export const SET_IS_LICENSE_REQUIRED = "SET_IS_LICENSE_REQUIRED";
export const SET_LICENSE_UPLOAD = "SET_LICENSE_UPLOAD";
export const RESET_LICENSE_UPLOADED = "RESET_LICENSE_UPLOADED";

export function setIsWizardLoaded(isWizardLoaded) {
  return {
    type: SET_IS_WIZARD_LOADED,
    isWizardLoaded,
  };
}

export function setMachineName(machineName) {
  return {
    type: SET_IS_MACHINE_NAME,
    machineName,
  };
}

export function setIsRequiredLicense(isRequired) {
  return {
    type: SET_IS_LICENSE_REQUIRED,
    isRequired,
  };
}

export function setLicenseUpload(message) {
  return {
    type: SET_LICENSE_UPLOAD,
    message,
  };
}

export function resetLicenseUploaded() {
  return {
    type: RESET_LICENSE_UPLOADED,
  };
}

export function getPortalPasswordSettings(token) {
  return (dispatch) => {
    return api.settings.getPortalPasswordSettings(token).then((settings) => {
      dispatch(setPasswordSettings(settings));
    });
  };
}

export function getPortalTimezones(token) {
  return (dispatch) => {
    return api.settings.getPortalTimezones(token).then((timezones) => {
      dispatch(setTimezones(timezones));
    });
  };
}

export function getPortalCultures() {
  return (dispatch) => {
    return api.settings.getPortalCultures().then((cultures) => {
      dispatch(setPortalCultures(cultures));
    });
  };
}

export function getMachineName(token) {
  return (dispatch) => {
    return api.settings.getMachineName(token).then((machineName) => {
      dispatch(setMachineName(machineName));
    });
  };
}

export function setPortalOwner(
  email,
  hash,
  lng,
  timeZone,
  confirmKey,
  analytics
) {
  return (dispatch) => {
    return api.settings
      .setPortalOwner(email, hash, lng, timeZone, confirmKey, analytics)
      .then(() => dispatch(setWizardComplete()))
      .then(() => getPortalSettings(dispatch));
  };
}

export function getIsRequiredLicense() {
  return (dispatch) => {
    return api.settings
      .getIsLicenseRequired()
      .then((isRequired) => dispatch(setIsRequiredLicense(isRequired)));
  };
}

export function setLicense(confirmKey, data) {
  return (dispatch) => {
    return api.settings
      .setLicense(confirmKey, data)
      .then((res) => dispatch(setLicenseUpload(res)));
  };
}
