import * as api from "../services/api";

export const SET_USERS = "SET_USERS";
export const SET_ADMINS = "SET_ADMINS";
export const SET_OWNER = "SET_OWNER";
export const SET_GREETING_SETTINGS = "SET_GREETING_SETTINGS";

export function setUsers(users) {
  return {
    type: SET_USERS,
    users
  };
}

export function setAdmins(admins) {
  return {
    type: SET_ADMINS,
    admins
  };
}

export function setOwner(owner) {
  return {
    type: SET_OWNER,
    owner
  };
}

export function setGreetingSettings(title) {
  return {
    type: SET_GREETING_SETTINGS,
    title
  };
}

export function getListUsers() {
  return dispatch => {
    return api.getUserList().then(users => dispatch(setUsers(users)));
  };
}

export function getListAdmins(productId) {
  return dispatch => {
    return api
      .getProductAdminsList(productId)
      .then(admins => dispatch(setAdmins(admins)));
  };
}

export function changeAdmins(userId, productId, isAdmin) {
  return dispatch => {
    return api.changeProductAdmin(userId, productId, isAdmin).then(() => {
      dispatch(getListUsers());
      dispatch(getListAdmins(productId));
    });
  };
}

export function getUserById(userId) {
  return dispatch => {
    return api.getUserById(userId).then(owner => dispatch(setOwner(owner)));
  };
}

export function getGreetingTitle() {
  return dispatch => {
    return api.getGreetingSettings()
      .then(greetingTitle => dispatch(setGreetingSettings(greetingTitle)));
  };
}

export function setGreetingTitle(greetingTitle) {
  return dispatch => {
    return api.setGreetingSettings(greetingTitle)
      .then((res) => {
        dispatch(setGreetingSettings(greetingTitle))
      });
  };
}

export function restoreGreetingTitle() {
  return dispatch => {
    return api.restoreGreetingSettings()
      .then((res) => {
        dispatch(setGreetingSettings(res.companyName))
      });
  };
}
