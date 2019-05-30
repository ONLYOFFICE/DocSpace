import * as api from '../utils/api';

export function login(data) {
    return dispatch => {
        return api.login(data);
    }
};