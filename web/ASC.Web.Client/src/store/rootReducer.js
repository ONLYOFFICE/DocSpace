import { combineReducers } from 'redux';
import authReducer from './auth/reducer';

const rootReducer = combineReducers({
    auth: authReducer
});

export default rootReducer;