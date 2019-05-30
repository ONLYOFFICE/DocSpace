import { SET_CURRENT_USER, SET_MODULES } from '../actions/actionTypes';
import isEmpty from 'lodash/isEmpty';

const initialState = {
    isAuthenticated: false,
    user: {},
    modules: []
}

const auth = (state = initialState, action) => {
    switch (action.type) {
        case SET_CURRENT_USER:
            return Object.assign({}, state, {
                isAuthenticated: !isEmpty(action.user),
                user: action.user
            });
        case SET_MODULES:
            return Object.assign({}, state, {
                modules: action.modules
            });
        default:
            return state;
    }
}

export default auth;