import { SET_PAYMENTS_SETTINGS } from "./actions";

const initialState = {
  salesEmail: "sales@onlyoffice.com",
  helpUrl: "https://helpdesk.onlyoffice.com",
  buyUrl:
    "https://www.onlyoffice.com/enterprise-edition.aspx?type=buyenterprise",
  standaloneMode: true,
  currentLicense: {
    expiresDate: new Date(),
    trialMode: false,
  },
};

const paymentsReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_PAYMENTS_SETTINGS:
      return Object.assign({}, state, {
        salesEmail: action.settings.salesEmail,
        helpUrl: action.settings.feedbackAndSupportUrl,
        buyUrl: action.settings.buyUrl,
        standaloneMode: action.settings.standalone,
        currentLicense: Object.assign({}, state.currentLicense, {
          expiresDate: new Date(action.settings.currentLicense.date),
          trialMode: action.settings.currentLicense.trial,
        }),
      });

    default:
      return state;
  }
};

export default paymentsReducer;
