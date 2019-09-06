import { combineReducers } from 'redux';
import authReducer from './auth/reducers';
import peopleReducer from './people/reducers';
import profileReducer from './profile/reducers';
import groupReducer from './group/reducers';

const rootReducer = combineReducers({
    auth: authReducer,
    people: peopleReducer,
    profile: profileReducer,
    group: groupReducer
});

export default rootReducer;