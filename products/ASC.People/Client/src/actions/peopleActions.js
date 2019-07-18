import { SET_GROUPS, SET_USERS } from './actionTypes';

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