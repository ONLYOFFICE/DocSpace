import { default as api } from "../../api";

export const LOGIN_POST = "LOGIN_POST";
export const SET_CURRENT_USER = "SET_CURRENT_USER";
export const SET_MODULES = "SET_MODULES";
export const SET_SETTINGS = "SET_SETTINGS";
export const SET_IS_LOADED = "SET_IS_LOADED";
export const LOGOUT = "LOGOUT";
export const SET_PASSWORD_SETTINGS = "SET_PASSWORD_SETTINGS";
export const SET_NEW_EMAIL = "SET_NEW_EMAIL";
export const GET_PORTAL_CULTURES = "GET_PORTAL_CULTURES";
export const SET_PORTAL_LANGUAGE_AND_TIME = "SET_PORTAL_LANGUAGE_AND_TIME";
export const GET_TIMEZONES = "GET_TIMEZONES";
export const SET_CURRENT_PRODUCT_ID = "SET_CURRENT_PRODUCT_ID";
export const SET_CURRENT_PRODUCT_HOME_PAGE = "SET_CURRENT_PRODUCT_HOME_PAGE";
export const SET_GREETING_SETTINGS = "SET_GREETING_SETTINGS";

export function setCurrentUser(user) {
  return {
    type: SET_CURRENT_USER,
    user
  };
}

export function setModules(modules) {
  return {
    type: SET_MODULES,
    modules
  };
}

export function setSettings(settings) {
  return {
    type: SET_SETTINGS,
    settings
  };
}

export function setIsLoaded(isLoaded) {
  return {
    type: SET_IS_LOADED,
    isLoaded
  };
}


export function setLogout() {
  return {
    type: LOGOUT
  };
}

export function setPasswordSettings(passwordSettings) {
  return {
    type: SET_PASSWORD_SETTINGS,
    passwordSettings
  };
}

export function setNewEmail(email) {
  return {
    type: SET_NEW_EMAIL,
    email
  };
}

export function setPortalCultures(cultures) {
  return {
    type: GET_PORTAL_CULTURES,
    cultures
  };
}

export function setPortalLanguageAndTime(newSettings) {
  return {
    type: SET_PORTAL_LANGUAGE_AND_TIME,
    newSettings
  };
}

export function setTimezones(timezones) {
  return {
    type: GET_TIMEZONES,
    timezones
  };
}

export function setCurrentProductId(currentProductId) {
  return {
    type: SET_CURRENT_PRODUCT_ID,
    currentProductId
  };
}

export function setCurrentProductHomePage(homepage) {
  return {
    type: SET_CURRENT_PRODUCT_HOME_PAGE,
    homepage
  };
}

export function setGreetingSettings(title) {
  return {
    type: SET_GREETING_SETTINGS,
    title
  };
}

export function getUser(dispatch) {
  return api.people.getUser().then(user => dispatch(setCurrentUser(user)));
}

export function getPortalSettings(dispatch) {
  return api.settings
    .getSettings()
    .then(settings => dispatch(setSettings(settings)));
}

export function getModules(dispatch) {
  return api.modules
    .getModulesList()
    .then(modules => dispatch(setModules(modules)));
}

export const loadInitInfo = dispatch => {
  return getPortalSettings(dispatch).then(() => getModules(dispatch));
};

export function getUserInfo(dispatch) {
  return getUser(dispatch).then(() => loadInitInfo(dispatch));
}

export function login(user, pass) {
  return dispatch => {
    return api.user.login(user, pass)
    .then(() => dispatch(setIsLoaded(false)))
    .then(() => getUserInfo(dispatch));
  };
}

export function logout() {
  return dispatch => {
    return api.user.logout()
      .then(() => dispatch(setLogout()))
      .then(() => dispatch(setIsLoaded(true)));
  };
}

export function getPortalCultures(dispatch = null) {
  return dispatch
    ? api.settings.getPortalCultures().then(cultures => {
        dispatch(setPortalCultures(cultures));
      })
    : dispatch => {
        return api.settings.getPortalCultures().then(cultures => {
          dispatch(setPortalCultures(cultures));
        });
      };
}

export function getPortalPasswordSettings(dispatch, confirmKey = null) {
  return api.settings.getPortalPasswordSettings(confirmKey).then(settings => {
    dispatch(setPasswordSettings(settings));
  });
}
