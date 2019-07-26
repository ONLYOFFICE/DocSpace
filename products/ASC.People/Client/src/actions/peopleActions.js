import { SET_GROUPS, SET_USERS, SET_TARGET_USER } from './actionTypes';
import * as api from '../utils/api';

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