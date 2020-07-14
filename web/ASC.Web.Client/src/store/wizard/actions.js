import { store, api } from "asc-web-common";
const { 
  setPasswordSettings, 
  setTimezones, 
  setPortalCultures,
  setCompleteWizard,
  getPortalSettings 
} = store.auth.actions;

export const SET_IS_WIZARD_LOADED = 'SET_IS_WIZARD_LOADED';
export const SET_IS_MACHINE_NAME = 'SET_IS_MACHINE_NAME';
export const SET_COMPLETE = 'SET_COMPLETE';

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
}

export function setComplete(isComplete) {
  return {
    type: SET_COMPLETE,
    isComplete
  }
}

export function getPortalPasswordSettings(token) {
  return dispatch => {
    return api.settings.getPortalPasswordSettings(token).then(settings => {
      dispatch(setPasswordSettings(settings));
    });
  };
}

export function getPortalTimezones(token) {
  return dispatch => {
    return api.settings.getPortalTimezones(token).then(timezones => {
      dispatch(setTimezones(timezones));
    });
  };
}

export function getPortalCultures() {
  return dispatch => {
    return api.settings.getPortalCultures().then(cultures => {
      dispatch(setPortalCultures(cultures));
    });
  };
}

export function getMachineName(token) {
  return dispatch => {
    return api.settings.getMachineName(token).then(machineName => {
      dispatch(setMachineName(machineName));
    });
  };
}

export function setPortalOwner(email, pwd, lng, confirmKey, analytics) {
  return dispatch => {
    return api.settings.setPortalOwner(email, pwd, lng, confirmKey, analytics)
    .then((res) => { 
      console.log(res)
      dispatch(setCompleteWizard()); 
    })
    .then(() => getPortalSettings(dispatch))
  }
}