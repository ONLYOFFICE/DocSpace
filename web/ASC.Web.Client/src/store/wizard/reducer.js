import { SET_IS_WIZARD_LOADED } from "./actions";
// import { api } from "asc-web-common";

const initState = { 
  isWizardLoaded: false,
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

    case SET_IS_WIZARD_LOADED:
      return Object.assign({}, state, {
        isWizardLoaded: action.isWizardLoaded
      });
    
    default:
      return state;
  }
}

export default ownerReducer;