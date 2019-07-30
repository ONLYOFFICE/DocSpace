import { combineReducers } from 'redux';
import authReducer from './auth/reducers';
import peopleReducer from './people/reducers';
import profileReducer from './profile/reducers';
import settingsReducer from './settings/reducer';
import { reducer as formReducer } from 'redux-form';

const rootReducer = combineReducers({
    auth: authReducer,
    people: peopleReducer,
    profile: profileReducer,
    form: formReducer,
    settings: settingsReducer
});

export default rootReducer;