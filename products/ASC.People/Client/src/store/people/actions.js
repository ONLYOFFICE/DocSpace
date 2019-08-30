import * as api from "../services/api";
import Filter from "./filter";

export const SET_GROUPS = "SET_GROUPS";
export const SET_USERS = "SET_USERS";
export const SET_USER = "SET_USER";
export const SET_SELECTION = "SET_SELECTION";
export const SELECT_USER = "SELECT_USER";
export const DESELECT_USER = "DESELECT_USER";
export const SET_SELECTED = "SET_SELECTED";
export const SET_FILTER = "SET_FILTER";
export const SELECT_GROUP = "SELECT_GROUP";

export function setUser(user) {
  return {
    type: SET_USER,
    user
  };
}

export function setUsers(users) {
  return {
    type: SET_USERS,
    users
  };
}

export function setGroups(groups) {
  return {
    type: SET_GROUPS,
    groups
  };
}

export function setSelection(selection) {
  return {
    type: SET_SELECTION,
    selection
  };
}

export function setSelected(selected) {
  return {
    type: SET_SELECTED,
    selected
  };
}

export function selectGroup(groupId) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { filter } = people;

    let newFilter = filter.clone();
    newFilter.group = groupId;

    return fetchPeopleByFilter(dispatch, newFilter);
  };
}

export function selectUser(user) {
  return {
    type: SELECT_USER,
    user
  };
}

export function deselectUser(user) {
  return {
    type: DESELECT_USER,
    user
  };
}

export function setFilter(filter) {
  return {
    type: SET_FILTER,
    filter
  };
}

export function fetchPeople(filter) {
  return dispatch => {
    return fetchPeopleByFilter(dispatch, filter);
  };
}

export function fetchPeopleByFilter(dispatch, filter) {
  let filterData = (filter && filter.clone()) || Filter.getDefault();
  return api.getUserList(filterData).then(res => {
    filterData.total = res.data.total;
    dispatch(setFilter(filterData));
    dispatch({
      type: SELECT_GROUP,
      groupId: filterData.group
    });
    return dispatch(setUsers(res.data.response));
  });
}

export async function fetchPeopleAsync(dispatch, filter = null) {
  let filterData = (filter && filter.clone()) || Filter.getDefault();

  const usersResp = await api.getUserList(filterData);

  filterData.total = usersResp.data.total;

  dispatch(setFilter(filterData));
  dispatch({
    type: SELECT_GROUP,
    groupId: filterData.group
  });
  dispatch(setUsers(usersResp.data.response));
}

export function updateUserStatus(status, userIds) {
  return dispatch => {
    return api.updateUserStatus(status, userIds).then(res => {
      if (res && res.data && res.data.error && res.data.error.message)
        throw res.data.error.message;

      const users = res.data.response;

      users.forEach(user => {
        dispatch(setUser(user));
      });
    });
  };
}

export function updateUserType(type, userIds) {
  return dispatch => {
    return api.updateUserType(type, userIds).then(res => {
      if (res && res.data && res.data.error && res.data.error.message)
        throw res.data.error.message;

      const users = res.data.response;

      users.forEach(user => {
        dispatch(setUser(user));
      });
    });
  };
}

export function resetFilter() {
  return (dispatch, getState) => {
    const { people } = getState();
    const { filter } = people;

    return fetchPeople(filter);
  };
}
