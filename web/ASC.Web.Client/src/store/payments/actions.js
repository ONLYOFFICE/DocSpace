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
export function setPaymentsLicense(confirmKey, data) {
  return (dispatch) => {
    return api.settings
      .setLicense(confirmKey, data)
      .then(() => dispatch(acceptPaymentsLicense()))
      .then(() => dispatch(getSettingsPayment()));
  };
}
export function acceptPaymentsLicense() {
  return () => {
    return api.settings.acceptLicense().then((res) => console.log(res));
  };
}
