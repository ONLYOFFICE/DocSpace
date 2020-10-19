import {
  SET_IS_WIZARD_LOADED,
  SET_IS_MACHINE_NAME,
  SET_IS_LICENSE_REQUIRED,
  SET_LICENSE_UPLOAD,
  RESET_LICENSE_UPLOADED,
} from "./actions";

const initState = {
  isWizardLoaded: false,
  isLicenseRequired: false,
  machineName: "unknown",
  licenseUpload: null,
};

const ownerReducer = (state = initState, action) => {
  switch (action.type) {
    case SET_IS_WIZARD_LOADED:
      return Object.assign({}, state, {
        isWizardLoaded: action.isWizardLoaded,
      });

    case SET_IS_MACHINE_NAME:
      return Object.assign({}, state, {
        machineName: action.machineName,
      });

    case SET_IS_LICENSE_REQUIRED:
      return Object.assign({}, state, {
        isLicenseRequired: action.isRequired,
      });

    case SET_LICENSE_UPLOAD:
      return Object.assign({}, state, {
        licenseUpload: action.message,
      });

    case RESET_LICENSE_UPLOADED:
      return Object.assign({}, state, {
        licenseUpload: null,
      });

    default:
      return state;
  }
};

export default ownerReducer;
