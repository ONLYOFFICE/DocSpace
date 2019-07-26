import { SET_GROUPS, SET_USERS, SET_TARGET_USER } from '../actions/actionTypes';

const initialState = {
  users: [],
  groups: [],
  targetUser: null
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
        case SET_TARGET_USER:
            return Object.assign({}, state, {
                targetUser: action.targetUser
            });
        default:
            return state;
    }
}

export default people;