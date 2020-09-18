import { api } from "asc-web-common";

export const SET_SETTINGS_PAYMENTS_ENTERPRISE =
  "SET_SETTINGS_PAYMENTS_ENTERPRISE";
export const SET_UPLOAD_PAYMENTS_ENTERPRISE_LICENSE =
  "SET_UPLOAD_PAYMENTS_ENTERPRISE_LICENSE";
export const RESET_UPLOADED_LICENSE = "RESET_UPLOADED_LICENSE";

export function setSettings(settings) {
  return {
    type: SET_SETTINGS_PAYMENTS_ENTERPRISE,
    settings,
  };
}

export function setLicenseUpload(message) {
  return {
    type: SET_UPLOAD_PAYMENTS_ENTERPRISE_LICENSE,
    message,
  };
}

export function resetUploadedLicense() {
  return {
    type: RESET_UPLOADED_LICENSE,
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
    return api.settings.setLicense(confirmKey, data).then((res) => {
      dispatch(setLicenseUpload(res));
      setTimeout(() => {
        dispatch(getSettingsPayment());
      }, 50);
    });
  };
}
export function acceptPaymentsLicense() {
  return () => {
    return api.settings.AcceptLicense().then((res) => console.log(res));
  };
}
