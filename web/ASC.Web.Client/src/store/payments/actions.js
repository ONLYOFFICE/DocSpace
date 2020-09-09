import { api, store } from "asc-web-common";
// const { setLicenseUpload } = store.wizard.actions;
// const { setIsConfirmLoaded } = store.confirm.actions;
import { setLicenseUpload } from "../wizard/actions";
import { SET_IS_MACHINE_NAME } from "./actions";
const { setTimezones, setPortalCultures } = store.auth.actions;

export const SET_SALES_EMAIL = "SET_SALES_EMAIL";
export const SET_HELP_URL = "SET_HELP_URL";
export const SET_BUY_URL = "SET_BUY_URL";
export const SET_CURRENT_LICENSE = "SET_CURRENT_LICENSE";
export const SET_SETTINGS = "SET_SETTINGS";
export const SET_STANDALONE = "SET_STANDALONE";
export function setSalesEmail(salesEmail) {
  return {
    type: SET_SALES_EMAIL,
    salesEmail,
  };
}
export function setHelpUrl(helpUrl) {
  return {
    type: SET_HELP_URL,
    helpUrl,
  };
}
export function setBuyUrl(buyUrl) {
  return {
    type: SET_BUY_URL,
    buyUrl,
  };
}

export function setSettings(settings) {
  return {
    type: SET_SETTINGS,
    settings,
  };
}
export function setStandalone(standalone) {
  return {
    type: SET_STANDALONE,
    standalone,
  };
}

export function setCurrentLicense(currentLicense) {
  return {
    type: SET_CURRENT_LICENSE,
    currentLicense,
  };
}

export function setLicense(confirmKey, data) {
  return (dispatch) => {
    return api.settings
      .setLicense(confirmKey, data)
      .then((res) => dispatch(setLicenseUpload(res)));
  };
}

export function getSalesEmail() {
  return (dispatch) => {
    return api.settings.getPaymentSettings().then((settings) => {
      dispatch(setSalesEmail(settings.salesEmail));
    });
  };
}
export function getSettings() {
  return (dispatch) => {
    return api.settings.getPaymentSettings().then((settings) => {
      dispatch(setSettings(settings));
    });
  };
}
export function getHelpUrl() {
  return (dispatch) => {
    return api.settings.getPaymentSettings().then((settings) => {
      dispatch(setHelpUrl(settings.feedbackAndSupportUrl));
    });
  };
}

export function getBuyUrl() {
  return (dispatch) => {
    return api.settings.getPaymentSettings().then((settings) => {
      dispatch(setBuyUrl(settings.buyUrl));
    });
  };
}
export function getStandalone() {
  return (dispatch) => {
    return api.settings.getPaymentSettings().then((settings) => {
      dispatch(setStandalone(settings.standalone));
    });
  };
}
export function getCurrentLicense() {
  return (dispatch) => {
    return api.settings.getPaymentSettings().then((settings) => {
      dispatch(setCurrentLicense(settings.currentLicense));
    });
  };
}
