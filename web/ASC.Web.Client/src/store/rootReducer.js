import { combineReducers } from 'redux';
import authReducer from './auth/reducer';
import peopleReducer from './settings/reducer';

const rootReducer = combineReducers({
    auth: authReducer,
    settings: peopleReducer
});

export default rootReducer;