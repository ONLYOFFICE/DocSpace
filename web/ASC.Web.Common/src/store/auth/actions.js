import { default as api } from "../../api";
import { setWithCredentialsStatus } from "../../api/client";
import history from "../../history";

export const LOGIN_POST = "LOGIN_POST";
export const SET_CURRENT_USER = "SET_CURRENT_USER";
export const SET_MODULES = "SET_MODULES";
export const SET_SETTINGS = "SET_SETTINGS";
export const SET_IS_LOADED = "SET_IS_LOADED";
export const SET_IS_LOADED_SECTION = "SET_IS_LOADED_SECTION";
export const LOGOUT = "LOGOUT";
export const SET_PASSWORD_SETTINGS = "SET_PASSWORD_SETTINGS";
export const SET_NEW_EMAIL = "SET_NEW_EMAIL";
export const SET_PORTAL_CULTURES = "SET_PORTAL_CULTURES";
export const SET_PORTAL_LANGUAGE_AND_TIME = "SET_PORTAL_LANGUAGE_AND_TIME";
export const SET_TIMEZONES = "SET_TIMEZONES";
export const SET_CURRENT_PRODUCT_ID = "SET_CURRENT_PRODUCT_ID";
export const SET_CURRENT_PRODUCT_HOME_PAGE = "SET_CURRENT_PRODUCT_HOME_PAGE";
export const SET_GREETING_SETTINGS = "SET_GREETING_SETTINGS";
export const SET_CUSTOM_NAMES = "SET_CUSTOM_NAMES";
export const SET_WIZARD_COMPLETED = "SET_WIZARD_COMPLETED";
export const SET_IS_AUTHENTICATED = "SET_IS_AUTHENTICATED";

export function setCurrentUser(user) {
  return {
    type: SET_CURRENT_USER,
    user,
  };
}

export function setModules(modules) {
  return {
    type: SET_MODULES,
    modules,
  };
}

export function setSettings(settings) {
  return {
    type: SET_SETTINGS,
    settings,
  };
}

export function setIsLoaded(isLoaded) {
  return {
    type: SET_IS_LOADED,
    isLoaded,
  };
}

export function setIsLoadedSection(isLoadedSection) {
  return {
    type: SET_IS_LOADED_SECTION,
    isLoadedSection,
  };
}

export function setLogout() {
  return {
    type: LOGOUT,
  };
}

export function setPasswordSettings(passwordSettings) {
  return {
    type: SET_PASSWORD_SETTINGS,
    passwordSettings,
  };
}

export function setNewEmail(email) {
  return {
    type: SET_NEW_EMAIL,
    email,
  };
}

export function setPortalCultures(cultures) {
  return {
    type: SET_PORTAL_CULTURES,
    cultures,
  };
}

export function setPortalLanguageAndTime(newSettings) {
  return {
    type: SET_PORTAL_LANGUAGE_AND_TIME,
    newSettings,
  };
}

export function setTimezones(timezones) {
  return {
    type: SET_TIMEZONES,
    timezones,
  };
}

export function setCurrentProductId(currentProductId) {
  return {
    type: SET_CURRENT_PRODUCT_ID,
    currentProductId,
  };
}

export function setCurrentProductHomePage(homepage) {
  return {
    type: SET_CURRENT_PRODUCT_HOME_PAGE,
    homepage,
  };
}

export function setGreetingSettings(title) {
  return {
    type: SET_GREETING_SETTINGS,
    title,
  };
}

export function setCustomNames(customNames) {
  return {
    type: SET_CUSTOM_NAMES,
    customNames,
  };
}

export function setWizardComplete() {
  return {
    type: SET_WIZARD_COMPLETED,
  };
}

export function setIsAuthenticated(isAuthenticated) {
  return {
    type: SET_IS_AUTHENTICATED,
    isAuthenticated,
  };
}

export function getUser(dispatch) {
  return api.people
    .getUser()
    .then((user) => dispatch(setCurrentUser(user)))
    .then(() => dispatch(setIsAuthenticated(true)))
    .catch((err) => dispatch(setCurrentUser({})));
}

export function getIsAuthenticated(dispatch) {
  return api.user
    .checkIsAuthenticated()
    .then((success) => { 
      dispatch(setIsAuthenticated(success));
      return success;
    });
}

export function getPortalSettings(dispatch) {
  return api.settings.getSettings().then((settings) => {
    const { passwordHash: hashSettings, ...otherSettings } = settings;
    const logoSettings = { logoUrl: "images/nav.logo.opened.react.svg" };
    dispatch(
      setSettings(
        hashSettings
          ? { ...logoSettings, ...otherSettings, hashSettings }
          : { ...logoSettings, ...otherSettings }
      )
    );

    otherSettings.nameSchemaId &&
      getCurrentCustomSchema(dispatch, otherSettings.nameSchemaId);
  });
}
export function getCurrentCustomSchema(dispatch, id) {
  return api.settings
    .getCurrentCustomSchema(id)
    .then((customNames) => dispatch(setCustomNames(customNames)));
}

export function getModules(dispatch) {
  return api.modules
    .getModulesList()
    .then((modules) => dispatch(setModules(modules)));
}

export const loadInitInfo = (dispatch) => {
  return getPortalSettings(dispatch).then(() => getModules(dispatch));
};

export function getUserInfo(dispatch) {
  return getUser(dispatch).finally(() => loadInitInfo(dispatch));
}

export function login(user, hash) {
  return (dispatch) => {
    return api.user
      .login(user, hash)
      .then(() => dispatch(setIsLoaded(false)))
      .then(() => {
        setWithCredentialsStatus(true);
        return dispatch(setIsAuthenticated(true));
      })
      .then(() => getUserInfo(dispatch));
  };
}

export function logout() {
  return (dispatch) => {
    return api.user.logout().then(() => {
      setWithCredentialsStatus(false);
      dispatch(setLogout());

      history.push("/login");
    });
  };
}

export function getPortalCultures(dispatch = null) {
  return dispatch
    ? api.settings.getPortalCultures().then((cultures) => {
        dispatch(setPortalCultures(cultures));
      })
    : (dispatch) => {
        return api.settings.getPortalCultures().then((cultures) => {
          dispatch(setPortalCultures(cultures));
        });
      };
}

export function getPortalPasswordSettings(dispatch, confirmKey = null) {
  return api.settings.getPortalPasswordSettings(confirmKey).then((settings) => {
    dispatch(setPasswordSettings(settings));
  });
}

export const reloadPortalSettings = () => {
  return (dispatch) => getPortalSettings(dispatch);
};