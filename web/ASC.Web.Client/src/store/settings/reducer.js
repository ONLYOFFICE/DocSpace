import { SET_USERS, SET_ADMINS, SET_OWNER } from "./actions";

const initialState = {

  accessRight: {
    options: [],
    admins: [],
    owner: {}
  },
};

const peopleReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_USERS:
      return Object.assign({}, state, {
        accessRight: Object.assign({}, state.accessRight, {
          options: action.options
        })
      });
    case SET_ADMINS:
      return Object.assign({}, state, {
        accessRight: Object.assign({}, state.accessRight, {
          admins: action.admins
        })
      });
    case SET_OWNER:
      return Object.assign({}, state, {
        accessRight: Object.assign({}, state.accessRight, {
          owner: action.owner
        })
      });
    default:
      return state;
  }
};

export default peopleReducer;
/*
      return Object.assign({}, state, {
        selector: Object.assign({}, state.selector, { 
          users: action.users
        })
      });
*/
