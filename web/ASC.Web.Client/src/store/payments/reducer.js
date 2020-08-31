// import {} from "./actions";
// import { state } from "asc-web-common";
// const { culture } = state.auth.settings.culture;

const initialState = {
  salesEmail: "sales@onlyoffice.com",
  helpUrl: "https://helpdesk.onlyoffice.com",
  buyUrl: "http://www.onlyoffice.com/post.ashx?type=buyenterprise",
  standaloneMode: true,
  currentLicense: {
    expiresDate: moment.utc("2020-09-01T23:59:59.000Z"),
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
