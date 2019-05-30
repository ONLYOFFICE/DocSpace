import * as api from '../utils/api';

export function getModules() {
    return dispatch => {
        return api.getModulesList();
    }
};