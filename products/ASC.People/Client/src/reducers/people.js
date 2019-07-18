import { SET_GROUPS, SET_USERS } from '../actions/actionTypes';

const initialState = {
  users: [],
  groups: []    
};

const people = (state = initialState, action) => {
    switch (action.type) {
        case SET_GROUPS:
            return Object.assign({}, state, {
                groups: action.groups
            });
        case SET_USERS:
            return Object.assign({}, state, {
                users: action.users
            });
        default:
            return state;
    }
}

export default people;