import { api } from "asc-web-common";

export const SET_PASSWORD_SETTINGS = "SET_PASSWORD_SETTINGS";
export const SET_IS_CONFIRM_LOADED = "SET_IS_CONFIRM_LOADED";

export function setPasswordSettings(passwordSettings) {
  return {
    type: SET_PASSWORD_SETTINGS,
    passwordSettings
  };
};

export function setIsConfirmLoaded(isConfirmLoaded) {
  return {
    type: SET_IS_CONFIRM_LOADED,
    isConfirmLoaded
  };
};

export function getConfirmationInfo(token) {
  return dispatch => {
    return api.settings
      .getPortalPasswordSettings(token)
      .then(settings => dispatch(setPasswordSettings(settings)))
      .then(() => dispatch(setIsConfirmLoaded(true)));
  };
};
