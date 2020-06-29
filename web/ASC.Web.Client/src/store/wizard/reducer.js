import { INIT_WIZARD, SET_OWNER } from "./actions";
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
        timezone: action.owner.timezone,
        language: action.owner.language
      });
    
    default:
      return state;
  }
}

export default ownerReducer;