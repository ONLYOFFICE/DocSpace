import { combineReducers } from 'redux';
import auth from './auth';
import people from './people';

const rootReducer = combineReducers({
    auth,
    people
});

export default rootReducer;