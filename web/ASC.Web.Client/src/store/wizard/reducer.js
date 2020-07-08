import { SET_IS_WIZARD_LOADED, SET_IS_MACHINE_NAME } from "./actions";

const initState = { 
  isWizardLoaded: false,
  isOwner: false,
  ownerEmail: '',
  machineName: ''
};

const ownerReducer = ( state = initState, action) => {
  switch(action.type) {

    case SET_IS_WIZARD_LOADED:
      return Object.assign({}, state, {
        isWizardLoaded: action.isWizardLoaded
      });

    case SET_IS_MACHINE_NAME:
      return Object.assign({}, state, {
        machineName: action.machineName
      });
    
    default:
      return state;
  }
}

export default ownerReducer;