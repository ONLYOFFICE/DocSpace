import * as api from '../services/api';
import setAuthorizationToken from '../services/setAuthorizationToken';

export const LOGIN_POST = 'LOGIN_POST';
export const SET_CURRENT_USER = 'SET_CURRENT_USER';
export const SET_MODULES = 'SET_MODULES';
export const SET_SETTINGS = 'SET_SETTINGS';
export const SET_IS_LOADED = 'SET_IS_LOADED';
export const LOGOUT = 'LOGOUT';
export const SET_PASSWORD_SETTINGS = 'SET_PASSWORD_SETTINGS';

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

export function setPasswordSettings(password) {
    return {
        type: SET_PASSWORD_SETTINGS,
        password
    };
};


export function getUserInfo(dispatch) {
    return api.getUser()
        .then((res) => dispatch(setCurrentUser(res.data.response)))
        .then(() => api.getSettings())
        .then(res => dispatch(setSettings(res.data.response)))
        .then(api.getModulesList)
        .then((res) => dispatch(setModules(res.data.response)))
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

export function getPasswordSettings(token) {
    return dispatch => {
        return api.getPasswordSettings(token)
            .then((res) => dispatch(setPasswordSettings(res.data.response)))
            .then(() => dispatch(setIsLoaded(true)));
    }
};

export function createConfirmUser(registerData, loginData, key) {
    const data = Object.assign({}, registerData, loginData);
    return dispatch => {
        return api.createUser(data, key)
            .then(res => {
                checkResponseError(res);
                console.log('register success:', res.data.response);
                return api.login(loginData);

            })
            .then(res => {
                console.log("log in, result:", res);
                checkResponseError(res);
                const token = res.data.response.token;
                setAuthorizationToken(token);
                return getUserInfo(dispatch);
            });
    };
};

export function checkResponseError(res) {
    if (res && res.data && res.data.error) {
        console.error(res.data.error);
        throw new Error(res.data.error.message);
    }
}