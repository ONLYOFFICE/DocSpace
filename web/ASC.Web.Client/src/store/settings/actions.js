import * as api from "../services/api";
import axios from "axios";
import {
  getUsers,
  //getAdmins,
  getSelectorOptions,
  getUserOptions
} from "./selectors";

export const SET_USERS = "SET_USERS";
export const SET_ADMINS = "SET_ADMINS";
export const SET_OWNER = "SET_OWNER";

export function setUsers(options) {
  return {
    type: SET_USERS,
    options
  };
}

export function setAdmins(admins) {
  console.log(admins)
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


export function getListUsers() {
  return (dispatch, getState) => {
    return api.getUserList().then(users => {
      const { auth } = getState();
      const { /*settings,*/ modules } = auth;
      //const { ownerId } = settings;
      //const convertedUsers = getUsers(users, ownerId);
      //const withoutAdmins = getAdmins(convertedUsers);
      //const options = getSelectorOptions(withoutAdmins);

      api.getProductAdminsList(modules[0].id).then(admins => {
        const options = getUserOptions(users, admins);
        const newOptions = getSelectorOptions(options);
        dispatch(setUsers(newOptions));
      });
    });
  };
}

export function getListAdmins(productId) {
  return (dispatch, getState) => {
    return api.getProductAdminsList(productId).then(admins => {
      const { auth } = getState();
      const { settings } = auth;
      const { ownerId } = settings;
      const convertedAdmins = getUsers(admins, ownerId);

      dispatch(setAdmins(convertedAdmins));
    });
  };
}

export function changeAdmins(userIds, productId, isAdmin) {
  return (dispatch, getState) => {
    return axios
      .all(
        userIds.map(userId =>
          api.changeProductAdmin(userId, productId, isAdmin)
        )
      )
      .then(() =>
        axios.all([api.getUserList(), api.getProductAdminsList(productId)])
      )
      .then(
        axios.spread((users, admins) => {
          const { auth } = getState();
          const { settings } = auth;
          const { ownerId } = settings;

          //const convertedUsers = getUsers(users, ownerId);
          //const withoutAdmins = getAdmins(convertedUsers);
          //const options = getSelectorOptions(withoutAdmins);

          const options = getUserOptions(users, admins);
          const newOptions = getSelectorOptions(options);
          dispatch(setUsers(newOptions));
          const convertedAdmins = getUsers(admins, ownerId);
          dispatch(setAdmins(convertedAdmins));
        })
      );
  };
}

export function getUserById(userId) {
  return dispatch => {
    return api.getUserById(userId).then(owner => dispatch(setOwner(owner)));
  };
}
