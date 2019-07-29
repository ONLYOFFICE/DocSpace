import config from "../../../package.json";

const initialState = {
  currentProductId: "f4d98afd-d336-4332-8778-3c6945c81ea0",
  currentCulture: "en-US",
  currentCultureName: "en-us",
  homepage: config.homepage,
  datePattern: "M/d/yyyy",
  datePatternJQ: "00/00/0000",
  dateTimePattern: "dddd, MMMM d, yyyy h:mm:ss tt",
  datepicker: {
    datePattern: "mm/dd/yy",
    dateTimePattern: "DD, mm dd, yy h:mm:ss tt",
    timePattern: "h:mm tt"
  }
};

const settingsReducer = (state = initialState, action) => {
  switch (action.type) {
    default:
      return state;
  }
};

export default settingsReducer;
