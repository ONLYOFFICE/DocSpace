import { combineReducers } from 'redux';
import authReducer from './auth/reducers';
import peopleReducer from './people/reducers';
import profileReducer from './profile/reducers';
import { reducer as formReducer } from 'redux-form';

const rootReducer = combineReducers({
    auth: authReducer,
    people: peopleReducer,
    profile: profileReducer,
    form: formReducer
});

export default rootReducer;