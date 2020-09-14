import { api } from "asc-web-common";

export const SET_SALES_EMAIL = "SET_SALES_EMAIL";
export const SET_HELP_URL = "SET_HELP_URL";
export const SET_BUY_URL = "SET_BUY_URL";
export const SET_CURRENT_LICENSE = "SET_CURRENT_LICENSE";
export const SET_SETTINGS = "SET_SETTINGS";
export const SET_STANDALONE = "SET_STANDALONE";

export function setSettings(settings) {
  return {
    type: SET_SETTINGS,
    settings,
  };
}

export function setLicense(confirmKey, data) {
  return (dispatch) => {
    return api.settings
      .setLicense(confirmKey, data)
      .then((res) => console.log(res));
  };
}

export function getSettings() {
  return (dispatch) => {
    return api.settings.getPaymentSettings().then((settings) => {
      dispatch(setSettings(settings));
    });
  };
}
