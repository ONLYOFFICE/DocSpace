import { combineReducers } from 'redux';
import settingsReducer from './settings/reducer';
import confirmReducer from './confirm/reducer';
import { store } from 'asc-web-common';
import wizardReducer from './wizard/reducer';

const { reducer: authReducer } = store.auth;

const rootReducer = combineReducers({
    auth: authReducer,
    settings: settingsReducer,
    confirm: confirmReducer,
    wizard: wizardReducer
});

export default rootReducer;