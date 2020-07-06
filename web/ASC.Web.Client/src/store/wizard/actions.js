import { store } from "asc-web-common";
const { getPortalPasswordSettings } = store.auth.actions;

export const SET_IS_WIZARD_LOADED = 'SET_IS_WIZARD_LOADED';

export function setIsWizardLoaded(isWizardLoaded) {
  return {
    type: SET_IS_WIZARD_LOADED,
    isWizardLoaded
  };
};

export function getWizardInfo(token) {
  return dispatch => {
    return getPortalPasswordSettings(dispatch, token)
      .then(() => dispatch(setIsWizardLoaded(true)));
  };
};