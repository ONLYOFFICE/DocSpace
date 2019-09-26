import * as api from "../services/api";
import { setGroups, fetchPeopleAsync } from "../people/actions";
import setAuthorizationToken from "../../store/services/setAuthorizationToken";
import { getFilterByLocation } from "../../helpers/converters";

export const LOGIN_POST = "LOGIN_POST";
export const SET_CURRENT_USER = "SET_CURRENT_USER";
export const SET_MODULES = "SET_MODULES";
export const SET_SETTINGS = "SET_SETTINGS";
export const SET_IS_LOADED = "SET_IS_LOADED";
export const LOGOUT = "LOGOUT";

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

export async function getUserInfo(dispatch) {
  const { user, modules, settings } = await api.getInitInfo();
  let newSettings = settings;
  if (user.isAdmin) {
    const inviteLinkResp = await api.getInvitationLinks();
    newSettings = Object.assign(newSettings, inviteLinkResp);
  }

  dispatch(setCurrentUser(user));
  dispatch(setModules(modules));
  dispatch(setSettings(newSettings));

  const groupResp = await api.getGroupList();

  dispatch(setGroups(groupResp.data.response));

  const newFilter = getFilterByLocation(window.location);

  await fetchPeopleAsync(dispatch, newFilter);

  return dispatch(setIsLoaded(true));
}

export function login(data) {
  return dispatch => {
    return api
      .login(data)
      .then(res => {
        const token = res.data.response.token;
        setAuthorizationToken(token);
      })
      .then(() => getUserInfo(dispatch));
  };
}

export function logout() {
  return dispatch => {
    setAuthorizationToken();
    return dispatch(setLogout());
  };
}
