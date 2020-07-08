import { store, api } from "asc-web-common";
const { 
  getPortalPasswordSettings, 
  setTimezones, 
  setPortalCultures 
} = store.auth.actions;

export const SET_IS_WIZARD_LOADED = 'SET_IS_WIZARD_LOADED';
export const SET_IS_MACHINE_NAME = 'SET_IS_MACHINE_NAME';

export function setIsWizardLoaded(isWizardLoaded) {
  return {
    type: SET_IS_WIZARD_LOADED,
    isWizardLoaded
  };
};

export function setMachineName(machineName) {
  return {
    type: SET_IS_MACHINE_NAME,
    machineName
  };
};

export function getWizardInfo(token) {
  return dispatch => {
    return getPortalPasswordSettings(dispatch, token);
  };
};

export function getPortalTimezones(token) {
  return dispatch => {
    return api.settings.getPortalTimezones(token).then(timezones => {
      dispatch(setTimezones(timezones));
    })
  }
}

export function getPortalCultures() {
  return dispatch => {
    return api.settings.getPortalCultures().then(cultures => {
      dispatch(setPortalCultures(cultures));
    })
  }
}

export function getMachineName(token) {
  return dispatch => {
    return api.settings.getMachineName(token).then(machineName => {
      dispatch(setMachineName(machineName));
    })
  }
}