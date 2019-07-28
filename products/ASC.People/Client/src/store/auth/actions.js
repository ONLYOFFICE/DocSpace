import * as api from '../../utils/api';
import { setGroups, setUsers } from '../people/actions';
import setAuthorizationToken from '../../utils/setAuthorizationToken';

export const LOGIN_POST = 'LOGIN_POST';
export const SET_CURRENT_USER = 'SET_CURRENT_USER';
export const SET_MODULES = 'SET_MODULES';
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
        .then((res) => dispatch(setCurrentUser(res.data.response)))
        .then(() => api.getModulesList())
        .then((res) => dispatch(setModules(res.data.response)))
        .then(() => api.getGroupList())
        .then((res) => dispatch(setGroups(res.data.response)))
        .then(() => api.getUserList())
        .then((res) => { 
            console.log("api.getUserList", res);
            return dispatch(setUsers(res.data.response)); })
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
