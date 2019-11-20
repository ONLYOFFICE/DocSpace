import * as api from "../services/api";
import Filter from "./filter";
import history from "../../history";
import config from "../../../package.json";
import {
  EMPLOYEE_STATUS,
  ACTIVATION_STATUS,
  ROLE,
  GROUP,
  SEARCH,
  SORT_BY,
  SORT_ORDER,
  PAGE,
  PAGE_COUNT,
  EmployeeStatus
} from "../../helpers/constants";
import unionBy from 'lodash/unionBy';

export const SET_GROUPS = "SET_GROUPS";
export const SET_USERS = "SET_USERS";
export const SET_USER = "SET_USER";
export const SET_SELECTION = "SET_SELECTION";
export const SELECT_USER = "SELECT_USER";
export const DESELECT_USER = "DESELECT_USER";
export const SET_SELECTED = "SET_SELECTED";
export const SET_FILTER = "SET_FILTER";
export const SELECT_GROUP = "SELECT_GROUP";
export const SET_SELECTOR_USERS = "SET_SELECTOR_USERS";

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

    return fetchPeople(newFilter, dispatch);
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

export function setFilterUrl(filter) {
  const defaultFilter = Filter.getDefault();
  const params = [];

  if (filter.employeeStatus) {
    params.push(`${EMPLOYEE_STATUS}=${filter.employeeStatus}`);
  }

  if (filter.activationStatus) {
    params.push(`${ACTIVATION_STATUS}=${filter.activationStatus}`);
  }

  if (filter.role) {
    params.push(`${ROLE}=${filter.role}`);
  }

  if (filter.group) {
    params.push(`${GROUP}=${filter.group}`);
  }

  if (filter.search) {
    params.push(`${SEARCH}=${filter.search}`);
  }

  if (filter.pageCount !== defaultFilter.pageCount) {
    params.push(`${PAGE_COUNT}=${filter.pageCount}`);
  }

  params.push(`${PAGE}=${filter.page + 1}`);
  params.push(`${SORT_BY}=${filter.sortBy}`);
  params.push(`${SORT_ORDER}=${filter.sortOrder}`);

  if (params.length > 0) {
    history.push(`${config.homepage}/filter?${params.join("&")}`);
  }
}

export function setFilter(filter) {
  setFilterUrl(filter);
  return {
    type: SET_FILTER,
    filter
  };
}

export function setSelectorUsers(users) {
  return {
    type: SET_SELECTOR_USERS,
    users
  };
}

export function fetchSelectorUsers() {
  return dispatch => {
    api.getSelectorUserList().then(data => {
      const users = data.items;
      return dispatch(setSelectorUsers(users));
    });
  };
}

export function fetchGroups(dispatchFunc = null) {
  return api.getGroupList().then(groups => {
    return dispatchFunc
      ? dispatchFunc(setGroups(groups))
      : Promise.resolve(dispatch => dispatch(setGroups(groups)));
  });
}

export function fetchPeople(filter, dispatchFunc = null) {
  return dispatchFunc
    ? fetchPeopleByFilter(dispatchFunc, filter)
    : (dispatch, getState) => {
        if (filter) {
          return fetchPeopleByFilter(dispatch, filter);
        } else {
          const { people } = getState();
          const { filter } = people;
          return fetchPeopleByFilter(dispatch, filter);
        }
      };
}

function fetchPeopleByFilter(dispatch, filter) {
  let filterData = filter && filter.clone();

  if (!filterData) {
    filterData = Filter.getDefault();
    filterData.employeeStatus = EmployeeStatus.Active;
  }

  return api.getUserList(filterData).then(data => {
    filterData.total = data.total;
    dispatch(setFilter(filterData));
    dispatch({
      type: SELECT_GROUP,
      groupId: filterData.group
    });
    return dispatch(setUsers(data.items));
  });
}

export function updateUserStatus(status, userIds) {
  return (dispatch, getState) => {
    return api.updateUserStatus(status, userIds).then(users => {
      const { people } = getState();
      const { users: currentUsers } = people;

      const newUsers = unionBy(users, currentUsers, "id");

      dispatch(setUsers(newUsers));
    });
  };
}

export function updateUserType(type, userIds) {
  return dispatch => {
    return api.updateUserType(type, userIds).then(users => {
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

    const newFilter = filter.clone(true);

    return fetchPeople(newFilter, dispatch);
  };
}
