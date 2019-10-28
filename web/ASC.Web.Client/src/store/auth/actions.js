import * as api from '../services/api';
import { setAuthorizationToken } from '../services/client';

export const LOGIN_POST = 'LOGIN_POST';
export const SET_CURRENT_USER = 'SET_CURRENT_USER';
export const SET_MODULES = 'SET_MODULES';
export const SET_SETTINGS = 'SET_SETTINGS';
export const SET_IS_LOADED = 'SET_IS_LOADED';
export const LOGOUT = 'LOGOUT';
export const SET_PASSWORD_SETTINGS = 'SET_PASSWORD_SETTINGS';
export const SET_IS_CONFIRM_LOADED = 'SET_IS_CONFIRM_LOADED';
export const SET_NEW_EMAIL = 'SET_NEW_EMAIL';
export const SET_NEW_SETTING_NODE = 'SET_NEW_SETTING_NODE';
export const GET_PORTAL_CULTURES = 'GET_PORTAL_CULTURES';
export const SET_PORTAL_LANGUAGE_AND_TIME = 'SET_PORTAL_LANGUAGE_AND_TIME';

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

export function setIsConfirmLoaded(isConfirmLoaded) {
    return {
        type: SET_IS_CONFIRM_LOADED,
        isConfirmLoaded
    };
};

export function setLogout() {
    return {
        type: LOGOUT
    };
};

export function setPasswordSettings(passwordSettings) {
    return {
        type: SET_PASSWORD_SETTINGS,
        passwordSettings
    };
};

export function setNewEmail(email) {
    return {
        type: SET_NEW_EMAIL,
        email
    };
};

export function setNewSelectedNode(selectedNodeLink) {
    return {
        type: SET_NEW_SETTING_NODE,
        selectedNodeLink
    };
};

export function getPortalCultures(cultures) {
    return {
        type: GET_PORTAL_CULTURES,
        cultures
    };
};

export function setPortalLanguageAndTime(newSettings) {
    return {
        type: SET_PORTAL_LANGUAGE_AND_TIME,
        newSettings
    };
};

export function getUser(dispatch) {
    return api.getUser()
        .then(user => dispatch(setCurrentUser(user)));
}

export function getPortalSettings(dispatch) {
    return api.getSettings()
        .then(settings => dispatch(setSettings(settings)));
}

export function getModules(dispatch) {
    return api.getModulesList()
        .then(modules => dispatch(setModules(modules)));
}

const loadInitInfo = (dispatch) => {
    return getPortalSettings(dispatch)
        .then(getModules.bind(this, dispatch))
        .then(() => dispatch(setIsLoaded(true)));
}

export function getUserInfo(dispatch) {
    return getUser(dispatch)
        .then(loadInitInfo.bind(this, dispatch));
};

export function login(user, pass) {
    return dispatch => {
        return api.login(user, pass)
            .then(() => getUserInfo(dispatch));
    }
};


export function logout(dispatch = null) {
    return dispatch ? () => {
        setAuthorizationToken();
        return Promise.resolve(dispatch(setLogout()));
    } : dispatch => {
        setAuthorizationToken();
        return Promise.resolve(dispatch(setLogout()));
    };
};

export function getConfirmationInfo(token, type) {
    return dispatch => {
        return api.getPasswordSettings(token)
            .then((settings) => dispatch(setPasswordSettings(settings)))
            .then(() => dispatch(setIsConfirmLoaded(true)));
    }
};

export function createConfirmUser(registerData, loginData, key) {
    const data = Object.assign({}, registerData, loginData);
    return (dispatch) => {
        return api.createUser(data, key)
            .then(user => dispatch(setCurrentUser(user)))
            .then(() => api.login(loginData.userName, loginData.password))
            .then(loadInitInfo.bind(this, dispatch));
    };
};

export function changePassword(userId, password, key) {
    return dispatch => {
        return api.changePassword(userId, password, key)
            .then(() => logout(dispatch));
    }
}

export function changeEmail(userId, email, key) {
    return dispatch => {
        return api.changeEmail(userId, email, key)
            .then(user => dispatch(setNewEmail(user.email)));
    }
}

export function activateConfirmUser(personalData, loginData, key, userId, activationStatus) {
    const changedData = {
        id: userId,
        FirstName: personalData.firstname,
        LastName: personalData.lastname
    }

    return dispatch => {
        return api.changePassword(userId, loginData.password, key)
            .then(data => {
                console.log('set password success:', data);
                return api.updateActivationStatus(activationStatus, userId, key);
            })
            .then(data => {
                console.log("activation success, result:", data);
                return dispatch(login(loginData.userName, loginData.password));
            })
            .then(data => {
                console.log("log in, result:", data);
                return api.updateUser(changedData);
            })
            .then(user => dispatch(setCurrentUser(user)));
    };
};

export function getCultures() {
    return dispatch => {
        return api.getPortalCultures()
            .then(cultures => {
                dispatch(getPortalCultures(cultures));
                return cultures;
            }
            );
    };
}

export function setLanguageAndTime(lng, timeZoneID) {
    return dispatch => {
        return api.setLanguageAndTime(lng, timeZoneID)
            .then(() => dispatch(setPortalLanguageAndTime({ lng, timeZoneID })));
    };
}
