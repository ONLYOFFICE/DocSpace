import { api, history, constants } from "asc-web-common";
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
} from "../../helpers/constants";
import { getUserByUserName } from "../people/selectors";

const { EmployeeStatus } = constants;
const { Filter } = api;

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
export const SET_IS_VISIBLE_DATA_LOSS_DIALOG =
  "SET_IS_VISIBLE_DATA_LOSS_DIALOG";
export const SET_IS_EDITING_FORM = "SET_IS_EDITING_FORM";
export const SET_IS_LOADING = "SET_IS_LOADING";
export const TOGGLE_AVATAR_EDITOR = "TOGGLE_AVATAR_EDITOR";
export function setIsLoading(isLoading) {
  return {
    type: SET_IS_LOADING,
    isLoading,
  };
}

export function setUser(user) {
  return {
    type: SET_USER,
    user,
  };
}

export function setUsers(users) {
  return {
    type: SET_USERS,
    users,
  };
}

export function setGroups(groups) {
  return {
    type: SET_GROUPS,
    groups,
  };
}

export function setSelection(selection) {
  return {
    type: SET_SELECTION,
    selection,
  };
}

export function setSelected(selected) {
  return {
    type: SET_SELECTED,
    selected,
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
    user,
  };
}

export function deselectUser(user) {
  return {
    type: DESELECT_USER,
    user,
  };
}

export function toggleAvatarEditor(avatarEditorIsOpen) {
  return {
    type: TOGGLE_AVATAR_EDITOR,
    avatarEditorIsOpen,
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
    params.push(`${SEARCH}=${filter.search.trim()}`);
  }

  if (filter.pageCount !== defaultFilter.pageCount) {
    params.push(`${PAGE_COUNT}=${filter.pageCount}`);
  }

  params.push(`${PAGE}=${filter.page + 1}`);
  params.push(`${SORT_BY}=${filter.sortBy}`);
  params.push(`${SORT_ORDER}=${filter.sortOrder}`);

  //const isProfileView = history.location.pathname.includes('/people/view') || history.location.pathname.includes('/people/edit');
  //if (params.length > 0 && !isProfileView) {
  history.push(`${config.homepage}/filter?${params.join("&")}`);
  //}
}

export function setFilter(filter) {
  setFilterUrl(filter);
  return {
    type: SET_FILTER,
    filter,
  };
}

export function setSelectorUsers(users) {
  return {
    type: SET_SELECTOR_USERS,
    users,
  };
}

export function setIsVisibleDataLossDialog(isVisible, callback) {
  return {
    type: SET_IS_VISIBLE_DATA_LOSS_DIALOG,
    isVisible,
    callback,
  };
}

export function setIsEditingForm(isEdit) {
  return {
    type: SET_IS_EDITING_FORM,
    isEdit,
  };
}

export function fetchSelectorUsers() {
  return (dispatch) => {
    api.people.getSelectorUserList().then((data) => {
      const users = data.items;
      return dispatch(setSelectorUsers(users));
    });
  };
}

export function fetchGroups(dispatchFunc = null) {
  return api.groups.getGroupList().then((groups) => {
    return dispatchFunc
      ? dispatchFunc(setGroups(groups))
      : Promise.resolve((dispatch) => dispatch(setGroups(groups)));
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

export function removeUser(userId, filter) {
  return (dispatch) => {
    return api.people
      .deleteUsers(userId)
      .then(() => fetchPeople(filter, dispatch));
  };
}

export function updateUserList(dispatch, filter) {
  let filterData = filter && filter.clone();
  if (!filterData) {
    filterData = Filter.getDefault();
    filterData.employeeStatus = EmployeeStatus.Active;
  }
  return api.people.getUserList(filterData).then((data) => {
    return dispatch(setUsers(data.items));
  });
}

function fetchPeopleByFilter(dispatch, filter) {
  let filterData = filter && filter.clone();

  if (!filterData) {
    filterData = Filter.getDefault();
    filterData.employeeStatus = EmployeeStatus.Active;
  }

  return api.people.getUserList(filterData).then((data) => {
    filterData.total = data.total;
    dispatch(setFilter(filterData));
    dispatch({
      type: SELECT_GROUP,
      groupId: filterData.group,
    });
    return dispatch(setUsers(data.items));
  });
}

export function updateUserStatus(status, userIds, isRefetchPeople = false) {
  return (dispatch, getState) => {
    return api.people.updateUserStatus(status, userIds).then((users) => {
      const { people } = getState();
      const { filter } = people;
      return isRefetchPeople
        ? fetchPeople(filter, dispatch)
        : Promise.resolve();
    });
  };
}

export function updateUserType(type, userIds) {
  return (dispatch) => {
    return api.people.updateUserType(type, userIds).then((users) => {
      users.forEach((user) => {
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

export function updateProfileInUsers(updatedProfile) {
  return (dispatch, getState) => {
    const { people } = getState();
    const { users } = people;

    if (!users) {
      return updateUserList(dispatch);
    }

    if (!updatedProfile) {
      const { profile } = getState();
      updatedProfile = profile.targetUser;
    }

    const { userName } = updatedProfile;
    const oldProfile = getUserByUserName(users, userName);
    const newProfile = {};

    for (let key in oldProfile) {
      if (
        updatedProfile.hasOwnProperty(key) &&
        updatedProfile[key] !== oldProfile[key]
      ) {
        newProfile[key] = updatedProfile[key];
      } else {
        newProfile[key] = oldProfile[key];
      }
    }

    const updatedUsers = users.map((user) => {
      if (user.id === newProfile.id) {
        return newProfile;
      } else {
        return user;
      }
    });
    return dispatch(setUsers(updatedUsers));
  };
}
