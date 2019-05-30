import * as api from '../utils/api';
import { SET_CURRENT_USER, SET_MODULES } from './actionTypes';
import setAuthorizationToken from '../utils/setAuthorizationToken';

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

export function getUserInfo(dispatch) {
    return api.getUser()
        .then((res) => dispatch(setCurrentUser(res.data.response)))
        .then(api.getModulesList)
        .then((res) => dispatch(setModules(res.data.response)));
};

export function login(data) {
    return dispatch => {
        return api.login(data)
            .then(res => {
                const token = res.data.response.token;
                setAuthorizationToken(token);
            })
            .then(() => getUserInfo(dispatch));
            /*.then(api.getUser)
            .then((res) => dispatch(setCurrentUser(res.data.response)))
            .then(api.getModulesList)
            .then((res) => dispatch(setModules(res.data.response)));*/
    }
};