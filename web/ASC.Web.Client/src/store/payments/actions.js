import { api } from "asc-web-common";

export const SET_SETTINGS_PAYMENTS_ENTERPRISE =
  "SET_SETTINGS_PAYMENTS_ENTERPRISE";

export function setSettings(settings) {
  return {
    type: SET_SETTINGS_PAYMENTS_ENTERPRISE,
    settings,
  };
}

export function getSettingsPayment() {
  return (dispatch) => {
    return api.settings.getPaymentSettings().then((settings) => {
      dispatch(setSettings(settings));
    });
  };
}
export function setLicense(confirmKey, data) {
  return (dispatch) => {
    return api.settings
      .setLicense(confirmKey, data)
      .then((res) => console.log(res));
  };
}
