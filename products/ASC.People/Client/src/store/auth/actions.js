import * as api from "../services/api";
import { fetchGroups, fetchPeople } from "../people/actions";
import setAuthorizationToken from "../../store/services/setAuthorizationToken";
import { getFilterByLocation } from "../../helpers/converters";
import config from "../../../package.json";
import { checkResponseError } from "../../helpers/utils";

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

  await fetchGroups(dispatch);

  var re = new RegExp(`${config.homepage}((/?)$|/filter)`, "gm");
  const match = window.location.pathname.match(re);

  if (match && match.length > 0)
  {
    const newFilter = getFilterByLocation(window.location);
    await fetchPeople(newFilter, dispatch);
  }

  return dispatch(setIsLoaded(true));
}

export function login(data) {
  return dispatch => {
    return api
      .login(data)
      .then(res => {
        checkResponseError(res);
        const token = res.data.response.token;
        setAuthorizationToken(token);
      })
      .then(() => getUserInfo(dispatch))
      .catch((e) => console.error(e));
  };
}

export function logout() {
  return dispatch => {
    setAuthorizationToken();
    return dispatch(setLogout());
  };
}
