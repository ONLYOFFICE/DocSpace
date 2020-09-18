import {
  SET_SETTINGS_PAYMENTS_ENTERPRISE,
  SET_UPLOAD_PAYMENTS_ENTERPRISE_LICENSE,
  RESET_UPLOADED_LICENSE,
} from "./actions";

const initialState = {
  salesEmail: "sales@onlyoffice.com",
  helpUrl: "https://helpdesk.onlyoffice.com",
  buyUrl:
    "https://www.onlyoffice.com/enterprise-edition.aspx?type=buyenterprise",
  standaloneMode: true,
  licenseUpload: null,
  currentLicense: {
    expiresDate: new Date("2021-10-14T01:59:59"),
    trialMode: false,
  },
};

const paymentsReducer = (state = initialState, action) => {
  switch (action.type) {
    case SET_SETTINGS_PAYMENTS_ENTERPRISE:
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
    case SET_UPLOAD_PAYMENTS_ENTERPRISE_LICENSE:
      return Object.assign({}, state, {
        licenseUpload: action.message,
      });

    case RESET_UPLOADED_LICENSE:
      return Object.assign({}, state, {
        licenseUpload: null,
      });
    default:
      return state;
  }
};

export default paymentsReducer;
