import { combineReducers } from 'redux';
import auth from './modulesReducer';
import modules from './authReducer';

const rootReducer = combineReducers({
    auth,
    modules
});

export default rootReducer;