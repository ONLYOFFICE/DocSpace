import { SET_USERS, SET_ADMINS, SET_OWNER } from "./actions";

const initialState = {
  users: [],
  admins: [],
  owner: {}
};

const peopleReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_USERS:
      return Object.assign({}, state, {
        users: action.users
      });
    case SET_ADMINS:
      return Object.assign({}, state, {
        admins: action.admins
      });
    case SET_OWNER:
      return Object.assign({}, state, {
        owner: action.owner
      });
    default:
      return state;
  }
};

export default peopleReducer;
