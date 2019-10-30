import * as api from "../services/api";

export const SET_USERS = "SET_USERS";

export function setUsers(users) {
  return {
    type: SET_USERS,
    users
  };
}

export function getAdminUsers() {
  return dispatch => {
    return api.getUserList()
      .then(users => dispatch(setUsers(users)));
  };
}
