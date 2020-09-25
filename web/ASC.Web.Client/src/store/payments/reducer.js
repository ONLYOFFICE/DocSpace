import { SET_PAYMENTS_SETTINGS } from "./actions";

const initialState = {
  salesEmail: "sales@onlyoffice.com",
  helpUrl: "https://helpdesk.onlyoffice.com",
  buyUrl:
    "https://www.onlyoffice.com/enterprise-edition.aspx?type=buyenterprise",
  standaloneMode: true,
  currentLicense: {
    expiresDate: new Date(),
    trialMode: true,
  },
};

const paymentsReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_PAYMENTS_SETTINGS:
      const {
        buyUrl,
        salesEmail,
        currentLicense,
        standalone: standaloneMode,
        feedbackAndSupportUrl: helpUrl,
      } = action.settings;

      return Object.assign({}, state, {
        salesEmail,
        buyUrl,
        helpUrl,
        standaloneMode,
        currentLicense: Object.assign({}, state.currentLicense, {
          expiresDate:
            currentLicense && currentLicense.date
              ? new Date(currentLicense.date)
              : state.currentLicense.expiresDate,
          trialMode:
            currentLicense && currentLicense.trial
              ? currentLicense.trial
              : state.currentLicense.trial,
        }),
      });

    default:
      return state;
  }
};

export default paymentsReducer;
