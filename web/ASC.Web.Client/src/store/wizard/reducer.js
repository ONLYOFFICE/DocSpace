import { 
  SET_IS_WIZARD_LOADED, 
  SET_IS_MACHINE_NAME, 
  SET_COMPLETE
} from "./actions";

const initState = { 
  isWizardLoaded: false,
  machineName: 'unknown',
  response: null,
  wizardCompleted: false
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

    case SET_COMPLETE:
      return Object.assign({}, state, {
        response: action.res,
        wizardCompleted: true
      });

    default:
      return state;
  }
}

export default ownerReducer;