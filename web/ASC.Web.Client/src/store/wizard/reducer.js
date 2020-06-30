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
      console.log('SET_OWNER', action.owner);
      return state;

    case SET_NEW_EMAIL:
      console.log('SET_NEW_EMAIL', action.newEmail);
      return Object.assign({}, state, { ownerEmail: action.newEmail });
    
    default:
      return state;
  }
}

export default ownerReducer;