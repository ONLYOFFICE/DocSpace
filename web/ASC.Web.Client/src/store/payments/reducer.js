import {
  SET_SALES_EMAIL,
  SET_HELP_URL,
  SET_BUY_URL,
  SET_CURRENT_LICENSE,
  SET_SETTINGS,
  SET_STANDALONE,
} from "./actions";

const initialState = {
  salesEmail: "",
  helpUrl: "",
  buyUrl: "",
  standaloneMode: true,
  currentLicense: {
    expiresDate: new Date("2021-09-01T23:59:59.000Z"),
    trialMode: false,
  },
};

const paymentsReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_SALES_EMAIL:
      return Object.assign({}, state, {
        salesEmail: action.salesEmail,
      });
    case SET_HELP_URL:
      return Object.assign({}, state, {
        helpUrl: action.helpUrl,
      });
    case SET_BUY_URL:
      return Object.assign({}, state, {
        buyUrl: action.buyUrl,
      });
    case SET_CURRENT_LICENSE:
      return Object.assign({}, state, {
        currentLicense: {
          ...state.currentLicense,
          expiresDate: action.currentLicense.date,
          trialMode: action.currentLicense.trial,
        },
      });
    case SET_STANDALONE:
      return Object.assign({}, state, {
        standaloneMode: action.standalone,
      });
    case SET_SETTINGS:
      return Object.assign({}, state, {
        salesEmail: action.settings.salesEmail,
        helpUrl: action.settings.feedbackAndSupportUrl,
        buyUrl: action.settings.buyUrl,
      });
    default:
      return state;
  }
};

export default paymentsReducer;
