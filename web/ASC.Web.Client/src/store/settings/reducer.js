import { SET_USERS, SET_ADMINS, SET_OWNER, SET_GREETING_SETTINGS } from "./actions";

const initialState = {
  users: [],
  admins: [],
  owner: {},
  greetingSettings: ''
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
    case SET_GREETING_SETTINGS:
      return Object.assign({}, state, {
        greetingSettings: action.title
      });
    default:
      return state;
  }
};

export default peopleReducer;
