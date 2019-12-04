import { combineReducers } from 'redux';
import settingsReducer from './settings/reducer';
import { store } from 'asc-web-common';
const { reducer: authReducer } = store.auth;

const rootReducer = combineReducers({
    auth: authReducer,
    settings: settingsReducer
});

export default rootReducer;