// import {} from "./actions";

const initialState = {
  salesEmail: "sales@onlyoffice.com",
  helpUrl: "https://helpdesk.onlyoffice.com",
  buyUrl: "http://www.onlyoffice.com/post.ashx?type=buyenterprise",
  standaloneMode: true,
  currentLicense: {
    expiresDate: new Date("2020-08-31T23:59:59.000Z"),
  },
  trialLicense: {
    trialMode: false,
  },
};

const paymentsReducer = (state = initialState, action) => {
  switch (action.type) {
    // case
    default:
      return state;
  }
};

export default paymentsReducer;
