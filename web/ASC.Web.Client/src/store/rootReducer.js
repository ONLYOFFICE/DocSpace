import { combineReducers } from 'redux';
import authReducer from './auth/reducer';
import peopleReducer from './people/reducer';

const rootReducer = combineReducers({
    auth: authReducer,
    people: peopleReducer
});

export default rootReducer;