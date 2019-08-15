import { SET_CURRENT_USER, SET_MODULES, SET_IS_LOADED, LOGOUT } from './actions';
import isEmpty from 'lodash/isEmpty';

const initialState = {
    isAuthenticated: false,
    isLoaded: false,
    user: {},
    modules: []    
}

const authReducer = (state = initialState, action) => {
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
        case SET_IS_LOADED:
            return Object.assign({}, state, {
                isLoaded: action.isLoaded
            });
        case LOGOUT:
            return initialState;
        default:
            return state;
    }
}

export default authReducer;