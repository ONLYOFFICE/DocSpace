import { combineReducers } from 'redux';
import authReducer from './auth/reducer';
import settingsReducer from './settings/reducer';

const rootReducer = combineReducers({
    auth: authReducer,
    settings: settingsReducer
});

export default rootReducer;