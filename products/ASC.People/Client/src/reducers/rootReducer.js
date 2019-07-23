import { combineReducers } from 'redux';
import auth from './auth';
import people from './people';
import { reducer as formReducer } from 'redux-form'

const rootReducer = combineReducers({
    auth,
    people,
    form: formReducer
});

export default rootReducer;