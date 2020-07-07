import { store, api } from "asc-web-common";
const { getPortalPasswordSettings, setPortalLanguageAndTime, setTimezones, setPortalCultures } = store.auth.actions;

export const SET_IS_WIZARD_LOADED = 'SET_IS_WIZARD_LOADED';

export function setIsWizardLoaded(isWizardLoaded) {
  return {
    type: SET_IS_WIZARD_LOADED,
    isWizardLoaded
  };
};

export function getWizardInfo(token) {
  return dispatch => {
    return getPortalPasswordSettings(dispatch, token);
  };
};

export function getPortalTimezones() {
  return dispatch => {
    return api.settings.getPortalTimezones().then(timezones => {
      console.log('timezones -----------------------------')
      dispatch(setTimezones(timezones));
    });
  };
};

export function getPortalCultures() {
  return dispatch => {
    return api.settings.getPortalCultures().then(cultures => {
      dispatch(setPortalCultures(cultures));
    })
  }
}