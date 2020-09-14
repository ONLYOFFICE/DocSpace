import { SET_SETTINGS } from "./actions";

const initialState = {
  salesEmail: "",
  helpUrl: "",
  buyUrl: "",
  standaloneMode: true,
  currentLicense: {
    expiresDate: new Date("2021-10-01T23:59:59.000Z"),
    trialMode: false,
  },
};

const paymentsReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_SETTINGS:
      return Object.assign({}, state, {
        salesEmail: action.settings.salesEmail,
        helpUrl: action.settings.feedbackAndSupportUrl,
        buyUrl: action.settings.buyUrl,
        standaloneMode: action.settings.standaloneMode,
        currentLicense: Object.assign({}, state.currentLicense, {
          expiresDate: action.settings.currentLicense.date,
          trialMode: action.settings.currentLicense.trial,
        }),
      });
    default:
      return state;
  }
};

export default paymentsReducer;
