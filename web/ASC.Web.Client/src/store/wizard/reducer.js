import { INIT_WIZARD, SET_OWNER, SET_NEW_EMAIL } from "./actions";
// import { api } from "asc-web-common";

const initState = { 
  isOwner: false,
  ownerEmail: '',
  domain: '',
  language: '',
  timezone: '',
  languages: [],
  timezones: []
};

const ownerReducer = ( state = initState, action) => {
  switch(action.type) {
    
    case INIT_WIZARD:
      return Object.assign({}, state, action.params);
    
    case SET_OWNER:
      return Object.assign({}, state, {
        isOwner: true,
        owner: action.owner
      });

    case SET_NEW_EMAIL:
      return Object.assign({}, state, { ownerEmail: action.newEmail });
    
    default:
      return state;
  }
}

export default ownerReducer;