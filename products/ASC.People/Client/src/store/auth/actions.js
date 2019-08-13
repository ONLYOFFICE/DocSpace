import * as api from '../services/api';
import { setGroups } from '../people/actions';
import setAuthorizationToken from '../../store/services/setAuthorizationToken';
import { fetchPeople } from '../people/actions';

export const LOGIN_POST = 'LOGIN_POST';
export const SET_CURRENT_USER = 'SET_CURRENT_USER';
export const SET_MODULES = 'SET_MODULES';
export const SET_SETTINGS = 'SET_SETTINGS';
export const SET_IS_LOADED = 'SET_IS_LOADED';
export const LOGOUT = 'LOGOUT';

export function setCurrentUser(user) {
    return {
        type: SET_CURRENT_USER,
        user
    };
};

export function setModules(modules) {
    return {
        type: SET_MODULES,
        modules
    };
};

export function setSettings(settings) {
    return {
        type: SET_SETTINGS,
        settings
    };
};

export function setIsLoaded(isLoaded) {
    return {
        type: SET_IS_LOADED,
        isLoaded
    };
};

export function setLogout() {
    return {
        type: LOGOUT
    };
};

export function getUserInfo(dispatch) {
    return api.getUser()
        .then(res => dispatch(setCurrentUser(res.data.response)))
        .then(() => api.getModulesList())
        .then(res => dispatch(setModules(res.data.response)))
        .then(() => api.getSettings())
        .then(res => dispatch(setSettings(res.data.response)))
        .then(() => api.getGroupList())
        .then(res => dispatch(setGroups(res.data.response)))
        .then(() => dispatch(fetchPeople()))
        .then(() => dispatch(setIsLoaded(true)));
};

export function login(data) {
    return dispatch => {
        return api.login(data)
            .then(res => {
                const token = res.data.response.token;
                setAuthorizationToken(token);
            })
            .then(() => getUserInfo(dispatch));
    }
};


export function logout() {
    return dispatch => {
        setAuthorizationToken();
        return dispatch(setLogout());
    };
};
