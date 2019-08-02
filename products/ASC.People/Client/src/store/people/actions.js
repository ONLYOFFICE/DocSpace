import * as api from "../../utils/api";
import Filter from "../../helpers/filter";

export const SET_GROUPS = 'SET_GROUPS';
export const SET_USERS = 'SET_USERS';
export const SET_SELECTION = 'SET_SELECTION';
export const SELECT_USER = 'SELECT_USER';
export const DESELECT_USER = 'DESELECT_USER';
export const SET_SELECTED = 'SET_SELECTED';
export const SET_FILTER = 'SET_FILTER';

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

export function setFilter(filter) {
    return {
      type: SET_FILTER,
      filter
    };
  };

export function fetchPeople(filter) {
    return dispatch => {

        let filterData = (filter && filter.clone()) || Filter.getDefault();
        return api.getUserList(filterData).then(res => {
            filterData.total = res.data.total;
            dispatch(setFilter(filterData));
            return dispatch(setUsers(res.data.response));
        });
    };
};