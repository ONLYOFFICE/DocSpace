import {
  SET_GROUPS,
  SET_USERS,
  SET_TARGET_USER,
  SET_SELECTION,
  SET_SELECTED,
  SELECT_USER,
  DESELECT_USER
} from "./actionTypes";
import * as api from "../utils/api";

export function setUsers(users) {
  return {
    type: SET_USERS,
    users
  };
};

export function setGroups(groups) {
  return {
    type: SET_GROUPS,
    groups
  };
};

export function setUser(targetUser) {
    return {
        type: SET_TARGET_USER,
        targetUser
    };
};

export function fetchAndSetUser(userId) {
    return async(dispatch, getState) => {
        try {
            const res = await api.getUser(userId);
            dispatch(setUser(res.data.response))
        } catch (error) {
            console.error(error);
        }
    };
}

export function setSelection(selection) {
  return {
    type: SET_SELECTION,
    selection
  };
};

export function setSelected(selected) {
  return {
    type: SET_SELECTED,
    selected
  };
};

export function selectUser(user) {
  return {
    type: SELECT_USER,
    user
  };
};

export function deselectUser(user) {
  return {
    type: DESELECT_USER,
    user
  };
};

export function getPeople(filter) {
  return dispatch => {
    return api.getUserList(filter).then(res => {
      console.log("api.getUserList", res);
      return dispatch(setUsers(res.data.response));
    });
  };
};
