
import { SET_USERS, SET_ADMINS, SET_OWNER, SET_GREETING_SETTINGS, SET_FILTER } from "./actions";
import Filter from "./filter";

const initialState = {

  security: {
    accessRight: {
      options: [],
      admins: [],
      owner: {},
      filter: Filter.getDefault()
    }
  },
  
  greetingSettings: ''
};

const peopleReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_USERS:
      return Object.assign({}, state, {
        security: Object.assign({}, state.security, {
          accessRight: Object.assign({}, state.security.accessRight, {
            options: action.options
          })          
        })
      });
    case SET_ADMINS:
        return Object.assign({}, state, {
          security: Object.assign({}, state.security, {
            accessRight: Object.assign({}, state.security.accessRight, {
              admins: action.admins
            })          
          })
        });
    case SET_OWNER:
        return Object.assign({}, state, {
          security: Object.assign({}, state.security, {
            accessRight: Object.assign({}, state.security.accessRight, {
              owner: action.owner
            })          
          })
        });
    case SET_FILTER:
        return Object.assign({}, state, {
          security: Object.assign({}, state.security, {
            accessRight: Object.assign({}, state.security.accessRight, {
              filter: action.filter
            })          
          })
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